public abstract class Hook : Intermediary
{
    public CanvasManager cm;
    public override void onStopCommand()
    {
        _paused = true;
    }

    public override void OnContinueCommand()
    {
        _paused = false;
    }

    public override void OnStartCommand()
    {
        _paused = false;
    }

    protected void Invokation()
    {
        nm.NotifyChildren();
    }
}
