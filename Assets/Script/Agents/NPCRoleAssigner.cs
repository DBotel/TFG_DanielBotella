using UnityEngine;
public enum NPCRole { Lumberjack, Miner }

[RequireComponent(typeof(GAgent))]
public class NPCRoleAssigner : MonoBehaviour
{
    
    public NPCRole role;
    public int desiredAmount = 5;
    public NPCProfile profile;


    private GAgent agent;
    private LumberjackAgent lumberjackAgent;
    private StoneMiner miner;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        lumberjackAgent = GetComponent<LumberjackAgent>();
        miner = GetComponent<StoneMiner>();
        // minerAgent = GetComponent<MinerAgent>();
    }

    void Start()
    {
        //ApplyRole();
    }
    [ContextMenu("ChangeRole")]
    public void ChangeRole()
    {
        agent.beliefs.states.Clear();
        agent.inventory.items.Clear();
        agent.goals.Clear();
        agent.ResetPlan();

        ApplyRole();

        foreach (var a in agent.GetComponents<GAction>())
            a.SetupAction();
    }
    public void ApplyRole()
    {
        agent.beliefs.states.Clear();
        agent.inventory.items.Clear();
        agent.goals.Clear();
        agent.ResetPlan();

        switch (role)
        {
            case NPCRole.Lumberjack:
                Debug.LogError("Lumberjack");
                agent.beliefs.states["hasTool_Axe"] = 0;
                agent.beliefs.states["collected_WOOD"] = 0;
                agent.beliefs.states["returnedTool_Axe"] = 0;

                lumberjackAgent.ConfigureLumberjack(desiredAmount);
                var takeL = GetComponent<GActionTakeTool>();
                takeL.collectStateKey = "collected_WOOD";
                break;

            case NPCRole.Miner:
                Debug.LogError("Miner");
                agent.beliefs.states["hasTool_Pickaxe"] = 0;
                agent.beliefs.states["collected_STONE"] = 0;
                agent.beliefs.states["returnedTool_Pickaxe"] = 0;

                miner.ConfigureStoneMiner(desiredAmount);
                var takeM = GetComponent<GActionTakeTool>();
                takeM.collectStateKey = "collected_STONE";
                break;
        }
    }
}
