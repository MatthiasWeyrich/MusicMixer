using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Hook
{
    public override void Interact()
    {
        if(!_paused)
        {
            if (Activated)
                Invoke("Invokation", cm._value);
            else Invokation();
        }   
    }
}
