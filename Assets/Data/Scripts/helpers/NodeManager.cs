using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager
{
    // If a Node is moved, all connections that node are removed, regardless of them outgoing or incoming.
    // Since it is not enough for the moved Node to destroy all its outgoing connections ...
    // we have to tell all other nodes that a node has moved and to remove all connections with that node as destination
    private static Action<int> NotifyAllOfMovement;
    // list of all nodes
    private static Dictionary<int,Node> _nodeList;
    // each Node adds a child to the set when a new outgoing line is created
    private HashSet<int> _children;
    public NodeManager(Node node){
        if(_nodeList==null) 
            _nodeList = new Dictionary<int, Node>();
        _children = new HashSet<int>();
        node.dm.DeleteLinesDueToMovement += GlobalMovementChange;
        if (node.TryGetComponent(out Sound s))
        {
            NotifyAllOfMovement += s.RemoveInvolvedLines;
        }
        else NotifyAllOfMovement += node.sk.RemoveInvolvedLines;
        node.BeingDestroyedNotice += NodeDestruction;
        node.BeingActivatedNotice += ActivationChange;
        node.BeingDeactivatedNotice += DeactivationChange;
        _nodeList.Add(node.sk._id,node);
    }
    public void AddChild(int id) => _children.Add(id);

    // after a node has Interacted, its calling all its _children that it's their turn
    public void NotifyChildren()
    {
        foreach (int id in _children)
        {
            _nodeList[id].Interact();
        }
    }

    // If a Node has moved, this function is invoked
    // its given the id of the moved node and since all connections to that moved node are gone,
    // we remove it form every possible _children list
    // we also clear the _children list of the moved node
    // afterwards, we call the event to signal all Nodes, to remove all connections to the node and destroy all corresponding line game objects
    void GlobalMovementChange(int id){
        foreach (var node in _nodeList.Values)
        {
            node.nm._children.Remove(id);
        }
        _nodeList[id].nm._children.Clear();
        NotifyAllOfMovement?.Invoke(id);
    }

    void DeactivationChange(string name, int id)
    {
        foreach(var node in _nodeList.Values)
                if (node.name == name)
                {
                    node.vm.SetColor(Color.gray);
                    if(node.sk._id != id) node._activated = false;
                }
    }
    void ActivationChange(string name, int id)
    {
        foreach (var node in _nodeList.Values)
                if (node.name == name)
                {
                    node.vm.ResetColor();
                    if(node.sk._id != id) node._activated = true;
                }
                
    }

    void NodeDestruction(Node node)
    {
        NotifyAllOfMovement -= node.sk.RemoveInvolvedLines;
        node.BeingDestroyedNotice -= NodeDestruction;
        node.BeingActivatedNotice -= ActivationChange;
        node.BeingDeactivatedNotice -= DeactivationChange;
    }
}
