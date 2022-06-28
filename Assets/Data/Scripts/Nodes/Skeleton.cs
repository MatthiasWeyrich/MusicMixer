using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    // Every Node has this component
    // This is responsible for creating line objects between two nodes
    // Line objects are LineRenderer Objects
    [SerializeField] GameObject _linePrefab;
    public LineInteraction currentLine;
    public List<Vector2> _linePositions;
    LineRenderer _lineRenderer;
    EdgeCollider2D _edgeCollider;
    public List<LineInteraction> _outgoingLines;
    public int _id;

    void OnEnable(){
        _outgoingLines = new List<LineInteraction>();
    }

    // this creates a new line Object and instantiates it at mouse position
    public void CreateLine(Vector3 mouseWP){
        GameObject current = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = current.AddComponent<LineInteraction>();
        _lineRenderer = currentLine.GetComponent<LineRenderer>();
        _edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        _linePositions.Clear();
        _linePositions.Add(mouseWP);
        _linePositions.Add(mouseWP);
        _lineRenderer.SetPosition(0,_linePositions[0]);
        _lineRenderer.SetPosition(1,_linePositions[1]);
        _edgeCollider.points = _linePositions.ToArray();
    }
    // during a nodes drag function, this will continuously be called to add positions the line is made up
    public void UpdateLine(Vector2 newPosition){
        _linePositions.Add(newPosition);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1,newPosition);
        _edgeCollider.points = _linePositions.ToArray();
    }
    public virtual void RemoveInvolvedLines(int id)
    {
        // this is called by the NodeManager if it got a notice that a node has moved
        // all nodes will check their outgoing lines and remove and destroy all that have the moved nodes id as their destination
        if (id == _id) return;
        for (int i = _outgoingLines.Count-1; i >=0; i--)
        {
            if (_outgoingLines[i].to == id)
            {
                LineInteraction ln = _outgoingLines[i];
                _outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
}