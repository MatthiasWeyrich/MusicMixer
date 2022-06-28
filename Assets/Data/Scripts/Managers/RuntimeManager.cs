using UnityEngine;

using System;
public class RuntimeManager : MonoBehaviour
{
    public Action StartCommand;
    public Action PauseCommmand;
    public Action ContinueCommand;
    public void OnStartButton(){
        StartCommand?.Invoke();
    }

    public void OnPauseButton(){
        PauseCommmand?.Invoke();
    }
    public void OnContinueButton(){
        ContinueCommand?.Invoke();
    }
}
