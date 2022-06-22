public class Start : Node
{
    public override void Interact()
    {
        nm.notifyChildren();
    }
    public void OnStartCommand()
    {
        Interact();
    }

    public override void onStopCommand()
    { 
    }

    public override void OnContinueCommand()
    {
    }
}
