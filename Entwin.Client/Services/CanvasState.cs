using Entwin.Shared.Models;
using Entwin.Shared.Components;
using Entwin.Client.Components;
using Entwin.Client.Pages.Canvas;
namespace Entwin.Client.Services;
using System.Net.Http.Json;

public class CanvasStateService
{
    private List<BaseComponentData> _cells = new();
    private List<CanvasView.Connection> _connections = new();
    private List<ProjectSaveDTO> _availableProjects = new();

    public IReadOnlyList<ProjectSaveDTO> AvailableProjects => _availableProjects;

    public IReadOnlyList<BaseComponentData> GetCells() => _cells.AsReadOnly();
    public IReadOnlyList<CanvasView.Connection> GetConnections() => _connections.AsReadOnly();

    public event Action? OnChange;

    public void UpdateCanvasState(List<BaseComponentData> cells, List<CanvasView.Connection> connections)
    {
        _cells = cells;
        _connections = connections;
    }

    public async Task LoadProjectsAsync(HttpClient http)
    {
        var projects = await http.GetFromJsonAsync<List<ProjectSaveDTO>>("api/project/load");
        _availableProjects = projects ?? new List<ProjectSaveDTO>();
    }

    public async Task LoadSelectedAsync(HttpClient http, string projectName)
    {
        var response = await http.PostAsJsonAsync("api/project/load", projectName);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Failed to load project.");

        var dto = await response.Content.ReadFromJsonAsync<ProjectSaveDTO>();
        if (dto == null)
            throw new Exception("Invalid project data.");

        var components = dto.CanvasData.Components?
            .Select(ToClientComponent)
            .ToList() ?? new();

        var connections = dto.CanvasData.Connections?
            .Select(c => new CanvasView.Connection
            {
                From = c.From,
                From_Position = c.From_Position,
                To = c.To,
                To_Position = c.To_Position
            })
            .ToList() ?? new();

        UpdateCanvasState(components, connections);
    }

    public void AddCell(BaseComponentData cell)
    {
        _cells.Add(cell);
        OnChange?.Invoke();
    }

    public void RemoveCell(int cellId)
    {
        _cells.RemoveAll(c => c.Id == cellId);
        _connections.RemoveAll(c => c.From == cellId || c.To == cellId);
        OnChange?.Invoke();
    }

    public void UpdateCellPosition(int cellId, double x, double y)
    {
        var cell = _cells.FirstOrDefault(c => c.Id == cellId);
        if (cell != null)
        {
            cell.X = x;
            cell.Y = y;
            OnChange?.Invoke();
        }
    }

    public void AddConnection(CanvasView.Connection connection)
    {
        _connections.Add(connection);
        OnChange?.Invoke();
    }

    public void RemoveConnection(CanvasView.Connection connection)
    {
        _connections.Remove(connection);
        OnChange?.Invoke();
    }

    public void ClearSelection()
    {
        foreach (var c in _connections) c.IsSelected = false;
        foreach (var c in _cells) c.IsSelected = false;
        OnChange?.Invoke();
    }

    public void ToggleConnectionSelection(CanvasView.Connection conn)
    {
        conn.IsSelected = !conn.IsSelected;
        OnChange?.Invoke();
    }

    public void ToggleCellSelection(int cellId)
    {
        var cell = _cells.FirstOrDefault(c => c.Id == cellId);
        if (cell != null)
        {
            cell.IsSelected = !cell.IsSelected;
            OnChange?.Invoke();
        }
    }

    public void DeleteSelected()
    {
        var selectedCellIds = _cells.Where(c => c.IsSelected).Select(c => c.Id).ToHashSet();

        _connections.RemoveAll(conn => selectedCellIds.Contains(conn.From) || selectedCellIds.Contains(conn.To) || conn.IsSelected);
        _cells.RemoveAll(c => c.IsSelected);

        OnChange?.Invoke();
    }

    public async Task SaveProjectAsync(HttpClient http, string name)
    {
        var dto = new ProjectSaveDTO
        {
            Name = name,
            SavedTime = DateTime.UtcNow,
            CanvasData = new CanvasDTO
            {
                Components = _cells
                    .OfType<BaseComponentData>()
                    .Select(c => c.ToSave())
                    .ToList(),

                Connections = _connections
                    .Select(c => new ConnectionDTO
                    {
                        From = c.From,
                        From_Position = c.From_Position,
                        To = c.To,
                        To_Position = c.To_Position
                    })
                    .ToList()
            }
        };

        var response = await http.PostAsJsonAsync("api/project/save", dto);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to save project: {response.StatusCode}", null, response.StatusCode);
        }
    }

    public BaseComponentData ToClientComponent(ComponentSaveDTO dto)
    {
        return dto.SimulationData switch
        {
            CustomFunctionDTO => new CustomFunction(dto),
            ConstantDTO => new Constant(dto),
            SumDTO => new Sum(dto),
            GainDTO => new Gain(dto),
            StepDTO => new Step(dto),
            TransferFunctionDTO => new TransferFunction(dto),
            _ => throw new NotSupportedException($"Unsupported DTO type: {dto.SimulationData.GetType().Name}")
        };
    }

}
