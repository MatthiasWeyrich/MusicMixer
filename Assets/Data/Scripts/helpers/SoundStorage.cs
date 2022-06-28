using System.Collections.Generic;
using UnityEngine;

public class SoundStorage
{
    // this class is saving all uploaded sound files
    // and returning them if needed
    private Dictionary<string, AudioClip> _soundsDictionary;

    public SoundStorage() => _soundsDictionary = new Dictionary<string, AudioClip>();


    public void AddToStorage(string name, AudioClip clip)
    {
        if(!_soundsDictionary.ContainsKey(name))
            _soundsDictionary.Add(name, clip);
    }

    public AudioClip GetClipFromStorage(string name)
    {
        return _soundsDictionary[name];
    }
}
