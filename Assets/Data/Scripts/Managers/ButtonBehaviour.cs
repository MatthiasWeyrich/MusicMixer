using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    // Firing events to relevant nodes notifying them of global states
    public Action StartCommand;
    public Action StopCommand;
    
    private bool isStopped = true;
    
    public void ToggleStartStop() {

        isStopped = !isStopped;
        
        if (!isStopped)
            StartCommand?.Invoke();
        else
            StopCommand?.Invoke();

        gameObject.GetComponent<Image>().color = isStopped ? Color.green : Color.red;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText(isStopped ? "Play" : "Stop");

    }
    
}