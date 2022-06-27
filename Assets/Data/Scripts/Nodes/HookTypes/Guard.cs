using UnityEngine;

public class Guard : Hook
{
    // UI changable Variable
    public float GuardinSeconds = 20f;
    public override void Interact()
    {
        if(!paused)
            if (Activated)
            {
                if (Time.realtimeSinceStartup >= value)
                {
                    Invokation();
                }
            }
            else Invokation();
    }
}
