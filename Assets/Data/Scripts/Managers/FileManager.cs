using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class FileManager : MonoBehaviour
{ 
    public Action<AudioClip,int> requestedSoundLoaded;
    public Action<AudioClip> requestedMusicLoaded;
    private string path;

    public void OpenFileExplorer(){
        path = EditorUtility.OpenFilePanel("Available Sounds", "F:/Audio Done/", "mp3");
        StartCoroutine(getSound());
    }
    public void OpenFileExplorerMusic(){
        path = EditorUtility.OpenFilePanel("Available Sounds", "F:/Audio Done/", "mp3");
        StartCoroutine(getMusic());
    }

    IEnumerator getSound(){
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path,AudioType.MPEG)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                AudioClip requestedSound = DownloadHandlerAudioClip.GetContent(www);
                string[] s = www.url.Split("/");
                int index = s.Length;
                requestedSound.name = s[index-1];
                requestedSoundLoaded?.Invoke(requestedSound,IDManager.id);
                IDManager.id++;
            }
        }
    }
    IEnumerator getMusic(){
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path,AudioType.MPEG)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                AudioClip requestedSound = DownloadHandlerAudioClip.GetContent(www);
                string[] s = www.url.Split("/");
                int index = s.Length;
                requestedSound.name = s[index-1];
                requestedMusicLoaded?.Invoke(requestedSound);
            }
        }
    }
}
