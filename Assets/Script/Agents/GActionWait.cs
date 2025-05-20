using UnityEngine;

public class GActionWait : GAction
{
    public float waitTime = 5f;

    public override void SetupAction()
    {
        preconditions.Clear();
        effects.Clear();
        duration = waitTime;
    }

    public override bool PrePerform()
    {
        Debug.Log("[Wait] Iniciando espera de " + waitTime + " segundos en el lugar.");
        return true;
    }

    public override bool PostPerform()
    {
        Debug.Log("[Wait] Espera completada.");
        return true;
    }
}

