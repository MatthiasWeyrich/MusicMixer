using UnityEngine;
using System;

public class Assignment : MonoBehaviour
{
    private Action newMusicNotification;
    [SerializeField] RuntimeManager rtm;
    [SerializeField] FileManager fm;
    [SerializeField] GameObject prefab;
    
    void OnEnable(){
        fm.requestedSoundLoaded += getLoadedSource;
        fm.requestedMusicLoaded += getLoadedMusic;
        createStartNode();
    }
    void getLoadedMusic(AudioClip clip){
        newMusicNotification?.Invoke();
        GameObject g = new GameObject();
        Music m = g.AddComponent<Music>();
        m.source = m.gameObject.AddComponent<AudioSource>();
        m.source.clip = clip;
        rtm.startCommand += m.OnStartCommand;
        rtm.pauseCommmand += m.OnPauseCommand;
        rtm.continueCommand += m.OnContinueCommand;
        newMusicNotification += m.newMusicReact;
        m.onNewMusic += deleteMusic;
    }
    void createStartNode(){
        GameObject parent = Instantiate(prefab,new Vector3(0,0,0),Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Cube);
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        Start s = parent.AddComponent<Start>();
        s.skeleton = s.GetComponent<Skeleton>();
        s.id = 0;
        s.instantiateNodeManager();
        rtm.startCommand += s.OnStartCommand;
    }

    void getLoadedSource(AudioClip clip,int id){
        GameObject node = createSoundNode();
        addAudioComponent(node, clip, id);
        addUIComponents(node);
    }

    GameObject createSoundNode(){
        GameObject parent = Instantiate(prefab,new Vector3(2,0,0),Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }

    void addAudioComponent(GameObject node, AudioClip clip, int id){
        Sound s = node.AddComponent<Sound>();
        s.skeleton = node.GetComponent<Skeleton>();
        s.source = s.gameObject.AddComponent<AudioSource>();
        s.source.clip = clip;
        s.id = id;
        s.instantiateNodeManager();
        rtm.pauseCommmand += s.onStopCommand;
        rtm.continueCommand += s.OnContinueCommand;
        s.BeingDestroyedNotice += DeleteNode;
    }

    void addUIComponents(GameObject node){

    }

    void deleteMusic(Music m){
        rtm.startCommand -= m.OnStartCommand;
        rtm.pauseCommmand -= m.OnPauseCommand;
        rtm.continueCommand -= m.OnContinueCommand;
        newMusicNotification -= m.newMusicReact;
        Destroy(m.gameObject);
    }
    void DeleteNode(Node n){
        rtm.pauseCommmand -= n.onStopCommand;
        rtm.continueCommand -= n.onStopCommand;
        n.BeingDestroyedNotice -= DeleteNode;
        Destroy(n.gameObject);
    }
}
    
