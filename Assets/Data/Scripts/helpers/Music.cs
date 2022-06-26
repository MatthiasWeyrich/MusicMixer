using UnityEngine;
using System;

public class Music : MonoBehaviour
{
    public Action<Music> onNewMusic;
    public AudioSource source { get; set; }
    public void OnStartCommand()
    {
        source.Play();
    }
    public void newMusicReact(){
        onNewMusic?.Invoke(this);
    }
    public void OnPauseCommand(){
        source.Pause();
    }
    public void OnContinueCommand(){
        source.Play();
    }

}
