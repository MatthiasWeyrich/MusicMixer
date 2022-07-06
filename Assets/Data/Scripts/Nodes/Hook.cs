public abstract class Hook : Intermediary
{
    public CanvasManager cm;

    protected void Invokation()
    {
        nm.NotifyChildren();
    }
}