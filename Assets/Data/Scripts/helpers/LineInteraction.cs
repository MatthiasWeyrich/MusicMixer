using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;

public class LineInteraction : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<LineInteraction> OnLineDeletion;
    // Every line knows its origin node and its destination node
    public int from;
    public int to;

    bool beingHovered;
    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        beingHovered = true;
        StartCoroutine(CheckForDeletion());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        beingHovered = false;
    }

    IEnumerator CheckForDeletion(){
        while(beingHovered){
            if(Input.GetKey(KeyCode.D)){
                StartProcess();
                beingHovered = false;
            }
            else yield return new WaitForSeconds(1f);
        }
    }

    public void StartProcess(){
        OnLineDeletion?.Invoke(this);
    }
}
