using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager
{
    private static List<Node> nodeList;
    public static void addToList(Node node) => nodeList.Add(node);
    private List<Node> children;
    public NodeManager(){
        if(nodeList==null) nodeList = new List<Node>();
        this.children = new List<Node>();
    }
    public void addChild(Node node) => children.Add(node);
    public void removeChild(Node node) => children.Remove(node);
    public void notifyChildren() {foreach (var Node in children) { Node.Interact();}}

}
