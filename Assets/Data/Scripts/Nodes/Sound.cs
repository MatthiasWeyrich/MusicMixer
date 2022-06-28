using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : Intermediary
{
    public AudioSource _source { get; set; }
    public CanvasBase cm;
    public SoundData data;
    public AudioManager am;
    public Dictionary<int,Parameter> _parameterList;
    protected void OnEnable()
    {
        data = new SoundData();
        _parameterList = new Dictionary<int, Parameter>();

    }
    public override void Interact()
    {
        if (!Activated)
        {
            Invokation();
            return;
        }
        if(!_paused) { 
            data.PrepareData(_parameterList);
            am.PlaySound(_source,data);
            data.ClearDictionary();
            vm._r.material.color = Color.magenta;
        }
        StartCoroutine("waitUntil");
    }
    IEnumerator waitUntil(){
        while(_paused) yield return null;
        Invoke("Invokation",_source.clip.length-0.09f);
    }
        private void Invokation(){
        if(Activated) vm._r.material.color = vm._defaultC;
        nm.NotifyChildren();
    }

    // this is called by the NodeManager if it got a notice that a node has moved
    // all nodes will check their outgoing lines and remove and destroy all that have the moved nodes id as their destination
    public void RemoveInvolvedLines(int id)
    {
        if (id == sk._id) return;
        if(_parameterList.ContainsKey(id)){
            _parameterList.Remove(id);
        }
        sk.RemoveInvolvedLines(id);
    }
    public override void onStopCommand()
    {
        _source.Pause();
        _paused = true;
    }

    public override void OnContinueCommand()
    {
        _paused = false;
    }

    public override void OnStartCommand()
    {
        _paused = false;
    }
}
