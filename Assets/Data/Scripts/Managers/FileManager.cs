using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class FileManager : MonoBehaviour
{ 
    public Action<AudioClip> requestedSoundLoaded;
    public Action<AudioClip> requestedMusicLoaded;
    private string path;
    int i,k = 0;

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
                assignClipName(requestedSound);
                requestedSoundLoaded?.Invoke(requestedSound);
            }
        }
    }
    IEnumerator getMusic(){
        using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path,AudioType.MPEG)){
            yield return www.SendWebRequest();
            if(www.result == UnityWebRequest.Result.Success){
                AudioClip requestedSound = DownloadHandlerAudioClip.GetContent(www);
                assignMusicName(requestedSound);
                requestedMusicLoaded?.Invoke(requestedSound);
            }
        }
    }

    void assignMusicName(AudioClip requestedMusic){
        requestedMusic.name = "Music"+k;
        k++;
    }

    void assignClipName(AudioClip requestedSound){
        requestedSound.name = "Sound"+i; 
        i++;
    }
}
