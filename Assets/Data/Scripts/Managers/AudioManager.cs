using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{    
    [SerializeField] public AudioMixer _mixer;

    public void PlaySound(AudioSource source, SoundData data){
        foreach(string s in data.parameters.Keys){
            _mixer.SetFloat(s,data.parameters[s]);
        }
        source.Play();
    }
    
}
