using UnityEngine;
using System;
using UnityEngine.Audio;

public class Assignment : MonoBehaviour
{
    private Action newMusicNotification;
    private Action<string> addNodeToDropdown;

    [SerializeField] AudioMixer mixer;
    [SerializeField] RuntimeManager rtm;
    [SerializeField] FileManager fm;
    [SerializeField] DropdownManager dm;
    NodeCreator nc;
    private SoundStorage ss;
    [SerializeField] GameObject prefab;
    
    void OnEnable(){
        nc = new NodeCreator(prefab);
        ss = new SoundStorage();
        fm.requestedSoundLoaded += getLoadedSource;
        fm.requestedMusicLoaded += getLoadedMusic;
        dm.nodeFromDropdown += getNodeFromDropdown;
        addNodeToDropdown += dm.AddNodeToDropdown;
        createStartNode();
    }
    void createStartNode()
    {
        const NodeCreator.Type type = NodeCreator.Type.Start;
        GameObject startNode = nc.createNewNode(null,type);
        Start s = startNode.AddComponent<Start>();
        addSkeletonComponents(s,0);
        AddEvents(s,type);
    }
    void getLoadedMusic(AudioClip clip){
        newMusicNotification?.Invoke();
        GameObject g = new GameObject();
        Music m = g.AddComponent<Music>();
        m.source = m.gameObject.AddComponent<AudioSource>();
            AudioMixerGroup[] amg = mixer.FindMatchingGroups(string.Empty);
            AudioMixerGroup a = amg[0];
            for(int x = 0; x < amg.Length; x++){
                if(amg[x].name.Equals("Music")) a = amg[x];
            }
            m.source.outputAudioMixerGroup = a;
        m.source.playOnAwake = false;    
        m.source.clip = clip;
        AddEvents(m);
    }

    void getLoadedSource(AudioClip clip,int id){
        GameObject node = nc.createNewNode(clip, NodeCreator.Type.Sound);
        addAudioComponent(node, clip, id);
        addUIComponents(node);
        addNodeToDropdown?.Invoke(clip.name);
        ss.addToStorage(clip.name,clip);
    }

    void addAudioComponent(GameObject node, AudioClip clip, int id){
        Sound s = node.AddComponent<Sound>();
        addSkeletonComponents(s,id);
        s.source = s.gameObject.AddComponent<AudioSource>();
            AudioMixerGroup[] amg = mixer.FindMatchingGroups(string.Empty);
            AudioMixerGroup a = amg[0];
            for(int x = 0; x < amg.Length; x++){
                if(amg[x].name.Equals("Sound")) a = amg[x];
            }
            s.source.outputAudioMixerGroup = a;
        s.source.playOnAwake = false;
        s.source.clip = clip;
        AddEvents(s,NodeCreator.Type.Sound);
    }

    void getNodeFromDropdown(string name, NodeCreator.Type type)
    {
        switch (type)
        {
            case NodeCreator.Type.Sound:
                getLoadedSource(ss.getClipFromStorage(name),IDManager.id);
                IDManager.id++;
                break;
        }
    }

    void addUIComponents(GameObject node){

    }
    void addSkeletonComponents<T>(T node, int id) where T : Node
    {
        node.skeleton = node.gameObject.GetComponent<Skeleton>();
        node.id = id;
        node.instantiateNodeManager();
    }
    void AddEvents<T>(T s, NodeCreator.Type type) where T : Node
    {
        rtm.startCommand += s.OnStartCommand;
        if (!type.Equals(NodeCreator.Type.Start))
        {
            rtm.pauseCommmand += s.onStopCommand;
            rtm.continueCommand += s.OnContinueCommand;
            s.BeingDestroyedNotice += DeleteNode;
            rtm.startCommand += s.OnStartCommand;
        }
    }
    void AddEvents(Music m)
    {
        rtm.startCommand += m.OnStartCommand;
        rtm.pauseCommmand += m.OnPauseCommand;
        rtm.continueCommand += m.OnContinueCommand;
        newMusicNotification += m.newMusicReact;
        m.onNewMusic += deleteMusic;
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
    
