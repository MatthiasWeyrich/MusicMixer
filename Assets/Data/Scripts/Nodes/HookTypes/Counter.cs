using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Hook
{
    // UI changable Variable
    public float timeToWait = 5f;

    public override void Interact()
    {
        if(!paused)
        {
            if (Activated)
                Invoke("Invokation", timeToWait);
            else Invokation();
        }   
    }
}
