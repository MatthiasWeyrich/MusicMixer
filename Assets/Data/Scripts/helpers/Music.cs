using UnityEngine;
using System;

public class Music : MonoBehaviour
{
    // When a music object was notified that are new piece of music was chosen by the user, it'll get a notification (NewMusicReact()).
    // Afterwards, the music object will fire this event and send itself to the assignment class to be destroyed
    public Action<Music> OnNewMusic;
    public AudioSource _source { get; set; }
    public void OnStartCommand()
    {
        _source.Play();
        
    }
    public void NewMusicReact(){
        OnNewMusic?.Invoke(this);
    }
    public void OnStopCommand(){
        _source.Stop();
    }
    public void OnContinueCommand(){
        _source.Play();
    }

}