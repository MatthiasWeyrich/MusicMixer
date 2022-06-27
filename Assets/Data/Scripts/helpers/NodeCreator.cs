using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeCreator
{
    GameObject prefab;
    Dictionary<string,ObjectData> types;

    protected class ObjectData
    {
        public Color c { get; set; }
        public PrimitiveType type { get; set; }
    }
    public enum Type{
        Hook, Sound, Start, Modifier
    }

    public NodeCreator(GameObject prefab)
    {
        types = new Dictionary<string,ObjectData>();
            // setting the prefab which holds the Skeleton component used for every Node in the game
        this.prefab = prefab;
    }
    public GameObject createNewNode(string name, Type type){
        // Branching based on type
        GameObject go = null;
            // creating parent GameObject since its always the same
            // the parent holds the Skeleton component
        GameObject parent = GameObject.Instantiate(prefab, new Vector3(1, 0, 0), Quaternion.identity);
        // Branching based on type
        switch (type){
            case Type.Start:
                    // this only happens once at the start of the game
                return FormStartNode();
            default:
                if (types.ContainsKey(name))
                {
                    // If this name is already part of a node that was at one point added to the scene, we'll take a copy of it from the dictionary
                    // name could be either the name of a hook/modifier or the name of an uploaded sound file
                    go = GetNodeFromDictionary(name,parent);
                }
                else
                {
                    // Such node does not already exists, so we create a new Node of that name/type, assign it a primitive and color, and add it to the dictionary
                    ObjectData od = new ObjectData();
                    go = FormNode(type, od,parent);
                    types.Add(name, od);
                }
                return go;
        }
    }
    private GameObject GetNodeFromDictionary(string name, GameObject parent)
    {
        // Since we get here only if a Node with the same type (represented as string name) has at some point been added to the scene
        // the child is assigned the color and primitive type of the corresponding ObjectData in the dictionary
        GameObject child = GameObject.CreatePrimitive(types[name].type);
        Renderer r = child.GetComponent<Renderer>();
        r.material.color = types[name].c;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject FormStartNode()
    {
        GameObject parent = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        child.GetComponent<Renderer>().material.color = Color.black;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject FormNode(Type type, ObjectData od, GameObject parent)
    {
            // Assigning a primitive based on its type and storing the information about that node's primitive in the ObjectData object
        GameObject child = null;
        switch (type)
        {
            case Type.Sound:
                child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                od.type = PrimitiveType.Cube;
                break;
            case Type.Hook:
                child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                od.type = PrimitiveType.Sphere;
                break;
            case Type.Modifier:
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
            // Assigning color to the new Node and storing the information in the data container object
        Renderer r = child.GetComponent<Renderer>();
        Color c = Random.ColorHSV();
        r.material.color = c;
        od.c = c;
    }
}
