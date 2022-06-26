using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numerator : Hook
{
    // UI changable Variable
    public int _signalsRequired = 2;
    int signals = 0;
    public override void Interact()
    {
        if(!paused)
        {
            if (Activated)
            {
                signals++;
                if (signals >= _signalsRequired)
                {
                    signals = 0;
                    Invokation();
                }
            }
            else Invokation();
        }  
    }
}
