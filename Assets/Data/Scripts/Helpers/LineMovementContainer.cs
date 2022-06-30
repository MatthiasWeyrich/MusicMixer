using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMovementContainer
{
    private static LineMovementContainer _container;
    public static LineMovementContainer Instance{
        get{
            if(_container==null) _container = new LineMovementContainer();
            return _container;   
        }
    }
    List<LineInteraction> _lineList = new List<LineInteraction>();

    // This is called each frame a node is moved
    public void ProgressLines(){
        foreach(LineInteraction li in _lineList){
            li.MoveLine();
        }
    }

    // filled by NodeManager.MovementReaction()
    public void AddLineToMovementList(LineInteraction line){
        _lineList.Add(line);
    }

    // Movement is done, clearing the list
    public void EndOfMovement(){
        foreach(LineInteraction li in _lineList){
            li.FinishLine();
        }
        _lineList.Clear();
    }
}
