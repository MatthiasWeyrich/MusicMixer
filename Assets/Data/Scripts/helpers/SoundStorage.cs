using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStorage
{
    private Dictionary<string, AudioClip> soundsDictionary;

    public SoundStorage()
    {
        soundsDictionary = new Dictionary<string, AudioClip>();
    }

    public void addToStorage(string name, AudioClip clip)
    {
        if(!soundsDictionary.ContainsKey(name))
            soundsDictionary.Add(name, clip);
    }

    public AudioClip getClipFromStorage(string name)
    {
        return soundsDictionary[name];
    }
}
