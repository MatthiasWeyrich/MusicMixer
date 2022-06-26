using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    // Every Node has this component
    // This is responsible for creating line objects between two nodes
    // Line objects are LineRenderer Objects
    [SerializeField] GameObject linePrefab;
    public LineInteraction currentLine;
    LineRenderer lineRenderer;
    // List of all position that the line is made up of
    public List<Vector2> linePositions;
    EdgeCollider2D edgeCollider;

    // this creates a new line Object and instantiates it at the position of the mouse
    public void CreateLine(Vector3 mouseWP){
        GameObject current = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = current.AddComponent<LineInteraction>();
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        linePositions.Clear();
        linePositions.Add(mouseWP);
        linePositions.Add(mouseWP);
        lineRenderer.SetPosition(0,linePositions[0]);
        lineRenderer.SetPosition(1,linePositions[1]);
        edgeCollider.points = linePositions.ToArray();
    }
    // during a nodes drag function, this will continuously be called and adds the positions to the line
    public void UpdateLine(Vector2 newPosition){
        linePositions.Add(newPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1,newPosition);
        edgeCollider.points = linePositions.ToArray();
    }
}