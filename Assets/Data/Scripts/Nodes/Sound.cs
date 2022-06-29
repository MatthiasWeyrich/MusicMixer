using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : Intermediary
{
    public AudioSource _source { get; set; }
    public CanvasBase cm;
    public SoundData data;
    public AudioManager am;
    // List of modifiers connected to a sound node
    public Dictionary<int,Parameter> _parameterList;
    protected void OnEnable()
    {
        data = new SoundData();
        _parameterList = new Dictionary<int, Parameter>();

    }
    public override void Interact()
    {
        // If deactivated, send the signal
        if (!Activated)
        {
            Invokation();
            return;
        }
        if(!_paused) { 
            // Updating the parameters we'll give to the music mixer based on modifiers in our list
            data.PrepareData(_parameterList);
            // Sending parameters and sound to the audio manager and playing the sound
            am.PlaySound(_source,data);
            // Reverting the parameters to default values
            data.ClearDictionary();
            // Effect during playback of the sound
            vm.SetColor(Color.magenta);
        }
        StartCoroutine("waitUntil");
    }
    // Waiting to notify connected children until sound has almost fully played back
    IEnumerator waitUntil(){
        while(_paused) yield return null;
        Invoke("Invokation",_source.clip.length-0.09f);
    }
    private void Invokation(){
        if(Activated) vm.ResetColor();
        if(!_wasPlaying) nm.NotifyChildren();
        _wasPlaying = false;
    }

    // Extending and preceeding the skeleton class's method since sounds have special regards to modifier (parameter) nodes
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
        if(_wasPlaying) _source.Play();
        _paused = false;
    }

    public override void OnStartCommand()
    {
        if(_source.isPlaying) _wasPlaying = true;
        _paused = false;
    }
}
