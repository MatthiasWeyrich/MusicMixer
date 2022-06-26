using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager
{
    // If a Node is moved, all connections that node are removed, regardless of them outgoing or incoming.
    // Since it is not enough for the moved Node to destroy all its outgoing connections ...
    // we have to tell all other nodes that a node has moved and to remove all connections with that node as destination
    private static Action<int> notifyAllOfMovement;

    private static Dictionary<int,Node> nodeList;
    // each Node adds a child to the set when a new outgoing line is created
    private HashSet<int> children;
    public NodeManager(Node node){
        // doing some dirty work
        if(nodeList==null) 
            nodeList = new Dictionary<int, Node>();
        children = new HashSet<int>();
        node.DeleteLinesDueToMovement += globalMovementChange;
        notifyAllOfMovement += node.RemoveInvolvedLines;
        node.BeingDestroyedNotice += NodeDestruction;
        node.beingActivatedNotice += activationChange;
        node.beingDeactivatedNotice += deactivationChange;
        nodeList.Add(node.id,node);
    }
    public void addChild(int id) => children.Add(id);

    // after a node has Interacted, its calling all its children that it's their turn
    public void notifyChildren()
    {
        foreach (int id in children)
        {
            nodeList[id].Interact();
        }
    }

    // If a Node has moved, this function is invoked
    // its given the id of the moved node and since all connections to that moved node are gone,
    // we remove it form every possible children list
    // we also clear the children list of the moved node
    // afterwards, we call the event to signal all Nodes, to remove all connections to the node and destroy all corresponding line game objects
    void globalMovementChange(int id){
        foreach (var node in nodeList.Values)
        {
            node.nm.children.Remove(id);
        }
        nodeList[id].nm.children.Clear();
        notifyAllOfMovement?.Invoke(id);
    }

    void deactivationChange(string name)
    {
        foreach(var node in nodeList.Values)
            if (node.name == name)
            {
                node.SetColor(Color.gray);
            }
    }
    void activationChange(string name)
    {
        foreach (var node in nodeList.Values)
            if (node.name == name)
            {
                node.ResetColor();
            }
                
    }

    void NodeDestruction(Node node)
    {
        notifyAllOfMovement -= node.RemoveInvolvedLines;
        node.BeingDestroyedNotice -= NodeDestruction;
        node.beingActivatedNotice -= activationChange;
        node.beingDeactivatedNotice -= deactivationChange;
    }
}
