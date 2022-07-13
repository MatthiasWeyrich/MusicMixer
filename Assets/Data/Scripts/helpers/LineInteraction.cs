using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;

public class LineInteraction : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // When a Line is deleted, we notify the Node this the line started from
    public Action<LineInteraction> OnLineDeletion;
    // Every line knows its origin node and its destination node
    public Node _from, _to;
    public int from, to;
    private LineRenderer _lineRenderer;
    private Vector3[] _positions;
    private MeshCollider _meshCollider;
    private Mesh _mesh;

    bool beingHovered = false;

    public void SetDefinitions(LineRenderer lr, Vector3[] posis, MeshCollider mc, Mesh m){
        _lineRenderer = lr;
        _positions = new Vector3[]{_from.outButton.transform.position, (_from is Parameter) ? _to.transform.position : _to.inButton.transform.position};
        _lineRenderer.SetPositions(_positions);
        _meshCollider = mc;
        _mesh = m;
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;
    }

    // Updating the two points (from, to) the line is drawn in between
    // which are just he positions of the origin and the destination node
    public void MoveLine(){
        _positions = new Vector3[]{_from.outButton.transform.position, (_from is Parameter) ? _to.transform.position : _to.inButton.transform.position};
        _lineRenderer.SetPositions(_positions);
    }

    // The line is done and we give it a collider to make it react to raycasts
    public void FinishLine(){
        _mesh = new Mesh();
        _lineRenderer.BakeMesh(_mesh);
        _meshCollider.sharedMesh = _mesh;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // supposed to be empty
    }

    // If you hover over a line, you can delete it when pressing D
    public void OnPointerEnter(PointerEventData eventData)
    {
        beingHovered = true;
        _lineRenderer.startColor = Color.gray;
        _lineRenderer.endColor = Color.gray;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _lineRenderer.startColor = Color.black;
        _lineRenderer.endColor = Color.black;
        beingHovered = false;
    }

    public void OnPointerClick(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Left) {
            OnLineDeletion?.Invoke(this);
            beingHovered = false;
        }
        
    }

}