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
        if (Input.GetMouseButton(1))
        {
            beingDragged = true;
            NotifyAllOfNodeMovement?.Invoke(node.sk._id);
        }
    }
    
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - _mouseOffset;
            Movement();
        }
    }

    public override void OnEndDrag(PointerEventData eventData){
        if(beingDragged){
            LineMovementContainer.Instance.EndOfMovement();
            beingDragged = false;
        }
    }

}
