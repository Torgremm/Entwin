using Entwin.Client.Components;
using Entwin.Client.Components.Editors;
using Entwin.Shared.Components;
using Entwin.Shared.Models;
using Entwin.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;

namespace Entwin.Client.Pages.Canvas;

public partial class CanvasView
{
    [Inject] private SimulationState SimulationState { get; set; } = default!;
    [Inject] private CanvasStateService CanvasState { get; set; } = default!;

    private ElementReference canvasRef;
    private DomRect canvasRectCache = new();

    protected override void OnInitialized()
    {
        CanvasState.OnChange += CanvasStateChanged;
    }

    private void CanvasStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        CanvasState.OnChange -= CanvasStateChanged;
    }

    private readonly List<(string Name, Type ComponentType)> availableComponents = new()
    {
        ("Constant", typeof(Constant)),
        ("Gain", typeof(Gain)),
        ("CustomFunction", typeof(CustomFunction)),
        ("Step", typeof(Step)),
        ("Sum", typeof(Sum)),
        ("TransferFunction", typeof(TransferFunction))
    };

    private IReadOnlyList<BaseComponentData> _cells => CanvasState.GetCells();
    private IReadOnlyList<Connection> _connections => CanvasState.GetConnections();

    public class Connection
    {
        public int From { get; set; }
        public int To { get; set; }
        public int From_Position { get; set; }
        public int To_Position { get; set; }
        public bool IsSelected { get; set; }
    }

    private int? draggingCellId = null;
    private double offsetX, offsetY;

    public class DomRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
    }

    private async Task RunSimulation()
    {
        try
        {
            var request = new SimulationRequestDTO
            {
                Settings = new SimulationSettingsDTO
                {
                    Duration = SimulationState.Duration,
                    TimeStep = SimulationState.TimeStep
                },

                Components = _cells.Select(cell =>
                {
                    if (cell is BaseComponentData dtoComponent)
                    {
                        if (!_connections.Any(conn => conn.From == cell.Id))
                            CanvasState.AddConnection(new Connection { From = cell.Id });

                        return dtoComponent.ToDTO();
                    }
                    else
                        throw new InvalidOperationException("Component does not support DTO conversion.");
                }).ToList(),

                Connections = _connections.Select(conn => new ConnectionDTO
                {
                    From = conn.From,
                    From_Position = conn.From_Position,
                    To = conn.To,
                    To_Position = conn.To_Position
                }).ToList()
            };

            var response = await Http.PostAsJsonAsync("api/simulation/simulate-step", request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Simulation failed: {response.StatusCode} - {msg}");
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<SimulationResultDTO>();
            if (result is null)
            {
                Console.Error.WriteLine("Simulation returned null result.");
                return;
            }

            Console.WriteLine("Simulation succeeded.");
            SimulationState.LastResult = result;

            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Simulation exception: {ex.Message}");
        }
    }


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            canvasRectCache = await JS.InvokeAsync<DomRect>("getBoundingClientRect", canvasRef);
            StateHasChanged();
        }
    }

    private async Task OnStartDrag((int Id, MouseEventArgs e) args)
    {
        var (cellId, e) = args;

        if (canvasRectCache == null)
            canvasRectCache = await JS.InvokeAsync<DomRect>("getBoundingClientRect", canvasRef);

        draggingCellId = cellId;

        var relativeX = e.ClientX - canvasRectCache.Left;
        var relativeY = e.ClientY - canvasRectCache.Top;

        var cell = _cells.FirstOrDefault(c => c.Id == cellId);
        if (cell != null)
        {
            offsetX = relativeX - cell.X;
            offsetY = relativeY - cell.Y;
        }
    }

    private void OnMouseMove(MouseEventArgs e)
    {
        if (_isDraggingOutput)
            GetCursorPosition(e);

        if (draggingCellId.HasValue && canvasRectCache != null)
        {
            var relativeX = e.ClientX - canvasRectCache.Left - offsetX;
            var relativeY = e.ClientY - canvasRectCache.Top - offsetY;

            var clampedX = Math.Clamp(relativeX, 0, canvasRectCache.Width - 100);
            var clampedY = Math.Clamp(relativeY, 0, canvasRectCache.Height - 100);

            var cell = _cells.FirstOrDefault(c => c.Id == draggingCellId.Value);
            if (cell != null)
            {
                CanvasState.UpdateCellPosition(cell.Id, clampedX, clampedY);
                StateHasChanged();
            }
        }
    }

    private void OnMouseUp(MouseEventArgs e)
    {
        draggingCellId = null;
        _isDraggingOutput = false;
    }

    private (int Id, int Index) _dragSource;
    private bool _isDraggingOutput;
    private (int X, int Y) _cursorPosition;
    private (int X, int Y) _startPosition;

    private BaseComponentData? _selectedComponent;

    private async Task OnOutputDrag((int Id, int Index, MouseEventArgs e) args)
    {
        var (cellId, outputIndex, e) = args;
        _dragSource = (cellId, outputIndex);

        if (canvasRectCache == null)
            canvasRectCache = await JS.InvokeAsync<DomRect>("getBoundingClientRect", canvasRef);

        var cell = _cells.First(c => c.Id == cellId);

        var outputX = cell.X + 100 + 6;
        var spacing = 100.0 / (cell.OutputCount + 1);
        var outputY = cell.Y + 8 + outputIndex * spacing + 6;

        _startPosition = ((int)outputX, (int)outputY);
        GetCursorPosition(e);

        _isDraggingOutput = true;
    }

    private void OnInputDropped((int Id, int Index, MouseEventArgs e) args)
    {
        if (!_isDraggingOutput)
            return;

        var (toCellId, toInputIndex, e) = args;

        if (_dragSource.Id == toCellId)
            return;

        if (_connections.Any(c => c.To == toCellId && c.To_Position == toInputIndex))
            return;

        if (_isDraggingOutput)
        {
            var connection = new Connection()
            {
                From = _dragSource.Id,
                From_Position = _dragSource.Index,
                To = toCellId,
                To_Position = toInputIndex,
                IsSelected = false
            };

            CanvasState.AddConnection(connection);

            _isDraggingOutput = false;
        }

    }

    private Dictionary<string, object> GetParameters(BaseComponentData component)
    {
        if (component == null)
            return new Dictionary<string, object>();

        return new Dictionary<string, object>
        {
            ["Component"] = component,
            ["OnClose"] = EventCallback.Factory.Create(this, () => _selectedComponent = null)
        };
    }

    private Type GetEditorType(BaseComponentData component)
    {
        return component switch
        {
            TransferFunction => typeof(TransferFunctionEditor),
            Gain => typeof(GainEditor),
            Constant => typeof(ConstantEditor),
            Step => typeof(StepEditor),
            Sum => typeof(SumEditor),
            CustomFunction => typeof(CustomFunctionEditor),
            _ => throw new ArgumentException($"No editor type defined for component of type {component.GetType().Name}")
        };
    }

    private void ShowEditor(BaseComponentData component)
    {
        _selectedComponent = component;
    }



    private Task SelectConnection((Connection conn, MouseEventArgs e) args)
    {
        var (conn, e) = args;
        bool shiftHeld = e.ShiftKey;

        if (!shiftHeld)
        {
            CanvasState.ClearSelection();
        }

        CanvasState.ToggleConnectionSelection(conn);

        if (conn.IsSelected)
        {
            var key = new SignalKey(
                conn.From,
                conn.From_Position,
                conn.To,
                conn.To_Position
            );

            ShowSignal(key);
        }

        StateHasChanged();

        return Task.CompletedTask;
    }


    private void SelectComponent(BaseComponentData cell, MouseEventArgs e)
    {
        bool shiftHeld = e.ShiftKey;

        if (!shiftHeld)
        {
            CanvasState.ClearSelection();
        }

        CanvasState.ToggleCellSelection(cell.Id);

        if (cell.IsSelected)
            _selectedComponent = cell;
        else
            _selectedComponent = null;
    }

    private void OnCanvasClick(MouseEventArgs e)
    {
        if (e.Button == 0)
        {
            CanvasState.ClearSelection();
            _selectedComponent = null;
            showContextMenu = false;
            showSignalPopup = false;
            StateHasChanged();
        }
    }


    private bool showContextMenu = false;
    private void ShowContextMenu(MouseEventArgs e)
    {
        GetCursorPosition(e);
        showContextMenu = true;
    }
    private void AddComponent(Type componentType)
    {
        var newId = _cells.Any() ? _cells.Max(c => c.Id) + 1 : 1;

        if (Activator.CreateInstance(componentType) is BaseComponentData newCell)
        {
            newCell.Id = newId;
            newCell.X = _cursorPosition.X;
            newCell.Y = _cursorPosition.Y;

            CanvasState.AddCell(newCell);
        }

        showContextMenu = false;
        StateHasChanged();
    }


    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Delete")
        {
            CanvasState.DeleteSelected();
            _selectedComponent = null;
            StateHasChanged();
        }
    }

    private void GetCursorPosition(MouseEventArgs e)
    {
        var relativeX = e.ClientX - canvasRectCache.Left;
        var relativeY = e.ClientY - canvasRectCache.Top;

        _cursorPosition.X = (int)relativeX;
        _cursorPosition.Y = (int)relativeY;
    }

    private bool ConnectionValue(int from, int from_p, int to, int to_p)
    {
        var key = new SignalKey(
            from,
            from_p,
            to,
            to_p
        );
        return SimulationState.LastResult?.Signals.ContainsKey(key) == true;
    }

    private bool showSignalPopup;
    private List<double>? selectedValues;
    private List<double>? selectedTimes;

    private void ShowSignal(SignalKey key)
    {
        if (SimulationState.LastResult?.Signals.TryGetValue(key, out var signalValues) ?? false)
        {
            selectedTimes = SimulationState.LastResult?.Time;
            selectedValues = signalValues;

            showSignalPopup = true;
        }
    }

}
