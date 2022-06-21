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
        rtm.startCommand += s.OnStartCommand;
    }

    void getLoadedSource(AudioClip clip){
        GameObject node = createSoundNode();
        addAudioComponent(node, clip);
        addUIComponents(node);

        if(node!=null) {
            if(node.TryGetComponent<Sound>(out Sound s)){
                NodeManager.addToList(s);
            }
        }
    }

    GameObject createSoundNode(){
        GameObject parent = Instantiate(prefab,new Vector3(2,0,0),Quaternion.identity);
        GameObject child = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        child.transform.parent = parent.transform;
        child.transform.position = parent.transform.position;
        return parent;
    }

    void addAudioComponent(GameObject node, AudioClip clip){
        Sound s = node.AddComponent<Sound>();
        s.skeleton = node.GetComponent<Skeleton>();
        s.source = s.gameObject.AddComponent<AudioSource>();
        s.source.clip = clip;
        rtm.pauseCommmand += s.onStopCommand;
    }

    void addUIComponents(GameObject node){

    }

    void deleteMusic(Music m){
        rtm.startCommand -= m.OnStartCommand;
        rtm.pauseCommmand -= m.OnPauseCommand;
        newMusicNotification -= m.newMusicReact;
        Destroy(m.gameObject);
    }
}
    
