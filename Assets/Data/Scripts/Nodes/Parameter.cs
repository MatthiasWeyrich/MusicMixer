using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class Parameter : Node
{
    public Action<int> onDeactivationNotice;
    public Action<int> onActivationNotice;

    private bool _activated = true;
    public override bool Activated
    {
        get => _activated;
        set
        {
            _activated = value;
            if (_activated)
            {
                beingActivatedNotice?.Invoke(gameObject.name);
                onActivationNotice?.Invoke(id);
            }
            else
            {
                beingDeactivatedNotice?.Invoke(gameObject.name);
                onDeactivationNotice?.Invoke(id);
            }
        }
    }
    // overriding since you can't draw lines from a Parameter.
    // You can only connect sounds with parameters when starting from a sound node
    // this restriction is set for simplicity reasons and ease of development
    public override void OnBeginDrag(PointerEventData eventData)
    {
        mouseOffset = transform.position - GetMouseWorldPos();
        if (Input.GetMouseButton(1))
        {
            DeleteLinesDueToMovement?.Invoke(id);
            for (int i = outgoingLines.Count - 1; i >= 0; i--)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
        // same reasoning as above. One shouldn't be able to draw a line that's outgoing from a parameter
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - mouseOffset;
            //if (Input.GetKey(KeyCode.D)) OnDeletion();
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // supposed to be empty
    }

    public override void OnContinueCommand()
    {
        paused = false;
    }

    public override void OnStartCommand()
    {
        paused = false;
    }

    public override void onStopCommand()
    {
        paused = true;
    }
}
