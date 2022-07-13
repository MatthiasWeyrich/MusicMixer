using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManagerSound : DragManager
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _mouseOffset = transform.position - GetMouseWorldPos();
        
        if(Input.GetMouseButton(0))
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
                    if (i == node)
                        continue;
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
}