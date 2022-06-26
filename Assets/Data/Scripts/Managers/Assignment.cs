using UnityEngine;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Assignment : MonoBehaviour
{
    // if there's an existing piece of music and we choose a new one, the old one is removed
    // this events calls the existing music to destroy itself
    private Action newMusicNotification;
    // if a new sound was loaded by the filemanager, we'll notify the dropdown manager
    private Action<string> addNodeToDropdown;

    [SerializeField] AudioMixer mixer;
    [SerializeField] RuntimeManager rtm;
    [SerializeField] FileManager fm;
    [SerializeField] DropdownManager dm;
    NodeCreator nc;
    private SoundStorage ss;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject canvasPrefab;
    
    void OnEnable(){
        // dirty work and connecting the managers via c# events
        nc = new NodeCreator(prefab);
        ss = new SoundStorage();
        fm.requestedSoundLoaded += getLoadedSource;
        fm.requestedMusicLoaded += getLoadedMusic;
        dm.nodeFromDropdown += getNodeFromDropdown;
        addNodeToDropdown += dm.AddNodeToDropdown;
        // creating mandatory start node
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
        // invoking the music destruction event
        newMusicNotification?.Invoke();
        GameObject g = new GameObject();
        // adding a music and audio source component to the gameobject
        Music m = g.AddComponent<Music>();
        m.source = m.gameObject.AddComponent<AudioSource>();
            // choosing the Music mixer group
            AudioMixerGroup[] amg = mixer.FindMatchingGroups(string.Empty);
            AudioMixerGroup a = amg[0];
            for(int x = 0; x < amg.Length; x++){
                if(amg[x].name.Equals("Music")) a = amg[x];
            }
            m.source.outputAudioMixerGroup = a;
        m.source.playOnAwake = false;    
        m.source.clip = clip;
        // assigning the music behavioural events like playback on playbutton press 
        AddEvents(m);
    }

    void getSecondaryNode(string name, int id, NodeCreator.Type type)
    {
        // delegating the creation of the node based on name to the NodeCreator class
        GameObject node = nc.createNewNode(name, type);
        // adding the actual type-based components to the game object
        if (type.Equals(NodeCreator.Type.Hook))
        {
            addHookComponent(node, name, id);
        }
        else
        {
            addModifierComponent(node, name, id);
        }

    }
    // Sound node path
    void getLoadedSource(AudioClip clip,int id){
        // creating a sound node
        GameObject node = nc.createNewNode(clip.name, NodeCreator.Type.Sound);
        // adding components to the sound node
        addAudioComponent(node, clip, id);
        addUIComponents(node);
        // notifying the dropdown manager
        addNodeToDropdown?.Invoke(clip.name);
        // adding the audioclip and its name to the list of sounds
        ss.addToStorage(clip.name,clip);
        node.name = clip.name;
    }
    void addAudioComponent(GameObject node, AudioClip clip, int id){
        // adding the sound, skeleton and audiosource components
        Sound s = node.AddComponent<Sound>();
        addSkeletonComponents(s,id);
        s.source = s.gameObject.AddComponent<AudioSource>();
        // assigning the audiosource the Sound mixer group
            AudioMixerGroup[] amg = mixer.FindMatchingGroups(string.Empty);
            AudioMixerGroup a = amg[0];
            for(int x = 0; x < amg.Length; x++){
                if(amg[x].name.Equals("Sound")) a = amg[x];
            }
            s.source.outputAudioMixerGroup = a;
        s.source.playOnAwake = false;
        s.source.clip = clip;
        // adding behavioural events
        AddEvents(s,NodeCreator.Type.Sound);
    }

    void addHookComponent(GameObject node, string name, int id)
    {
        // assigning specific hook components
        Hook h = null;
        switch (name)
        {
            case "Numerator":
                h = node.AddComponent<Numerator>();
                break;
            case "Counter":
                h = node.AddComponent<Counter>();
                break;
            case "Guard":
                h = node.AddComponent<Guard>();
                break;    
        }
        addSkeletonComponents(h,id);
        AddEvents(h, NodeCreator.Type.Hook);
        node.name = name;
    }
    
    private void addModifierComponent(GameObject node, string name, int id)
    {
        Parameter p = null;
        switch(name){
            case "Amplitude": p = node.AddComponent<Amplitude>(); break;
            case "Echo": p = node.AddComponent<Echo>(); break;
            case "Fade": p = node.AddComponent<Fade>(); break;
            case "Pitch": p = node.AddComponent<Pitch>(); break;
            case "Priority": p = node.AddComponent<Priority>(); break;
            case "Reverb": p = node.AddComponent<Reverb>(); break;
        }
        addSkeletonComponents(p,id);
        AddEvents(p, NodeCreator.Type.Modifier);
        node.name = name;
    }
    // this is the callback to the dropdown manager's event
    // called when the user has chosen a node from one of the dropdown menus
    // we'll create different nodes based on what was chosen
    void getNodeFromDropdown(string name, NodeCreator.Type type)
    {
        switch (type)
        {
            case NodeCreator.Type.Sound:
                getLoadedSource(ss.getClipFromStorage(name),IDManager.id);
                break;
            default:
                getSecondaryNode(name,IDManager.id,type);
                break;        
        }
        IDManager.id++;
    }
    void addUIComponents(GameObject node){
        GameObject o = Instantiate(canvasPrefab);
        if(node.TryGetComponent<Intermediary>(out Intermediary inter)){
            o.transform.parent = node.transform;
            o.transform.position = node.transform.position;
            o.transform.position = new Vector3(o.transform.position.x+0.05f, o.transform.position.y-1.2f,o.transform.position.z);
            inter.canvas = o.GetComponentInChildren<Canvas>();
            inter.Button = inter.canvas.GetComponentInChildren<Button>();
            inter.slider = inter.canvas.GetComponentInChildren<Slider>();
            inter.text = inter.canvas.GetComponentInChildren<TextMeshProUGUI>();
            inter.addListeners();
        }

    }
    // assigning the node-internal skeleton component, assigning it the given id and instantiating its internal NodeManager object
    void addSkeletonComponents<T>(T node, int id) where T : Node
    {
        node.skeleton = node.gameObject.GetComponent<Skeleton>();
        node.id = id;
        node.instantiateNodeManager();
    }
    // AddEvents adds behavioral events to Nodes/Music
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
    // Nodes and Music will call this method with themselves as reference
    // their callbacks to this class's events will be unassigned and the object destroyed
    // NOTE: THIS MIGHT BE INCOMPLETE. MAKE SURE ALL OUTGOING AND INCOMING LINES ARE DESTROYED BEFOREHAND
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
    
