using System.Collections.Generic;
using UnityEngine;

public class NodeCreator
{
    // GameObject with skeleton component
    GameObject _prefab;
    // All unique nodes created so far
    Dictionary<string,ObjectData> _types;
    // Container class to store data about color and representation a node should have
    protected class ObjectData
    {
        public Color c { get; set; }
        public PrimitiveType type { get; set; }
    }
    public NodeCreator(GameObject prefab)
    {
        _types = new Dictionary<string,ObjectData>();
        this._prefab = prefab;
    }
    public GameObject CreateNewNode(string name, NodeType type){
        // Instantiating the skeleton prefab and assigning it an id
        GameObject parent = GameObject.Instantiate(_prefab, new Vector3(1, 0, 0), Quaternion.identity);
        parent.GetComponent<Skeleton>()._id = IDManager.id;
        IDManager.id++;
        switch (type){
            case NodeType.Start:
                return FormStartNode(parent);
            default:
                if (_types.ContainsKey(name))
                {
                    // A nodes name is always representative of it's type and makes statements about its uniqueness
                    // A nodes name is either the name of an uploaded sound file, the name of a hook or the name of a modifier
                    // Since the user is not limited to the amount of equivalent nodes they can place in the scene,
                    // if there's a node with the same name, we'll take a copy of it from the dictionary
                    return GetNodeFromDictionary(name,parent);
                }
                else
                {
                    GameObject go = null;
                    // Such node does not already exists, so we create a new node with that name, assign it a primitive and color, and add it to the dictionary
                    ObjectData od = new ObjectData();
                    go = FormNode(type, od,parent);
                    _types.Add(name, od);
                    return go;
                }
        }
    }
    private GameObject GetNodeFromDictionary(string name, GameObject parent)
    {
        // Assigning the node the same color and primitive as nodes that share the same name
        GameObject child = GameObject.CreatePrimitive(_types[name].type);
        Renderer r = child.GetComponent<Renderer>();
        r.material.color = _types[name].c;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject FormStartNode(GameObject parent)
    {
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        child.GetComponent<Renderer>().material.color = Color.black;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject FormNode(NodeType type, ObjectData od, GameObject parent)
    {
        GameObject child = null;
        switch (type)
        {
            case NodeType.Sound:
                child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                od.type = PrimitiveType.Cube;
                break;
            case NodeType.Hook:
                child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                od.type = PrimitiveType.Sphere;
                break;
            case NodeType.Modifier:
                child = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                child.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                od.type = PrimitiveType.Capsule;
                break;
        }
        AddColorToNode(child,od);
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private void AddColorToNode(GameObject child, ObjectData od){
        Renderer r = child.GetComponent<Renderer>();
        Color c = Random.ColorHSV();
        r.material.color = c;
        od.c = c;
    }
}
