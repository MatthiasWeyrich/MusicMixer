using System.Collections.Generic;
using UnityEngine;
using System;

public class NodeManager
{
    // If a <Node> is moved or destroyed, all lines that involve the <Node>
    // Since a node only ever retains information about its outgoing lines,
    // we need to notify all other nodes to inspect their lists and remove lines with the <Node> as destination 
    private static Action<int> NotifyAllOfDestruction;

    // List of all nodes
    private static Dictionary<int,Node> _nodeList;
    // A node's children it sends signals to
    private HashSet<int> _children;

    public NodeManager(Node node){
        if(_nodeList==null) 
            _nodeList = new Dictionary<int, Node>();
        _children = new HashSet<int>();
        node.dm.DeleteLinesDueToDeletion += GlobalNodeDestructionChange;
        if (node.TryGetComponent(out Sound s))
        {
            NotifyAllOfDestruction += s.RemoveInvolvedLines;
        }
        else NotifyAllOfDestruction += node.sk.RemoveInvolvedLines;
        node.BeingDestroyedNotice += NodeDestruction;
        node.BeingActivatedNotice += ActivationChange;
        node.BeingDeactivatedNotice += DeactivationChange;
        node.dm.NotifyAllOfNodeMovement += MovementReaction;
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

    // A node has moved and thus involved lines from or to it must be adjusted
    // This searches through every nodes _outgoingLines list and checks whether the moved node's id is included.
    // It that is the case, the LineInteraction object is given to the LineMovementContainer class which will conduct the adjustments
    private void MovementReaction(int id){
        foreach(int ident in _nodeList.Keys){
            foreach(LineInteraction li in _nodeList[ident].sk._outgoingLines){
                if(li.to == id || li.from == id){
                    LineMovementContainer.Instance.AddLineToMovementList(li);
                }
            }
        }
    }

    // Direct delegate for the <DeleteLinesDueToDeletion> in the skeleton class
    void GlobalNodeDestructionChange(int id){
        foreach (var node in _nodeList.Values)
        {
            node.nm._children.Remove(id);
        }
        _nodeList[id].nm._children.Clear();
        NotifyAllOfDestruction?.Invoke(id);
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
        NotifyAllOfDestruction -= node.sk.RemoveInvolvedLines;
        node.BeingDestroyedNotice -= NodeDestruction;
        node.BeingActivatedNotice -= ActivationChange;
        node.BeingDeactivatedNotice -= DeactivationChange;
    }
}
