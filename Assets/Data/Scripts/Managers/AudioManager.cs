using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{    
    [SerializeField] public AudioMixer mixer;

    public void playSound(AudioSource source, SoundData data){
        foreach(string s in data.parameters.Keys){
            mixer.SetFloat(s,data.parameters[s]);
        }
        source.Play();
    }
    
}
