using UnityEngine;

public class Guard : Hook
{
    protected override void Interact()
    {
        
        if (Activated)
        {
            if (Time.realtimeSinceStartup >= cm._value)
            {
                Invokation();
            }
        }
        else Invokation();
    }
}