using UnityEngine;

using System;
public class RuntimeManager : MonoBehaviour
{
    public Action startCommand;
    public Action pauseCommmand;
    
    public void OnStartButton(){
        startCommand?.Invoke();
    }

    public void OnPauseButton(){
        pauseCommmand?.Invoke();
    }
}
