using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NodeCreator
{
    // GameObject with skeleton component
    private GameObject startPrefab;
    private GameObject soundPrefab;
    private GameObject hookPrefab;
    private GameObject modifierPrefab;
    // All unique nodes created so far
    Dictionary<string,ObjectData> _types;
    // Container class to store data about color and representation a node should have
    protected class ObjectData
    {
        public Color c { get; set; }
        public PrimitiveType type { get; set; }
    }
    public NodeCreator(GameObject startPrefab, GameObject soundPrefab, GameObject hookPrefab, GameObject modifierPrefab)
    {
        _types = new Dictionary<string,ObjectData>();

        this.startPrefab = startPrefab;
        this.soundPrefab = soundPrefab;
        this.hookPrefab = hookPrefab;
        this.modifierPrefab = modifierPrefab;
    }
    public GameObject CreateNewNode(string name, NodeType type){
        // Instantiating the skeleton prefab and assigning it an id
        GameObject prefab = null;

        switch (type) {
            case NodeType.Start:
                prefab = startPrefab;
                break;
            case NodeType.Sound:
                prefab = soundPrefab;
                break;
            case NodeType.Hook:
                prefab = hookPrefab;
                break;
            case NodeType.Modifier:
                prefab = modifierPrefab;
                break;
        }
        
        GameObject gameObject = GameObject.Instantiate(prefab, new Vector3(1, 0, 0), Quaternion.identity);
        
        //add random offset so new elements aren't entirely hidden by previous, still unmoved elements
        gameObject.transform.position += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
        
        gameObject.GetComponent<Skeleton>()._id = IDManager.id;
        IDManager.id++;
        
        if (type == NodeType.Hook || type == NodeType.Modifier)
            Array.Find(gameObject.GetComponentsInChildren<TextMeshProUGUI>(), button => button.name.Equals("Name")).SetText(name);
        
        switch (type){
            case NodeType.Start:
                return gameObject;
            default:
                if (_types.ContainsKey(name))
                {
                    // A nodes name is always representative of it's type and makes statements about its uniqueness
                    // A nodes name is either the name of an uploaded sound file, the name of a hook or the name of a modifier
                    // Since the user is not limited to the amount of equivalent nodes they can place in the scene,
                    // if there's a node with the same name, we'll take a copy of it from the dictionary
                    GetNodeFromDictionary(name,gameObject);
                    return gameObject;
                }
                else
                {
                    // Such node does not already exists, so we create a new node with that name, assign it a primitive and color, and add it to the dictionary
                    ObjectData od = new ObjectData();
                    FormNode(type, od, gameObject);
                    _types.Add(name, od);
                    return gameObject;
                }
        }
    }
    private void GetNodeFromDictionary(string name, GameObject gameObject)
    {
        // Assigning the node the same color and primitive as nodes that share the same name
        Image img = gameObject.GetComponentInChildren<Image>();
        img.color = _types[name].c;
    }
    private void FormNode(NodeType type, ObjectData od, GameObject gameObject)
    {
        switch (type)
        {
            case NodeType.Sound:
                od.type = PrimitiveType.Cube;
                break;
            case NodeType.Hook:
                od.type = PrimitiveType.Sphere;
                break;
            case NodeType.Modifier:
                od.type = PrimitiveType.Capsule;
                break;
        }
        AddColorToNode(type, gameObject, od);
    }
    private void AddColorToNode(NodeType type, GameObject gameObject, ObjectData od){
        Image img = gameObject.GetComponentInChildren<Image>();
        Color c = Color.HSVToRGB(Random.Range(0.0f, 1.0f), type == NodeType.Sound ? 1.0f : 0.5f, 1.0f);
        img.color = c;
        od.c = c;
    }
}