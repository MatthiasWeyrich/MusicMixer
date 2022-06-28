using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class FileManager : MonoBehaviour
{ 
    // when either a music or sound file was loaded, we invoke the event with the corresponding soundclip
    // if its a sound, we also provide an integer which will be the id of the sound node
    public Action<AudioClip> RequestedSoundLoaded;
    public Action<AudioClip> RequestedMusicLoaded;
    private string _path;

    public void OpenFileExplorer(){
        _path = EditorUtility.OpenFilePanel("Available Sounds", "F:/Audio Done/A test/", "mp3");
        StartCoroutine(GetSound());
    }
    public void OpenFileExplorerMusic(){
        _path = EditorUtility.OpenFilePanel("Available Sounds", "F:/Audio Done/", "mp3");
        StartCoroutine(GetMusic());
    }

    IEnumerator GetSound(){
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_path,AudioType.MPEG)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                AudioClip requestedSound = DownloadHandlerAudioClip.GetContent(www);
                string[] s = www.url.Split("/");
                int index = s.Length;
                requestedSound.name = s[index-1];
                RequestedSoundLoaded?.Invoke(requestedSound);
                IDManager.id++;
            }
        }
    }
    IEnumerator GetMusic(){
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_path,AudioType.MPEG)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                AudioClip requestedSound = DownloadHandlerAudioClip.GetContent(www);
                string[] s = www.url.Split("/");
                int index = s.Length;
                requestedSound.name = s[index-1];
                RequestedMusicLoaded?.Invoke(requestedSound);
            }
        }
    }
}
