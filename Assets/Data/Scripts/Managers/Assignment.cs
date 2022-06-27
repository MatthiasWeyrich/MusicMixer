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
    [SerializeField] AudioManager am;
    NodeCreator nc;
    private SoundStorage ss;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject canvasPrefab;
    [SerializeField] private GameObject canvasPrefabSound;
    
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
        node.name = clip.name;
        // adding components to the sound node
        addAudioComponent(node, clip, id);
        addUIComponents(node);
        // notifying the dropdown manager
        addNodeToDropdown?.Invoke(clip.name);
        // adding the audioclip and its name to the list of sounds
        ss.addToStorage(clip.name,clip);
    }
    void addAudioComponent(GameObject node, AudioClip clip, int id){
        // adding the sound, skeleton and audiosource components
        Sound s = node.AddComponent<Sound>();
        addSkeletonComponents(s,id);
        s.am = am;
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
        float setValue = 0f;
        switch (name)
        {
            case "Numerator":
                h = node.AddComponent<Numerator>();
                h.minValue = 1f;
                h.maxValue = 5f;
                setValue = 1f;
                break;
            case "Counter":
                h = node.AddComponent<Counter>();
                h.minValue = 0f;
                h.maxValue = 10f;
                setValue = 0f;
                break;
            case "Guard":
                h = node.AddComponent<Guard>();
                h.minValue = 10f;
                h.maxValue = 120f;
                setValue = 10f;
                break;    
        }
        addSkeletonComponents(h,id);
        AddEvents(h, NodeCreator.Type.Hook);
        addUIComponents(node);
        node.name = name;
        h.slider.SetValueWithoutNotify(setValue);
        h.value = setValue;
        h.handleValueChange(h.value);
    }
    
    private void addModifierComponent(GameObject node, string name, int id)
    {
        Parameter p = null;
        float setValue = 0f;
        switch(name){
            case "Volume": 
            p = node.AddComponent<Volume>(); 
            p.minValue = -80f;
            p.maxValue = 20f;
            setValue = -10.0f;
            break;
            case "PitchSpeed": 
            p = node.AddComponent<PitchSpeed>(); 
            p.minValue = 1f;
            p.maxValue = 150f;
            setValue = 100f;
            break;
            case "ChorusDepth": 
            p = node.AddComponent<ChorusDepth>();
            p.minValue = 0f;
            p.maxValue = 1f; 
            setValue = 0f;
            break;
            case "ChorusRate": 
            p = node.AddComponent<ChorusRate>(); 
            p.minValue = 0f;
            p.maxValue = 20f;
            setValue = 0f;
            break;
            case "EchoDryMix": 
            p = node.AddComponent<EchoDryMix>(); 
            p.minValue = 0f;
            p.maxValue = 1f;
            setValue = 0f;
            break;
            case "FlangeDryMix": 
            p = node.AddComponent<FlangeDryMix>(); 
            p.minValue = 0f;
            p.maxValue = 1f;
            setValue = 0f;
            break;
            case "FlangeRate": 
            p = node.AddComponent<FlangeRate>(); 
            p.minValue = 0f;
            p.maxValue = 20f;
            setValue = 0f;
            break;
            case "FlangeDepth": 
            p = node.AddComponent<FlangeDepth>(); 
            p.minValue = 0.1f;
            p.maxValue = 1f;
            setValue = 0.01f;
            break;
            case "ParamEQOctaveRange": 
            p = node.AddComponent<ParamEQOctaveRange>(); 
            p.minValue = 0.2f;
            p.maxValue = 5.0f;
            setValue = 1f;
            break;
            case "ParamEQFrequencyGain": 
            p = node.AddComponent<ParamEQFrequencyGain>(); 
            p.minValue = 0.05f;
            p.maxValue = 3.0f;
            setValue = 1f;
            break;
            case "PitchShifterPitch": 
            p = node.AddComponent<PitchShifterPitch>(); 
            p.minValue = 0.5f;
            p.maxValue = 2.0f;
            setValue = 1f;
            break;
            case "ReverbRoom": 
            p = node.AddComponent<ReverbRoom>(); 
            p.minValue = -5000f;
            p.maxValue = 0f;
            setValue = -5000f;
            break;
        }
        addSkeletonComponents(p,id);
        AddEvents(p, NodeCreator.Type.Modifier);
        addUIComponents(node);
        p.slider.SetValueWithoutNotify(setValue);
        p.value = setValue;
        p.handleValueChange(p.value);
        node.name = name;
        p.paramName = name;
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
    void addUIComponents(GameObject node)
    {
        GameObject o = null;
        if (node.TryGetComponent<Sound>(out Sound s))
        {
            o = Instantiate(canvasPrefabSound);
            o.transform.parent = node.transform;
            o.transform.position = node.transform.position;
            o.transform.position = new Vector3(o.transform.position.x + 0.05f, o.transform.position.y, o.transform.position.z);
            s.canvas = o.GetComponentInChildren<Canvas>();
            s.Button = s.canvas.GetComponentInChildren<Button>();
            s.addListeners();
        }
        else if(node.TryGetComponent<Hook>(out Hook inter)){
            o = Instantiate(canvasPrefab);;
            o.transform.parent = node.transform;
            o.transform.position = node.transform.position;
            o.transform.position = new Vector3(o.transform.position.x+0.05f, o.transform.position.y-1.2f,o.transform.position.z);
            inter.canvas = o.GetComponentInChildren<Canvas>();
            inter.Button = inter.canvas.GetComponentInChildren<Button>();
            inter.slider = inter.canvas.GetComponentInChildren<Slider>();
            inter.text = inter.canvas.GetComponentInChildren<TextMeshProUGUI>();
            inter.addListeners();
        }
        else if(node.TryGetComponent<Parameter>(out Parameter p)){
            o = Instantiate(canvasPrefab);
            o.transform.parent = node.transform;
            o.transform.position = node.transform.position;
            o.transform.position = new Vector3(o.transform.position.x+0.05f, o.transform.position.y-1.2f,o.transform.position.z);
            p.canvas = o.GetComponentInChildren<Canvas>();
            p.Button = p.canvas.GetComponentInChildren<Button>();
            p.slider = p.canvas.GetComponentInChildren<Slider>();
            p.text = p.canvas.GetComponentInChildren<TextMeshProUGUI>();
            p.addListeners();
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
    
