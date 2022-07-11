using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManagerSound : DragManager
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _mouseOffset = transform.position - GetMouseWorldPos();
        
        if(Input.GetMouseButton(1))
        {
            _drawing = false;
            NotifyAllOfNodeMovement?.Invoke(node.sk._id);
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
                    // Making modifier nodes a valid target
                else{
                    if(g.TryGetComponent(out Parameter p)){
                        OnObjectHover(p);
                        return;
                    }
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
    private void OnObjectHover(Parameter p){
        node.nm.AddChild(p.sk._id);
        node.sk.currentLine.to = p.sk._id;
        node.sk.currentLine._to = p;
        node.sk._outgoingLines.Add(node.sk.currentLine);
        node.sk.CreateLineMesh();
        node.GetComponent<Sound>()._parameterList.Add(p.sk._id,p);
    }
}