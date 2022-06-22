using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager
{
    private static Action<int> notifyAllChildrenOfMovement;

    private static Dictionary<int,Node> nodeList;
    private HashSet<int> children;
    public NodeManager(Node node){
        if(nodeList==null) 
            nodeList = new Dictionary<int, Node>();
        children = new HashSet<int>();
        node.DeleteLinesDueToMovement += globalMovementChange;
        notifyAllChildrenOfMovement += node.RemoveInvolvedLines;
        node.BeingDestroyedNotice += NodeDestruction;
        nodeList.Add(node.id,node);
    }
    public void addChild(int id) => children.Add(id);
    public void notifyChildren()
    {
        foreach (int id in children)
        {
            nodeList[id].Interact();
        }
    }

    void globalMovementChange(int id){
        foreach (var node in nodeList.Values)
        {
            node.nm.children.Remove(id);
        }
        nodeList[id].nm.children.Clear();
        notifyAllChildrenOfMovement?.Invoke(id);
    }

    void NodeDestruction(Node node)
    {
        notifyAllChildrenOfMovement -= node.RemoveInvolvedLines;
        node.BeingDestroyedNotice -= NodeDestruction;
    }
}
