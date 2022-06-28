using UnityEngine;

public class Guard : Hook
{
    // UI changable Variable
    public float GuardinSeconds = 20f;
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
