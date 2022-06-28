using UnityEngine;
using System;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Assignment : MonoBehaviour
{
    // if there's an existing piece of music and we choose a new one, the old one is removed
    // this events calls the existing music to destroy itself
    private Action NewMusicNotification;
    // if a new sound was loaded by the filemanager, we'll notify the dropdown manager
    private Action<string> AddNodeToDropdown;

    [SerializeField] AudioMixer _mixer;
    [SerializeField] RuntimeManager rtm;
    [SerializeField] FileManager fm;
    [SerializeField] DropdownManager dm;
    [SerializeField] AudioManager am;
    NodeCreator nc;
    private SoundStorage ss;
    [SerializeField] GameObject _componentPrefab;
    [SerializeField] GameObject _canvasPrefab;
    [SerializeField] private GameObject _canvasPrefabSound;
    
    void OnEnable(){
        nc = new NodeCreator(_componentPrefab);
        ss = new SoundStorage();
        fm.RequestedSoundLoaded += GetLoadedSource;
        fm.RequestedMusicLoaded += GetLoadedMusic;
        dm.nodeFromDropdown += getNodeFromDropdown;
        AddNodeToDropdown += dm.AddNodeToDropdown;
        CreateStartNode();
    }
    void CreateStartNode()
    {
        const NodeType type = NodeType.Start;
        GameObject startNode = nc.CreateNewNode(null,type);
        Start s = ApplyStartBasics(startNode);
        s.sk._id = 0;
        AddEvents(s,type);
    }
    void GetLoadedMusic(AudioClip clip){
        // invoking the music destruction event
        NewMusicNotification?.Invoke();
        GameObject g = new GameObject();
        // adding a music and audio _source component to the gameobject
        Music m = g.AddComponent<Music>();
        m._source = m.gameObject.AddComponent<AudioSource>();
        // choosing the Music _mixer group
        AudioMixerGroup[] amg = _mixer.FindMatchingGroups(string.Empty);
        AudioMixerGroup a = amg[0];
        for(int x = 0; x < amg.Length; x++){
            if(amg[x].name.Equals("Music")) a = amg[x];
        }
        m._source.outputAudioMixerGroup = a;
        m._source.playOnAwake = false;    
        m._source.clip = clip;
        // assigning the music behavioural events like playback on playbutton press 
        AddEvents(m);
    }
    void GetSecondaryNode(string name, NodeType type)
    {
        GameObject node = nc.CreateNewNode(name, type);
        if (type.Equals(NodeType.Hook))
        {
            ApplyHookBasics(node, name);
        }
        else
        {
            ApplyParameterBasics(node, name);
        }

    }
    void GetLoadedSource(AudioClip clip){
        GameObject node = nc.CreateNewNode(clip.name, NodeType.Sound);
        Sound s = ApplySoundBasics(node);
        node.name = clip.name;
        AddAudioComponent(s, clip);
        AddNodeToDropdown?.Invoke(clip.name);
        ss.AddToStorage(clip.name,clip);
    }
    void AddAudioComponent(Sound s, AudioClip clip){
        s._source = s.gameObject.AddComponent<AudioSource>();
        AudioMixerGroup[] amg = _mixer.FindMatchingGroups(string.Empty);
        AudioMixerGroup a = amg[0];
        for(int x = 0; x < amg.Length; x++){
            if(amg[x].name.Equals("Sound")) a = amg[x];
        }
        s._source.outputAudioMixerGroup = a;
        s._source.playOnAwake = false;
        s._source.clip = clip;
        AddEvents(s,NodeType.Sound);
    }
    // this is the callback to the dropdown manager's event
    // called when the user has chosen a node from one of the dropdown menus
    // we'll create different nodes based on what was chosen
    void getNodeFromDropdown(string name, NodeType type)
    {
        switch (type)
        {
            case NodeType.Sound:
                GetLoadedSource(ss.GetClipFromStorage(name));
                break;
            default:
                GetSecondaryNode(name,type);
                break;        
        }
        IDManager.id++;
    }
    void AddEvents<T>(T s, NodeType type) where T : Node
    {
        rtm.StartCommand += s.OnStartCommand;
        if (!type.Equals(NodeType.Start))
        {
            rtm.PauseCommmand += s.onStopCommand;
            rtm.ContinueCommand += s.OnContinueCommand;
            s.BeingDestroyedNotice += DeleteNode;
            rtm.StartCommand += s.OnStartCommand;
        }
    }
    void AddEvents(Music m)
    {
        rtm.StartCommand += m.OnStartCommand;
        rtm.PauseCommmand += m.OnPauseCommand;
        rtm.ContinueCommand += m.OnContinueCommand;
        NewMusicNotification += m.NewMusicReact;
        m.OnNewMusic += deleteMusic;
    }
    // Nodes and Music will call this method with themselves as reference
    // their callbacks to this class's events will be unassigned and the object destroyed
    // NOTE: THIS MIGHT BE INCOMPLETE. MAKE SURE ALL OUTGOING AND INCOMING LINES ARE DESTROYED BEFOREHAND
    void deleteMusic(Music m){
        rtm.StartCommand -= m.OnStartCommand;
        rtm.PauseCommmand -= m.OnPauseCommand;
        rtm.ContinueCommand -= m.OnContinueCommand;
        NewMusicNotification -= m.NewMusicReact;
        Destroy(m.gameObject);
    }
    void DeleteNode(Node n){
        rtm.PauseCommmand -= n.onStopCommand;
        rtm.ContinueCommand -= n.onStopCommand;
        n.BeingDestroyedNotice -= DeleteNode;
        Destroy(n.gameObject);
    }


    Start ApplyStartBasics(GameObject go){
        Start s = go.AddComponent<Start>();
        s.sk = s.gameObject.GetComponent<Skeleton>();
        s.vm = s.gameObject.AddComponent<VisualManager>();
        s.dm = s.gameObject.AddComponent<DragManager>();
        s.nm = new NodeManager(s);
        AddEvents(s,NodeType.Start);
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
        s.cm.AddListeners();
        s.nm = new NodeManager(s);
        s.am = am;
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
        h.cm.AddListeners(minValue, maxValue);
        h.cm._slider.SetValueWithoutNotify(setValue);
        h.cm._value = setValue;
        h.cm.HandleValueChange(h.cm._value);

        h.sk = h.gameObject.GetComponent<Skeleton>();
        h.vm = h.gameObject.AddComponent<VisualManager>();
        h.dm = h.gameObject.AddComponent<DragManager>();
        h.nm = new NodeManager(h);
        h.name = name;
        AddEvents(h,NodeType.Hook);
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
        AddEvents(p,NodeType.Modifier);
    }

}
    
