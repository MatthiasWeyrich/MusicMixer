using UnityEngine;
using UnityEngine.EventSystems;

public class DragManagerParameter : DragManager
{
    // overriding since you can't draw lines from a Parameter.
    // You can only connect sounds with parameters when starting from a sound node
    // this restriction is set for simplicity reasons and ease of development
    public override void OnBeginDrag(PointerEventData eventData)
    {
        _mouseOffset = transform.position - GetMouseWorldPos();
        if (Input.GetMouseButton(1))
        {
            MovementStart();
        }
    }
    
    // same reasoning as above. One shouldn't be able to draw a line that's outgoing from a parameter
    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - _mouseOffset;
        }
    }

    public override void OnEndDrag(PointerEventData eventData){}

}
