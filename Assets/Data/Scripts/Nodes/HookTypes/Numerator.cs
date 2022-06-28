using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numerator : Hook
{
    int _signals = 0;
    public override void Interact()
    {
        if(!_paused)
        {
            if (Activated)
            {
                _signals++;
                if (_signals >= cm._value)
                {
                    _signals = 0;
                    Invokation();
                }
            }
            else Invokation();
        }  
    }
}
