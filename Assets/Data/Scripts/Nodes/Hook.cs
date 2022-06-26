public abstract class Hook : Intermediary
{
    public override void onStopCommand()
    {
        paused = true;
    }

    public override void OnContinueCommand()
    {
        paused = false;
    }

    public override void OnStartCommand()
    {
        paused = false;
    }

    protected void Invokation()
    {
        nm.notifyChildren();
    }
}
