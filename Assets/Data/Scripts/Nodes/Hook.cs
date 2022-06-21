public class Hook : Intermediary
{
    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        nm = new NodeManager();
    }
}
