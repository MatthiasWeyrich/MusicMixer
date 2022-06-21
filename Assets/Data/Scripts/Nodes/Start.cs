using UnityEngine;

public class Start : Node
{
    public override void Interact()
    {
        nm.notifyChildren();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        nm = new NodeManager();
        NodeManager.addToList(this);
    }
    public void OnStartCommand()
    {
        Interact();
    }
}
