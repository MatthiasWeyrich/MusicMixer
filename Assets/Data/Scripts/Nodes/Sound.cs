using System.Collections;
using UnityEngine;

public class Sound : Intermediary
{
    public AudioSource source { get; set; }
    
    public override void Interact()
    {
        if(!paused) { 
            source.Play();
            r.material.color = Color.magenta;
        }
        StartCoroutine("waitUntil");
    }
    IEnumerator waitUntil(){
        while(paused) yield return null;
        //while(source.isPlaying) yield return null;
        Invoke("Invokation",source.clip.length-0.09f);
    }

    private void Invokation(){
        r.material.color = defaultC;
        nm.notifyChildren();
    }
    
    public override void onStopCommand()
    {
        source.Pause();
        paused = true;
    }

    public override void OnContinueCommand()
    {
        paused = false;
        Interact();
    }

    public override void OnStartCommand()
    {
        paused = false;
    }
}
