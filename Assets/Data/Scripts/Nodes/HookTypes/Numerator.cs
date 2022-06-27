using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numerator : Hook
{
    int signals = 0;
    public override void Interact()
    {
        if(!paused)
        {
            if (Activated)
            {
                signals++;
                if (signals >= value)
                {
                    signals = 0;
                    Invokation();
                }
            }
            else Invokation();
        }  
    }
}
