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

    public void UpdateCanvasState(List<BaseComponentData> cells, List<CanvasView.Connection> connections)
    {
        _cells = cells;
        _connections = connections;
    }

    public async Task SaveProjectAsync(HttpClient http)
    {
        var dto = new ProjectSaveDTO
        {
            SavedTime = DateTime.UtcNow,
            CanvasData = new CanvasDTO
            {
                Components = _cells
                    .OfType<ISimulatableComponent>()
                    .Select(c => c.ToDTO())
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
}
