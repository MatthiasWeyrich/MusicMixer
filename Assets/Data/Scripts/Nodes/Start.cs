using UnityEngine;

public class Start : Node
{
    public override void Interact()
    {
        nm.notifyChildren();
    }

    public override void onStopCommand()
    { 
        // supposed to be empty
    }

    public override void OnContinueCommand()
    {
        // supposed to be empty
    }

    public override void OnStartCommand()
    {
        Interact();
    }
}
