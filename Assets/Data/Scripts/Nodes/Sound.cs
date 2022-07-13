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

    protected override void Interact()
    {
        
        if (_source.isPlaying)
            _source.Stop();

        // If activated, play the sound
        if (Activated)
        {
            // Updating the parameters we'll give to the music mixer based on modifiers in our list
            data.PrepareData(_parameterList);
            // Sending parameters and sound to the audio manager and playing the sound
            am.PlaySound(_source, data);
            // Reverting the parameters to default values
            data.ClearDictionary();
            // Effect during playback of the sound
            vm.SetColor(Color.magenta);
        }
        
        Invoke("ResetColor",_source.clip.length-0.09f);
        Invokation();
    }

    private void Invokation(){
        nm.NotifyChildren();
    }

    private void ResetColor() {
        vm.ResetColor();
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

    public override void OnStopCommand()
    {
        base.OnStopCommand();
        _source.Stop();
        ResetColor();
    }
}