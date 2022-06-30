using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager
{
    // If a <Node> is moved or destroyed, all lines that involve the <Node>
    // Since a node only ever retains information about its outgoing lines,
    // we need to notify all other nodes to inspect their lists and remove lines with the <Node> as destination 
    private static Action<int> NotifyAllOfMovement;

    // List of all nodes
    private static Dictionary<int,Node> _nodeList;
    // A node's children it sends signals to
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
    public void RemoveChild(int id) => _children.Remove(id);

    // After a node has interacted, it signals all its children
    public void NotifyChildren()
    {
        foreach (int id in _children)
        {
            _nodeList[id].Interact();
        }
    }

    // Direct delegate for the <DeleteLinesDueToMovement> in the skeleton class
    void GlobalMovementChange(int id){
        foreach (var node in _nodeList.Values)
        {
            node.nm._children.Remove(id);
        }
        _nodeList[id].nm._children.Clear();
        NotifyAllOfMovement?.Invoke(id);
    }

    // Direct delegate for the <BeingDeactivatedNotice> in the node class
    void DeactivationChange(string name, int id)
    {
        foreach(var node in _nodeList.Values)
                if (node.name == name)
                {
                    node.vm.SetColor(Color.gray);
                    if(node.sk._id != id) node._activated = false;
                }
    }

    // Direct delegate for the <BeingActivatedNotice> in the node class
    void ActivationChange(string name, int id)
    {
        foreach (var node in _nodeList.Values)
                if (node.name == name)
                {
                    node.vm.ResetColor();
                    if(node.sk._id != id) node._activated = true;
                }
                
    }

    // Nodes send themselves here via their <BeingDestroyedNotice> event to be destroyed
    void NodeDestruction(Node node)
    {
        NotifyAllOfMovement -= node.sk.RemoveInvolvedLines;
        node.BeingDestroyedNotice -= NodeDestruction;
        node.BeingActivatedNotice -= ActivationChange;
        node.BeingDeactivatedNotice -= DeactivationChange;
    }
}
