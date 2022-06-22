using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public abstract class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<int> DeleteLinesDueToMovement;
    public Action<Node> BeingDestroyedNotice;

    public Skeleton skeleton;
    protected List<LineInteraction> outgoingLines;
    public NodeManager nm;
    protected bool paused, drawing;
    public int id;

    Renderer r;
    Color defaultC;
    protected virtual void OnEnable(){
        outgoingLines = new List<LineInteraction>();
        r = GetComponentInChildren<Renderer>();
        defaultC = r.material.color;
    }
    public void instantiateNodeManager()
    {
        nm = new NodeManager(this);
    }
    public abstract void Interact();
    public abstract void onStopCommand();
    public abstract void OnContinueCommand();
    /*
    float mouseZ;
    void OnMouseDown(){
        mouseZ = Camera.main.WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mouseZ;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag(){
        transform.position = GetMouseWorldPos() - mOffset;
    }
    */
    Vector3 mouseOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        mouseOffset = transform.position - GetMouseWorldPos();
        if(Input.GetMouseButton(0)) {
            skeleton.CreateLine(GetMouseWorldPos());
            skeleton.currentLine.from = id;
            drawing = true;
        }
        else if(Input.GetMouseButton(1))
        {
            DeleteLinesDueToMovement?.Invoke(id);
            for (int i = outgoingLines.Count - 1; i >= 0; i--)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if(Input.GetMouseButton(0)){
            Vector2 position = GetMouseWorldPos();
            if(Vector2.Distance(position, skeleton.linePositions[skeleton.linePositions.Count -1]) > .2f)
                skeleton.UpdateLine(position);
        }
        else if(Input.GetMouseButton(1)) 
            transform.position = GetMouseWorldPos() - mouseOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
            Destroy(skeleton.currentLine);
        }

        drawing = false;
    }

    public void RemoveInvolvedLines(int id)
    {
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        r.material.color = Color.red;
    }

    public void OnDrop(PointerEventData eventData)
    {
        r.material.color = defaultC;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        r.material.color = defaultC;
    }
    public void OnDeletion(){
        BeingDestroyedNotice?.Invoke(this);
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
}
