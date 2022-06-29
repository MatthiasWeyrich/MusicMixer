using UnityEngine;
using System;

public abstract class Node : MonoBehaviour
{
    // Notifying all ndes that <Node> has been destroyed and subsequently all lines to that <Node> must be removed
    // direct delegate => NodeManager
    public Action<Node> BeingDestroyedNotice;

    // When a specific node is deactivated / or reactivated, all nodes of the same type are also deactivated / reactivated. 
    // direct delegate => NodeManager
    public Action<string, int> BeingDeactivatedNotice;
    public Action<string, int> BeingActivatedNotice;

    public Skeleton sk;
    public VisualManager vm;
    public DragManager dm;
    public NodeManager nm;

    protected bool _paused;
    public bool _activated = true, _wasPlaying;
    public virtual bool Activated
    {
        get => _activated;
        set
        {
            // Firing activation events on change
            _activated = value;
            if(_activated)
                BeingActivatedNotice?.Invoke(gameObject.name,sk._id);
            else BeingDeactivatedNotice?.Invoke(gameObject.name,sk._id);
        }
    }

    // Firing the deletion event and removing all outgoing lines
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
