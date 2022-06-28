using UnityEngine;
using System;

public abstract class Node : MonoBehaviour
{
    // if a node is destroyed by the user, the node sends itself to the Assignment class in order to be destroyed
    public Action<Node> BeingDestroyedNotice;
    // Those two are for global toggling. We give the name of a node and send it to all other nodes. If they're the same name, their activation status is also toggled.
    // they are called by the boolean property. The boolean itself is changed by camera rays
    public Action<string, int> BeingDeactivatedNotice;
    public Action<string, int> BeingActivatedNotice;

    public Skeleton sk;
    public VisualManager vm;
    public DragManager dm;
    public NodeManager nm;
    // paused whether to Interact() or not
    // drawing to make differentiation between right click (movement) and left click (drawing)
    // this can't be implemented via Input.GetMouseButton(1), since OnEndDrag is called when the MouseButton was released
    protected bool _paused;
    // activated determines, whether a sound interacts or only sends the signal further
    public bool _activated = true;
    public virtual bool Activated
    {
        get => _activated;
        set
        {
            _activated = value;
            if(_activated)
                BeingActivatedNotice?.Invoke(gameObject.name,sk._id);
            else BeingDeactivatedNotice?.Invoke(gameObject.name,sk._id);
        }
    }
    public void OnDeletion(){
        for (int i = sk._outgoingLines.Count - 1; i >= 0; i--)
        {
            LineInteraction ln = sk._outgoingLines[i];
            sk._outgoingLines.Remove(ln);
            Destroy(ln.gameObject);
        }
        BeingDestroyedNotice?.Invoke(this);
        dm.DeleteLinesDueToMovement?.Invoke(sk._id);
    }
    public abstract void Interact();
    public abstract void onStopCommand();
    public abstract void OnContinueCommand();
    public abstract void OnStartCommand();
}
