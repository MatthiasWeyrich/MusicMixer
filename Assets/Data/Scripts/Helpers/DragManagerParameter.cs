using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManagerParameter : DragManager
{
    bool beingDragged;
    // Overriding since you can't draw lines from a parameter node.
    // You can only connect sounds with parameters when starting from a sound node
    // this restriction is set for ease of development
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _mouseOffset = transform.position - GetMouseWorldPos();
        if (Input.GetMouseButton(0))
        {
            beingDragged = true;
            NotifyAllOfNodeMovement?.Invoke(node.sk._id);
        }
    }
    
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            transform.position = GetMouseWorldPos() - _mouseOffset;
            Movement();
        }
    }

    public override void OnEndDrag(PointerEventData eventData){
        if (_drawing)
        {
            List<GameObject> hoveredObjects = eventData.hovered;
            foreach (GameObject g in hoveredObjects)
            {
                if (g.TryGetComponent(out Sound i))
                {
                    if (i != node)
                        OnObjectHover(i);
                    return;
                }
            }
            if(node.sk.currentLine.gameObject!=null) {
                node.sk.currentLine.OnLineDeletion -= LineDeletionProcess;
                Destroy(node.sk.currentLine.gameObject);
            }
        }
        else{
            LineMovementContainer.Instance.EndOfMovement();
        }
        _drawing = false;
    }

    // Overloading the method to include modifier nodes
    private void OnObjectHover(Sound s){
        node.nm.AddChild(s.sk._id);
        node.sk.currentLine.to = s.sk._id;
        node.sk.currentLine._to = s;
        node.sk._outgoingLines.Add(node.sk.currentLine);
        node.sk.CreateLineMesh();
        s.GetComponent<Sound>()._parameterList.Add(node.sk._id, (Parameter) node);
    }
}