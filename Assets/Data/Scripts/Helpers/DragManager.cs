using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Notifying all ndes that <Node> has been destroyed and subsequently all lines to that <Node> must be removed
    // direct delegate => NodeManager
    // This seems redundant to the event <BeingDestroyedNotice> in the node class
    public Action<int> DeleteLinesDueToMovement;

    public Node node;
    // Mouse offset so the dragged object's center won't snap to the mouse position
    protected Vector3 _mouseOffset;
    protected bool _drawing;
    protected void OnEnable() => node = GetComponent<Node>();
    protected void InstantiateLine(){
        node.sk.CreateLine(GetMouseWorldPos());
        node.sk.currentLine.from = node.sk._id;
        _drawing = true;
    }
    // When a node is moved, we fire the event and remove all outgoing lines the firing node has
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
        _mouseOffset = transform.position - GetMouseWorldPos();
            // Drawing a new line / making a new connections
        if(Input.GetMouseButton(0)) {
            InstantiateLine();
        }
            // Moving the node in the scene
        else if(Input.GetMouseButton(1))
        {
            MovementStart();
        }
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        // Continuously drawing the line
        if(Input.GetMouseButton(0)){
            Vector2 position = GetMouseWorldPos();
                // Making sure that a new position of the line has a minimum distance
            if(Vector2.Distance(position, node.sk._linePositions[node.sk._linePositions.Count-1]) > .2f)
                node.sk.UpdateLine(position);
        }
        else if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - _mouseOffset;
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {

        // When the user stops drawing the line.
        // If there's a valid node, we add the line to our list
        // else, the line is destroyed
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
            if(node.sk.currentLine.gameObject!=null) Destroy(node.sk.currentLine.gameObject);
        }
        _drawing = false;
    }
    
    protected void OnObjectHover(Intermediary i){
        node.nm.AddChild(i.sk._id);
        node.sk.currentLine.to = i.sk._id;
        node.sk._outgoingLines.Add(node.sk.currentLine);
    }
    // Since mouse coordinates are 2D, transforming them to 3D
    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
}
