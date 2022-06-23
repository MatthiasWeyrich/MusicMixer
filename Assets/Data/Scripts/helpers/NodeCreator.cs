using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCreator
{
    GameObject prefab;
    Dictionary<string,ObjectData> types;

    protected class ObjectData
    {
        public Renderer r { get; set; }
        public PrimitiveType type { get; set; }
    }

    public enum Type{
        Hook, Sound, Start
    }
    public NodeCreator(GameObject prefab)
    {
        types = new Dictionary<string,ObjectData>();
        this.prefab = prefab;
    } 
    public GameObject createNewNode(AudioClip clip, Type type){
        GameObject go = null;
        if(type.Equals(Type.Start)) return formStartNode();
        if (types.ContainsKey(clip.name))
        {
            go = getObjectFromDictionary(clip.name);
        }
        else
        {
            ObjectData od = new ObjectData();
            go = formNode(type, od);
            types.Add(clip.name,od);
        }
        return go;
    }

    private GameObject getObjectFromDictionary(string clip)
    {
        GameObject parent = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(types[clip].type);
        Renderer r = child.GetComponent<Renderer>();
        r = types[clip].r;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject formNode(Type type, ObjectData od){
        GameObject parent = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        GameObject child = null;
        switch(type){
            case Type.Sound:
                child = GameObject.CreatePrimitive(PrimitiveType.Cube);
                od.type = PrimitiveType.Cube;
                AddColorToNode(child,od);
                break;
            case Type.Hook:
                child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                od.type = PrimitiveType.Sphere;
                AddColorToNode(child,od);
                break;
        }
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }
    private GameObject formStartNode(){
        GameObject parent = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        child.GetComponent<Renderer>().material.color = Color.black;
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }

    private void AddColorToNode(GameObject child, ObjectData od){
        Renderer r = child.GetComponent<Renderer>();
        Color c = Random.ColorHSV();
        r.material.color = c;
        od.r = r;
    }
}
