using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    public GameObject currentLine;
    public LineRenderer lineRenderer;
    public List<Vector2> linePositions;
    public EdgeCollider2D edgeCollider;

    public void CreateLine(Vector3 mouseWP){
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        linePositions.Clear();
        linePositions.Add(mouseWP);
        linePositions.Add(mouseWP);
        lineRenderer.SetPosition(0,linePositions[0]);
        lineRenderer.SetPosition(1,linePositions[1]);
        edgeCollider.points = linePositions.ToArray();
    }
    public void UpdateLine(Vector2 newPosition){
        linePositions.Add(newPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1,newPosition);
        edgeCollider.points = linePositions.ToArray();
    }
}