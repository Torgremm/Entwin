public class LoginModalService
{
    public event Func<Task>? LoginRequested;

    public Func<Task>? DeferredAction { get; private set; }

    public async Task ShowLoginAsync(Func<Task>? afterLogin = null)
    {
        DeferredAction = afterLogin;
        if (LoginRequested != null)
        {
            await LoginRequested.Invoke();
        }
    }

    public void Clear()
    {
        DeferredAction = null;
    }
}