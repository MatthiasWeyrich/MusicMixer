using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Notifying all ndes that <Node> has been destroyed and subsequently all lines to that <Node> must be removed
    // directly delegated to => NodeManager
    // This seems redundant to the event <BeingDestroyedNotice> in the node class
    public Action<int> DeleteLinesDueToDeletion;
    // If a node is moved, all the outgoing and incoming lines need to adapt to the changing position.
    // This method's delegate is the NodeManager which will in turn give the LineMovementContainer class all lines involved in the movement
    public Action<int> NotifyAllOfNodeMovement;

    public Node node;
    // Mouse offset so the dragged object's center won't snap to the mouse position
    protected Vector3 _mouseOffset;
    protected bool _drawing;

    protected void OnEnable() {
        
        node = GetComponent<Node>();

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        
        node.inButton = Array.Find(buttons, button => button.name.Equals("InButton"));
        node.outButton = Array.Find(buttons, button => button.name.Equals("OutButton"));
        EventTrigger eventTrigger = node.outButton.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.BeginDrag;
        entry.callback.AddListener(eventData => {
            
            if (Input.GetMouseButton(0))
                InstantiateLine();

        });
        eventTrigger.triggers.Add(entry);
        
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener(eventData => {
            
            if(Input.GetMouseButton(0)){
                Vector2 position = GetMouseWorldPos();
                // Making sure that a new position of the line has a minimum distance
                //if(Vector2.Distance(position, node.sk._linePositions[node.sk._linePositions.Count-1]) > .2f)
                node.sk.UpdateLine(position);
            }

        });
        eventTrigger.triggers.Add(entry);
        
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener(eventData => {
            
            OnEndDrag((PointerEventData) eventData);

        });
        eventTrigger.triggers.Add(entry);

    }

    // Prep work for a new line
    protected void InstantiateLine(){
        node.sk.CreateLine(node.outButton.transform.position);
        node.sk.currentLine.from = node.sk._id;
        node.sk.currentLine._from = node;
        node.sk.currentLine.OnLineDeletion += LineDeletionProcess;
        _drawing = true;
    }

    // If a line is deleted, its reference needs to be removed from all lists and the object destroyed
    protected void LineDeletionProcess(LineInteraction li){
        int index = node.sk._outgoingLines.IndexOf(li);
        int childID = node.sk._outgoingLines[index].to;
        node.nm.RemoveChild(childID);
        node.sk._outgoingLines.Remove(li);
        Destroy(li.gameObject);
    }
    // On each frame that we've moved until the node is dropped, we signal each involved line to adjust itself
    protected void Movement(){
        _drawing = false;
        LineMovementContainer.Instance.ProgressLines();
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        _mouseOffset = transform.position - GetMouseWorldPos();
        
            // Moving the node in the scene
        if(Input.GetMouseButton(1))
        {
            _drawing = false;
            NotifyAllOfNodeMovement?.Invoke(node.sk._id);
        }
    }
    public virtual void OnDrag(PointerEventData eventData) {
        if (Input.GetMouseButton(1))
        {
            transform.position = GetMouseWorldPos() - _mouseOffset;
            Movement();
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
                if (g.TryGetComponent(out Intermediary i)) {
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
            // Signals the container that the movement has stopped
            LineMovementContainer.Instance.EndOfMovement();
        }
        _drawing = false;
    }
    
    // The line has finished being drawn and we can finalize it
    protected void OnObjectHover(Intermediary i){
        node.nm.AddChild(i.sk._id);
        node.sk.currentLine.to = i.sk._id;
        node.sk.currentLine._to = i;
        node.sk._outgoingLines.Add(node.sk.currentLine);
        node.sk.CreateLineMesh();
    }
    
    // Since mouse coordinates are 2D, transforming them to 3D
    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
}