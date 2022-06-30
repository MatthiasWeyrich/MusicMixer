using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    // This is responsible for creating line objects between two nodes
    // Line objects are LineRenderer Objects
    [SerializeField] GameObject _linePrefab;
    public LineInteraction currentLine;
    public List<Vector2> _linePositions;
    LineRenderer _lineRenderer;
    MeshCollider _meshCollider;
    Mesh mesh;
    // List of outgoing lines from a node
    public List<LineInteraction> _outgoingLines;

    public int _id;

    void OnEnable(){
        _outgoingLines = new List<LineInteraction>();
    }

    // On left click on a node, this creates a new line object and instantiates it at mouse position
    public void CreateLine(Vector3 mouseWP){
        GameObject current = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
        _meshCollider = current.GetComponent<MeshCollider>();
        currentLine = current.AddComponent<LineInteraction>();
        _lineRenderer = current.GetComponent<LineRenderer>();

        _linePositions.Clear();
        _linePositions.Add(mouseWP);
        _linePositions.Add(mouseWP);
        _lineRenderer.SetPosition(0,_linePositions[0]);
        _lineRenderer.SetPosition(1,_linePositions[1]);
    }
    
    // During dragging, this will continuously be called to continue the line
    public void UpdateLine(Vector2 newPosition){
        _linePositions.Add(newPosition);
        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1,newPosition);
    }

    public void CreateLineMesh(){
        mesh = new Mesh();
        _lineRenderer.BakeMesh(mesh);
        _meshCollider.sharedMesh = mesh;
    }
    
    // If a node fired it's destruction event this method is called by the NodeManager
    // Nodes will destroy all their outgoing lines that have <id> as their destination
    public virtual void RemoveInvolvedLines(int id)
    {
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