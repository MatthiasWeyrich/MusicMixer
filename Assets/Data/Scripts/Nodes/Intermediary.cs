using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Intermediary : Node
{
    // Intermediary class between Nodes and more advanced types (sound, hook, modifier)
    // we need this to simplify the check whether we've hit a VALID node in the OnEndDrag() function in the node class
    public override void OnDrag(PointerEventData eventData)
    {
        // updating the line, drawing, along the mouse position
        if (Input.GetMouseButton(0))
        {
            Vector2 position = GetMouseWorldPos();
            if (Vector2.Distance(position, skeleton.linePositions[skeleton.linePositions.Count - 1]) > .2f)
                skeleton.UpdateLine(position);
        }
        // just moving the node along the mouse position
        else if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - mouseOffset;
                // Making Intermediary Nodes able to be deleted
            //if (Input.GetKey(KeyCode.D)) OnDeletion();
        }
    }
}
   