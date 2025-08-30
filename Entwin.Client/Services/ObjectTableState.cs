using Entwin.Shared.Models;
using Entwin.Shared.Data;
using Entwin.Shared.Components;
using Entwin.Client.Components;
using Entwin.Client.Data;
using Entwin.Client.Pages.Canvas;
namespace Entwin.Client.Services;

using System.ComponentModel;
using System.Net.Http.Json;

public class ObjectTableStateService
{
    public static ObjectTableStateService? Instance { get; private set; }

    private List<RigObject> _objects = new();
    private List<ObjectParameter> _parameters = new();

    public IReadOnlyList<RigObject> GetObjects() => _objects.AsReadOnly();
    public IReadOnlyList<ObjectParameter> GetParameters()
    {
        var selectedIds = _objects.Where(o => o.IsSelected).Select(o => o.Id).ToHashSet();
        return _parameters.Where(p => selectedIds.Contains(p.ObjectId ?? 0)).ToList();
    }

    public RigObject? GetById(int id) => _objects.FirstOrDefault(o => o.Id == id);

    public event Action? OnChange;

    public void UpdateObjectState(List<RigObject> objects)
    {
        _objects = objects;
    }

    public void AddObject(RigObject obj)
    {
        _objects.Add(obj);
        OnChange?.Invoke();
    }

    public void RemoveObject(int objectId)
    {
        _objects.RemoveAll(o => o.Id == objectId);
        OnChange?.Invoke();
    }

    public void ClearSelection()
    {
        foreach (var o in _objects)
            o.IsSelected = false;
        OnChange?.Invoke();
    }

    public void ToggleObjectSelection(int objectId)
    {
        var obj = _objects.FirstOrDefault(o => o.Id == objectId);
        if (obj != null)
        {
            obj.IsSelected = !obj.IsSelected;
            OnChange?.Invoke();
        }
    }

    public void DeleteSelected()
    {
        var selectedObjectIds = _objects.Where(c => c.IsSelected).Select(c => c.Id).ToHashSet();

        _objects.RemoveAll(c => c.IsSelected);

        OnChange?.Invoke();
    }

    // public async Task SaveProjectAsync(HttpClient http, string name)
    // {
    //     var dto = new ProjectSaveDTO
    //     {
    //         Name = name,
    //         SavedTime = DateTime.UtcNow,
    //         CanvasData = new CanvasDTO
    //         {
    //             Components = _cells
    //                 .OfType<BaseComponentData>()
    //                 .Select(c => c.ToSave())
    //                 .ToList(),

    //             Connections = _connections
    //                 .Select(c => new ConnectionDTO
    //                 {
    //                     From = c.From,
    //                     From_Position = c.From_Position,
    //                     To = c.To,
    //                     To_Position = c.To_Position
    //                 })
    //                 .ToList()
    //         }
    //     };

    //     var response = await http.PostAsJsonAsync("api/project/save", dto);
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         throw new HttpRequestException($"Failed to save project: {response.StatusCode}", null, response.StatusCode);
    //     }
    // }

}
