using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Skeleton skeleton;
    List<GameObject> lines;
    protected NodeManager nm;
    protected bool pause;
    Renderer r;
    Color defaultC;
    protected virtual void OnEnable(){
        lines = new List<GameObject>();
        r = GetComponentInChildren<Renderer>();
        defaultC = r.material.color;
    }
    public abstract void Interact();
    public void onStopCommand(){
    }
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
        Debug.Log("HI");
        }
            //skeleton.CreateLine(GetMouseWorldPos());
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
        List<GameObject> p = eventData.hovered;
        foreach(GameObject g in p){
            if(g.TryGetComponent<Intermediary>(out Intermediary i)){
                nm.addChild(g.GetComponent<Sound>());
                lines.Add(skeleton.currentLine);
                return;
            }
        }
        // Right now this destroyes the current line if we move the node somewhere else, which also removes it from the list
        // You could just assign a new currentLine in the skeleton after it's been added to the list
        Destroy(skeleton.currentLine);
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
    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    } 
}
