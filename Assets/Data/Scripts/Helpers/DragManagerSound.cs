using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManagerSound : DragManager
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        // getting the offset from the center of the node to the actual position the node was clicked
        _mouseOffset = transform.position - GetMouseWorldPos();
            // if left mouse button, we draw a new line
        if(Input.GetMouseButton(0)) {
            InstantiateLine();
        }
            // else we move the line and thus need to destroy all connection -> invoking the event and destroying all outgoing lines
        else if(Input.GetMouseButton(1))
        {
            MovementStart();
            node.GetComponent<Sound>()._parameterList.Clear();
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (_drawing)
        {
            List<GameObject> hoveredObjects = eventData.hovered;
            foreach (GameObject g in hoveredObjects)
            {
                if (g.TryGetComponent(out Intermediary i))
                {
                    OnObjectHover(i);
                    return;
                }
                else{
                    if(g.TryGetComponent(out Parameter p)){
                        OnObjectHover(p);
                        return;
                    }
                }
            }
            // if no valid game object was hit, we destroy the line instead
            if(node.sk.currentLine.gameObject!=null) Destroy(node.sk.currentLine.gameObject);
        }
        _drawing = false;
    }
    private void OnObjectHover(Parameter p){
        node.nm.AddChild(p.sk._id);
        node.sk.currentLine.to 
            = 
            p.sk._id;
        node.sk._outgoingLines.Add(node.sk.currentLine);
        node.GetComponent<Sound>()._parameterList.Add(p.sk._id,p);
    }
}
