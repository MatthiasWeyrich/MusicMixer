using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{    
    [SerializeField] public AudioMixer _mixer;

    // Before playing the sound, it temporarily, for the time of playback, sets the exposed parameters of the audio mixer.
    // <data> holds all those changes to the parameters that were determined by the connections a sound has to modifiers.
    // Each modifier changes one exposed parameter. 
    // Non-influences parameters are just the default values
    public void PlaySound(AudioSource source, SoundData data){
        foreach(string s in data.parameters.Keys){
            _mixer.SetFloat(s,data.parameters[s]);
        }
        source.Play();
    }
    
}
