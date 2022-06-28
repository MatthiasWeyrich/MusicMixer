using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // If a Node was moved, all incoming and outgoing connections are removed and all corresponding drawn line (LineInteraction) game objects are destroyed
    // we'll tell all nodes in the scene to remove all connections they have that include the node that called this event (given by the id)
    // the same event is called when a node is removed from the scene by the user 
    public Action<int> DeleteLinesDueToMovement;
    // mouse offset for when we drag a node
    // if we didn't subtract the mouse offset when dragging, the node would always snap its center to the mouse position
    public Node node;
    protected Vector3 _mouseOffset;
    protected bool _drawing;
    protected void OnEnable() => node = GetComponent<Node>();
    protected void InstantiateLine(){
        node.sk.CreateLine(GetMouseWorldPos());
        node.sk.currentLine.from = node.sk._id;
        _drawing = true;
    }
    protected void MovementStart(){
        _drawing = false;
        DeleteLinesDueToMovement?.Invoke(node.sk._id);
        for (int i = node.sk._outgoingLines.Count - 1; i >= 0; i--)
        {
            LineInteraction ln = node.sk._outgoingLines[i];
            node.sk._outgoingLines.Remove(ln);
            Destroy(ln.gameObject);
        }
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
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
        }
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        // updating the line, drawing, along the mouse position
        if(Input.GetMouseButton(0)){
            Vector2 position = GetMouseWorldPos();
                // Drawing a line ONLY if it's more than 0.2 away from the last point we're drawn
            if(Vector2.Distance(position, node.sk._linePositions[node.sk._linePositions.Count-1]) > .2f)
                node.sk.UpdateLine(position);
        }
        // just moving the node along the mouse position
        else if (Input.GetMouseButton(1))
        {
            // Deletion of a Node is outsourced to inheriting classes that override this method since we never want a StartNode to be deleted.
            transform.position = GetMouseWorldPos() - _mouseOffset;
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // nothing happens if we're not drawing
        // if we're drawing however,
        // we check if any object is below the mouse courser at the moment of release
        // if that's the case, we check whether its a valid node
        // and then add it to the lines "to" field
        if (_drawing)
        {
            List<GameObject> p = eventData.hovered;
            foreach (GameObject g in p)
            {
                if (g.TryGetComponent(out Intermediary i))
                {
                    OnObjectHover(i);
                    return;
                }
            }
            // if no valid game object was hit, we destroy the line instead
            if(node.sk.currentLine.gameObject!=null) Destroy(node.sk.currentLine.gameObject);
        }
        _drawing = false;
    }
    protected void OnObjectHover(Intermediary i){
        node.nm.AddChild(i.sk._id);
        node.sk.currentLine.to = i.sk._id;
        node.sk._outgoingLines.Add(node.sk.currentLine);
    }
    // since we can interact with objects via our mouse, which is given in 2d coordinates, we need to transform these 2d coordinates into 3d
    // this only works because the camera is set to orthogonal projection
    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
}
