using UnityEngine;
using System;

public class Music : MonoBehaviour
{
    public Action<Music> OnNewMusic;
    public AudioSource _source { get; set; }
    public void OnStartCommand()
    {
        _source.Play();
    }
    public void NewMusicReact(){
        OnNewMusic?.Invoke(this);
    }
    public void OnPauseCommand(){
        _source.Pause();
    }
    public void OnContinueCommand(){
        _source.Play();
    }

}
