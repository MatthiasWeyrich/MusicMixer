using UnityEngine;

public class Sound : Intermediary
{
    public AudioSource source { get; set; }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        nm = new NodeManager();
    }
    public override void Interact()
    {
        if(!pause) source.Play();
        // This doesn't work as intended. This only just doesn't play the audio but still sends it further. But the point is that the signal remains here until unpaused
        else source.Pause();
        nm.notifyChildren();
    }

}
