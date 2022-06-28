public abstract class Parameter : Node
{
    public CanvasManager cm;
    public string _paramName;
    public override void OnContinueCommand()
    {
        _paused = false;
    }

    public override void OnStartCommand()
    {
        _paused = false;
    }

    public override void onStopCommand()
    {
        _paused = true;
    }

    public override void Interact(){}
}
