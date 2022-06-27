using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    // If a Node was moved, all incoming and outgoing connections are removed and all corresponding drawn line (LineInteraction) game objects are destroyed
    // we'll tell all nodes in the scene to remove all connections they have that include the node that called this event (given by the id)
    // the same event is called when a node is removed from the scene by the user 
    public Action<int> DeleteLinesDueToMovement;
    // if a node is destroyed by the user, the node sends itself to the Assignment class in order to be destroyed
    public Action<Node> BeingDestroyedNotice;
    // Those two are for global toggling. We give the name of a node and send it to all other nodes. If they're the same name, their activation status is also toggled.
    // they are called by the boolean property. The boolean itself is changed by camera rays
    public Action<string, int> beingDeactivatedNotice;
    public Action<string, int> beingActivatedNotice;

    // personal skeleton component
    public Skeleton skeleton;
    // list of outgoing lines
    protected List<LineInteraction> outgoingLines;
    // personal node manager for saving the children nodes. children nodes are all nodes that are a "to" field in a LineInteraction
    public NodeManager nm;
    // paused whether to Interact() or not
    // drawing to make differentiation between right click (movement) and left click (drawing)
        // this can't be implemented via Input.GetMouseButton(1), since OnEndDrag is called when the MouseButton was released
        protected bool paused, drawing;
        // activated determines, if a sound interacts or only sends the signal further
        public bool activated = true;

        // Notiying all nodes that another node was deactivated.
        public virtual bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                if(activated)
                    beingActivatedNotice?.Invoke(gameObject.name,id);
                else beingDeactivatedNotice?.Invoke(gameObject.name,id);
            }
        }
        

        // a node's personal ID
    public int id;

    // a node's renderer component and a default color
    // since we're visualizing when a node is hovered and is currently interacting, we need to save the default values to revert its color
    protected Renderer r;
    protected Color defaultC;

    // mouse offset for when we drag a node
    // if we didn't subtract the mouse offset when dragging, the node would always snap its center to the mouse position
    protected Vector3 mouseOffset;
    
    protected virtual void OnEnable(){
        outgoingLines = new List<LineInteraction>();
        r = GetComponentInChildren<Renderer>();
        defaultC = r.material.color;
    }
    public void instantiateNodeManager()
    {
        nm = new NodeManager(this);
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // getting the offset from the center of the node to the actual position the node was clicked
        mouseOffset = transform.position - GetMouseWorldPos();
            // if left mouse button, we draw a new line
        if(Input.GetMouseButton(0)) {
            skeleton.CreateLine(GetMouseWorldPos());
            skeleton.currentLine.from = id;
            drawing = true;
        }
            // else we move the line and thus need to destroy all connection -> invoking the event and destroying all outgoing lines
        else if(Input.GetMouseButton(1))
        {
            drawing = false;
            DeleteLinesDueToMovement?.Invoke(id);
            for (int i = outgoingLines.Count - 1; i >= 0; i--)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        // updating the line, drawing, along the mouse position
        if(Input.GetMouseButton(0)){
            Vector2 position = GetMouseWorldPos();
                // Drawing a line ONLY if it's more than 0.2 away from the last point we're drawn
            if(Vector2.Distance(position, skeleton.linePositions[skeleton.linePositions.Count-1]) > .2f)
                skeleton.UpdateLine(position);
        }
        // just moving the node along the mouse position
        else if (Input.GetMouseButton(1))
        {
            // Deletion of a Node is outsourced to inheriting classes that override this method since we never want a StartNode to be deleted.
            transform.position = GetMouseWorldPos() - mouseOffset;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // nothing happens if we're not drawing
        // if we're drawing however,
        // we check if any object is below the mouse courser at the moment of release
        // if that's the case, we check whether its a valid node
        // and then add it to the lines "to" field
        if (drawing)
        {
            List<GameObject> p = eventData.hovered;
            foreach (GameObject g in p)
            {
                if (g.TryGetComponent(out Intermediary i))
                {
                    Node n = g.GetComponent<Node>();
                    nm.addChild(n.id);
                    skeleton.currentLine.to = n.id;
                    outgoingLines.Add(skeleton.currentLine);
                    return;
                }
            }
            // if no valid game object was hit, we destroy the line instead
            if(skeleton.currentLine.gameObject!=null) Destroy(skeleton.currentLine.gameObject);
        }
        drawing = false;
    }

    public virtual void RemoveInvolvedLines(int id)
    {
        // this is called by the NodeManager if it got a notice that a node has moved
        // all nodes will check their outgoing lines and remove and destroy all that have the moved nodes id as their destination
        if (id == this.id) return;
        for (int i = outgoingLines.Count-1; i >=0; i--)
        {
            if (outgoingLines[i].to == id)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }
    public virtual void OnDrop(PointerEventData eventData)
    {
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }

    public void SetColor(Color c)
    {
        r.material.color = c;
    }

    public void ResetColor()
    {
        r.material.color = defaultC;
    }

    public void OnDeletion(){
        for (int i = outgoingLines.Count - 1; i >= 0; i--)
        {
            LineInteraction ln = outgoingLines[i];
            outgoingLines.Remove(ln);
            Destroy(ln.gameObject);
        }
        BeingDestroyedNotice?.Invoke(this);
        DeleteLinesDueToMovement?.Invoke(id);
    }

    // since we can interact with objects via our mouse, which is given in 2d coordinates, we need to transform these 2d coordinates into 3d
    // this only works because the camera is set to orthogonal projection
    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
    public abstract void Interact();
    public abstract void onStopCommand();
    public abstract void OnContinueCommand();
    public abstract void OnStartCommand();
}
