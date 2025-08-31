namespace Entwin.Client.Services;

public interface ITableService<T>
{
    IReadOnlyList<T> GetObjects();

    T GetById(int id);
    IReadOnlyList<T> GetByIds(IEnumerable<int> ids);

    event Action? OnChange;

    void UpdateObjectState(List<T> objects);

    void AddObject(T obj);

    void RemoveObject(int objectId);

    void ClearSelection();

    void ToggleObjectSelection(int objectId);

    void DeleteSelected();
}