public class Start : Node
{
    public override void Interact()
    {
        nm.NotifyChildren();
    }

    public override void onStopCommand(){}
    public override void OnContinueCommand(){}

    public override void OnStartCommand()
    {
        Interact();
    }
}
