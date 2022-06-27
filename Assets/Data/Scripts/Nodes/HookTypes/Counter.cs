using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Hook
{
    public override void Interact()
    {
        if(!paused)
        {
            if (Activated)
                Invoke("Invokation", value);
            else Invokation();
        }   
    }
}
