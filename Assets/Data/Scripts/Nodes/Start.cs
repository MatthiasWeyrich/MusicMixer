public class Start : Node
{
    protected override void Interact()
    {
        nm.NotifyChildren();
    }

    public override void OnStartCommand()
    {
        base.OnStartCommand();
        OnInteract();
    }
}