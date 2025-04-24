using UnityEngine;

/// <summary>
/// Asigna un rol a un NPC y configura su agente correspondiente.
/// </summary>
[RequireComponent(typeof(GAgent))]
public class NPCRoleAssigner : MonoBehaviour
{
    public enum NPCRole { Lumberjack, Miner }
    public NPCRole role;
    public int desiredAmount = 5;

    private GAgent agent;
    private LumberjackAgent lumberjackAgent;
    // Añade aquí MinerAgent si lo creas
    // private MinerAgent minerAgent;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        lumberjackAgent = GetComponent<LumberjackAgent>();
        // minerAgent = GetComponent<MinerAgent>();
    }

    void Start()
    {
        ApplyRole();
    }

    /// <summary>
    /// Configura el agente según el rol seleccionado.
    /// </summary>
    public void ApplyRole()
    {
        switch (role)
        {
            case NPCRole.Lumberjack:
                if (lumberjackAgent != null)
                    lumberjackAgent.ConfigureLumberjack(desiredAmount);
                else
                    Debug.LogError("LumberjackAgent no encontrado en el NPC.");
                break;

            case NPCRole.Miner:
                Debug.LogWarning("MinerAgent no implementado aun.");
                // if (minerAgent != null)
                //     minerAgent.ConfigureMiner(desiredAmount);
                break;
        }
    }
}
