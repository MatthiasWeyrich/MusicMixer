using System.Collections;
using UnityEngine;

public class Sound : Intermediary
{
    public AudioSource source { get; set; }
    
    public override void Interact()
    {
        if(!paused) source.Play();
        StartCoroutine("waitUntil");
    }

    IEnumerator waitUntil(){
        while(paused) yield return null;
        Invoke("Invokation",5f);
    }

    private void Invokation(){
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
}
