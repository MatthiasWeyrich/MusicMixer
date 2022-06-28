using UnityEngine;

public class Guard : Hook
{
    public override void Interact()
    {
        if(!_paused)
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
