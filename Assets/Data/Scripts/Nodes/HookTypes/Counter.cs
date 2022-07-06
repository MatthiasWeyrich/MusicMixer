using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : Hook
{
	protected override void Interact()
    {
        
		if (Activated)
			Invoke("Invokation", cm._value);
		else Invokation();
        
    }
}