using UnityEngine;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Assignment : MonoBehaviour
{
    // If there's an existing piece of music and we choose a new one, the old one is removed.
    // This events calls the existing music to destroy itself
    private Action NewMusicNotification;
    // If a new sound was loaded by the filemanager, we'll notify the dropdown manager
    private Action<string> AddNodeToDropdown;

    [SerializeField] AudioMixer _mixer;
    [FormerlySerializedAs("rtm")] [SerializeField] ButtonBehaviour buttonBehaviour;
    [FormerlySerializedAs("fm")] [SerializeField] FileManager fileManager;
    [FormerlySerializedAs("dm")] [SerializeField] DropdownManager dropdownManager;
    [FormerlySerializedAs("am")] [SerializeField] AudioManager audioManager;
    NodeCreator nodeCreator;
    private SoundStorage soundStorage;
    [SerializeField] GameObject startPrefab;
    [SerializeField] GameObject soundPrefab;
    [SerializeField] GameObject hookPrefab;
    [SerializeField] GameObject modifierPrefab;
    [SerializeField] GameObject _canvasPrefab;
    [SerializeField] private GameObject _canvasPrefabSound;
    
    void OnEnable(){
        nodeCreator = new NodeCreator(startPrefab, soundPrefab, hookPrefab, modifierPrefab);
        soundStorage = new SoundStorage();
        fileManager.RequestedSoundLoaded += GetLoadedSource;
        fileManager.RequestedMusicLoaded += GetLoadedMusic;
        dropdownManager.NodeFromDropdown += GetNodeFromDropdown;
        AddNodeToDropdown += dropdownManager.AddNodeToDropdown;
        CreateStartNode();
    }
    
    void CreateStartNode()
    {
        const NodeType type = NodeType.Start;
        GameObject startNode = nodeCreator.CreateNewNode(null,type);
        Start s = ApplyStartBasics(startNode);
    }
    
    void GetLoadedMusic(AudioClip clip){
        /*
            Signaling possible music objects to destroy themselves,
            creating a new music object,
            assigning it the uploaded sound file,
            assigning it a mixer group,
            and assigning it some reactional events
        */
        NewMusicNotification?.Invoke();
        GameObject g = new GameObject();
        Music m = g.AddComponent<Music>();
        m._source = m.gameObject.AddComponent<AudioSource>();
        AudioMixerGroup[] amg = _mixer.FindMatchingGroups(string.Empty);
        AudioMixerGroup a = amg[0];
        for(int x = 0; x < amg.Length; x++){
            if(amg[x].name.Equals("Music")) a = amg[x];
        }
        m._source.outputAudioMixerGroup = a;
        m._source.playOnAwake = false;
        m._source.clip = clip;
        AddEvents(m);
    }

    void GetLoadedSource(AudioClip clip){
        GameObject node = nodeCreator.CreateNewNode(clip.name, NodeType.Sound);
        Sound s = ApplySoundBasics(node);
        node.name = clip.name;
        AddAudioComponent(s, clip);
        AddNodeToDropdown?.Invoke(clip.name);
        soundStorage.AddToStorage(clip.name,clip);
    }
    
    void AddAudioComponent(Sound s, AudioClip clip){
        // Assigning audio source and mixer group to the sound node
        s._source = s.gameObject.AddComponent<AudioSource>();
        AudioMixerGroup[] amg = _mixer.FindMatchingGroups(string.Empty);
        AudioMixerGroup a = amg[0];
        for(int x = 0; x < amg.Length; x++){
            if(amg[x].name.Equals("Sound")) a = amg[x];
        }
        s._source.outputAudioMixerGroup = a;
        s._source.playOnAwake = false;
        s._source.clip = clip;
        AddEvents(s);
    }
    void GetSecondaryNode(string name, NodeType type)
    {
        GameObject node = nodeCreator.CreateNewNode(name, type);
        if (type.Equals(NodeType.Hook))
        {
            ApplyHookBasics(node, name);
        }
        else
        {
            ApplyParameterBasics(node, name);
        }
    }

    // Direct delegate to the dropdown managers event fired when a node was chosen from the dropdown
    void GetNodeFromDropdown(string name, NodeType type)
    {
        switch (type)
        {
            case NodeType.Sound:
                GetLoadedSource(soundStorage.GetClipFromStorage(name));
                break;
            default:
                GetSecondaryNode(name,type);
                break;
        }
        IDManager.id++;
    }

    void AddEvents<T>(T s) where T : Node
    {
        buttonBehaviour.StartCommand += s.OnStartCommand;
        buttonBehaviour.StopCommand += s.OnStopCommand;
        s.BeingDestroyedNotice += DeleteNode;
    }
    
    void AddEvents(Music m)
    {
        buttonBehaviour.StartCommand += m.OnStartCommand;
        buttonBehaviour.StopCommand += m.OnStopCommand;
        NewMusicNotification += m.NewMusicReact;
        m.OnNewMusic += DeleteMusic;
    }

    void DeleteMusic(Music m){
        buttonBehaviour.StartCommand -= m.OnStartCommand;
        buttonBehaviour.StopCommand -= m.OnStopCommand;
        NewMusicNotification -= m.NewMusicReact;
        Destroy(m.gameObject);
    }

    // Nodes will call this method with themselves as reference
    // their callbacks to this class's events will be unassigned and the object destroyed
    // NOTE: THIS MIGHT BE INCOMPLETE. MAKE SURE ALL OUTGOING AND INCOMING LINES ARE DESTROYED BEFOREHAND
    void DeleteNode(Node n){
        buttonBehaviour.StopCommand -= n.OnStopCommand;
        n.BeingDestroyedNotice -= DeleteNode;
        Destroy(n.gameObject);
    }

    /*
        Following methods are purely assignment of relevant classes
        and callback assignments
    */
    Start ApplyStartBasics(GameObject go){
        Start s = go.AddComponent<Start>();
        s.sk = s.gameObject.GetComponent<Skeleton>();
        s.sk._id = 0;
        s.vm = s.gameObject.AddComponent<VisualManager>();
        s.dm = s.gameObject.AddComponent<DragManager>();
        s.nm = new NodeManager(s);
        AddEvents(s);
        return s;
    }
    
    Sound ApplySoundBasics(GameObject go){
        Sound s = go.AddComponent<Sound>();
        s.sk = s.gameObject.GetComponent<Skeleton>();
        s.vm = s.gameObject.AddComponent<VisualManager>();
        s.dm = s.gameObject.AddComponent<DragManagerSound>();
        s.cm = s.gameObject.AddComponent<CanvasBase>();
        GameObject canvasPrefab = Instantiate(_canvasPrefabSound);
        canvasPrefab.transform.parent = go.transform;
        canvasPrefab.transform.position = go.transform.position;
        canvasPrefab.transform.position = new Vector3(canvasPrefab.transform.position.x+0.05f, canvasPrefab.transform.position.y-1.2f,canvasPrefab.transform.position.z);
        s.cm._canvas = canvasPrefab.GetComponentInChildren<Canvas>();
        s.cm._button = canvasPrefab.GetComponentInChildren<Button>();
        s.cm._toggle = canvasPrefab.GetComponentInChildren<Toggle>();
        s.cm.AddListeners();
        s.nm = new NodeManager(s);
        s.am = audioManager;
        return s;
    }
    
    void ApplyHookBasics(GameObject go, string name){
        Hook h = null;
        float setValue = 0f, minValue = 0f, maxValue = 0f;
        switch(name){
            case "Numerator":
                h = go.AddComponent<Numerator>();
                minValue = 1f;
                maxValue = 5f;
                setValue = 1f;
                break;
            case "Counter":
                h = go.AddComponent<Counter>();
                minValue = 0f;
                maxValue = 10f;
                setValue = 0f;
                break;
            case "Guard":
                h = go.AddComponent<Guard>();
                minValue = 10f;
                maxValue = 120f;
                setValue = 10f;
                break;
        }
        h.cm = h.gameObject.AddComponent<CanvasManager>();
        GameObject canvasPrefab = Instantiate(_canvasPrefab);
        canvasPrefab.transform.parent = go.transform;
        canvasPrefab.transform.position = go.transform.position;
        canvasPrefab.transform.position = new Vector3(canvasPrefab.transform.position.x+0.05f, canvasPrefab.transform.position.y-1.2f,canvasPrefab.transform.position.z);
        h.cm._canvas = canvasPrefab.GetComponentInChildren<Canvas>();
        h.cm._button = canvasPrefab.GetComponentInChildren<Button>();
        h.cm._slider = canvasPrefab.GetComponentInChildren<Slider>();
        h.cm._text = canvasPrefab.GetComponentInChildren<TextMeshProUGUI>();
        h.cm._toggle = canvasPrefab.GetComponentInChildren<Toggle>();
        h.cm.AddListeners(minValue, maxValue);
        h.cm._slider.SetValueWithoutNotify(setValue);
        h.cm._value = setValue;
        h.cm.HandleValueChange(h.cm._value);

        h.sk = h.gameObject.GetComponent<Skeleton>();
        h.vm = h.gameObject.AddComponent<VisualManager>();
        h.dm = h.gameObject.AddComponent<DragManager>();
        h.nm = new NodeManager(h);
        h.name = name;
        AddEvents(h);
    }
    
    void ApplyParameterBasics(GameObject go, string name){
        Parameter p = null;
        float setValue = 0f, minValue = 0f, maxValue = 0f;
        switch(name){
            case "Volume": 
            p = go.AddComponent<Volume>(); 
            minValue = -80f;
            maxValue = 20f;
            setValue = -10.0f;
            break;
            case "PitchSpeed": 
            p = go.AddComponent<PitchSpeed>(); 
            minValue = 1f;
            maxValue = 150f;
            setValue = 100f;
            break;
            case "ChorusDepth": 
            p = go.AddComponent<ChorusDepth>();
            minValue = 0f;
            maxValue = 1f; 
            setValue = 0f;
            break;
            case "ChorusRate": 
            p = go.AddComponent<ChorusRate>(); 
            minValue = 0f;
            maxValue = 20f;
            setValue = 0f;
            break;
            case "EchoDryMix": 
            p = go.AddComponent<EchoDryMix>(); 
            minValue = 0f;
            maxValue = 1f;
            setValue = 0f;
            break;
            case "FlangeDryMix": 
            p = go.AddComponent<FlangeDryMix>(); 
            minValue = 0f;
            maxValue = 1f;
            setValue = 0f;
            break;
            case "FlangeRate": 
            p = go.AddComponent<FlangeRate>(); 
            minValue = 0f;
            maxValue = 20f;
            setValue = 0f;
            break;
            case "FlangeDepth": 
            p = go.AddComponent<FlangeDepth>(); 
            minValue = 0.1f;
            maxValue = 1f;
            setValue = 0.01f;
            break;
            case "ParamEQOctaveRange": 
            p = go.AddComponent<ParamEQOctaveRange>(); 
            minValue = 0.2f;
            maxValue = 5.0f;
            setValue = 1f;
            break;
            case "ParamEQFrequencyGain": 
            p = go.AddComponent<ParamEQFrequencyGain>(); 
            minValue = 0.05f;
            maxValue = 3.0f;
            setValue = 1f;
            break;
            case "PitchShifterPitch": 
            p = go.AddComponent<PitchShifterPitch>(); 
            minValue = 0.5f;
            maxValue = 2.0f;
            setValue = 1f;
            break;
            case "ReverbRoom": 
            p = go.AddComponent<ReverbRoom>(); 
            minValue = -5000f;
            maxValue = 0f;
            setValue = -5000f;
            break;
        }
        p.cm = p.gameObject.AddComponent<CanvasManager>();
        GameObject canvasPrefab = Instantiate(_canvasPrefab);
        canvasPrefab.transform.parent = go.transform;
        canvasPrefab.transform.position = go.transform.position;
        canvasPrefab.transform.position = new Vector3(canvasPrefab.transform.position.x+0.05f, canvasPrefab.transform.position.y-1.2f,canvasPrefab.transform.position.z);
        p.cm._canvas = canvasPrefab.GetComponentInChildren<Canvas>();
        p.cm._button = canvasPrefab.GetComponentInChildren<Button>();
        p.cm._slider = canvasPrefab.GetComponentInChildren<Slider>();
        p.cm._text = canvasPrefab.GetComponentInChildren<TextMeshProUGUI>();
        p.cm._toggle = canvasPrefab.GetComponentInChildren<Toggle>();
        p.cm.AddListeners(minValue, maxValue);
        p.cm._slider.SetValueWithoutNotify(setValue);
        p.cm._value = setValue;
        p.cm.HandleValueChange(p.cm._value);

        p.sk = p.gameObject.GetComponent<Skeleton>();
        p.vm = p.gameObject.AddComponent<VisualManager>();
        p.dm = p.gameObject.AddComponent<DragManagerParameter>();
        p.nm = new NodeManager(p);
        p.name = name;
        p._paramName = name;
        AddEvents(p);
    }

}