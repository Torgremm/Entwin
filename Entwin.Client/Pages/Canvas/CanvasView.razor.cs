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

    private ElementReference canvasRef;
    private DomRect canvasRectCache = new();

    private readonly List<(string Name, Type ComponentType)> availableComponents = new()
    {
        ("Constant", typeof(Constant)),
        ("Gain", typeof(Gain)),
        ("CustomFunction", typeof(CustomFunction)),
        ("Step", typeof(Components.Step)),
        ("Sum", typeof(Sum)),
        ("TransferFunction", typeof(TransferFunction))
    };

    private readonly List<BaseComponentData> _cells = new();

    public class Connection
    {
        public int From { get; set; }
        public int To { get; set; }
        public int From_Position { get; set; }
        public int To_Position { get; set; }
        public bool IsSelected { get; set; }
    }
    private readonly List<Connection> _connections = new();



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
                Components = _cells.Select(cell =>
                {
                    if (cell is ISimulatableComponent dtoComponent)
                        return dtoComponent.ToDTO();
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
                cell.X = Math.Round(clampedX);
                cell.Y = Math.Round(clampedY);
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

            _connections.Add(connection);

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
            Components.Step => typeof(StepEditor),
            Sum => typeof(SumEditor),
            CustomFunction => typeof(CustomFunctionEditor),
            _ => throw new ArgumentException($"No editor type defined for component of type {component.GetType().Name}")
        };
    }

    private void ShowEditor(BaseComponentData component)
    {
        _selectedComponent = component;
    }



    private Task SelectConnection((CanvasView.Connection conn, MouseEventArgs e) args)
    {
        var (conn, e) = args;

        bool shiftHeld = e.ShiftKey;

        if (!shiftHeld)
        {
            foreach (var c in _connections)
                c.IsSelected = false;
            foreach (var c in _cells)
                c.IsSelected = false;
        }

        conn.IsSelected = !conn.IsSelected;

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
            foreach (var c in _cells)
                c.IsSelected = false;
            foreach (var c in _connections)
                c.IsSelected = false;
        }
        cell.IsSelected = !cell.IsSelected;

        if (cell.IsSelected)
            _selectedComponent = cell;


    }

    private void OnCanvasClick(MouseEventArgs e)
    {
        if (e.Button == 0)
        {
            foreach (var conn in _connections)
            {
                conn.IsSelected = false;
            }
            foreach (var cell in _cells)
            {
                cell.IsSelected = false;
            }

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

            _cells.Add(newCell);
        }

        showContextMenu = false;
        StateHasChanged();
    }


    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Delete")
        {
            _connections.RemoveAll(c => c.IsSelected);

            var selectedCellIds = _cells.Where(c => c.IsSelected).Select(c => c.Id).ToHashSet();
            _connections.RemoveAll(c => selectedCellIds.Contains(c.From) || selectedCellIds.Contains(c.To));
            _cells.RemoveAll(c => c.IsSelected);

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
    private SignalKey? selectedSignalKey;

    private void ShowSignal(SignalKey key)
    {
        if (SimulationState.LastResult?.Signals.TryGetValue(key, out var signalValues) ?? false)
        {
            selectedTimes = Enumerable.Range(0, signalValues.Count).Select(i => i * 0.1).ToList(); //FIX TIMESTEP
            selectedValues = new List<double>(signalValues);

            Console.WriteLine($"ShowSignal called with {selectedValues.Count} values.");
            selectedSignalKey = key;
            showSignalPopup = true;
        }
    }
}
