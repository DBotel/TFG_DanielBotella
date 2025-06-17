using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
public enum NPCRole
{
    Hunter,
    Builder,
    Guard,
    Miner,        
    Lumberjack,   
}

[RequireComponent(typeof(GAgent))]
public class NPCRoleAssigner : MonoBehaviour
{
    
    public NPCRole role;
    public int desiredAmount = 5;
    public NPCProfile profile;


    private GAgent agent;
    private LumberjackAgent lumberjackAgent;
    private StoneMiner miner;
    private HunterAgent hunterAgent;
    private GuardAgent guardAgent;
    
    [SerializeField] private TownHall townHall;
    void Awake()
    {
        agent = GetComponent<GAgent>();
        lumberjackAgent = GetComponent<LumberjackAgent>();
        miner = GetComponent<StoneMiner>();
        hunterAgent = GetComponent<HunterAgent>();
        guardAgent = GetComponent<GuardAgent>();
        // minerAgent = GetComponent<MinerAgent>();
        
        townHall = FindObjectOfType<TownHall>();
    }

    
    void Start()
    {
        return;
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            GActionWander wanderAction = GetComponent<GActionWander>();
            wanderAction.PostPerform();
            ChangeRole();

        }
        StartCoroutine(Wait());
    }
    
    public void SetupRole(NPCRole newRole, int amount )
    {
        role = newRole;
        desiredAmount = amount;
        ChangeRole();
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
                agent.beliefs.states["hasTool_Axe"] = 0;
                agent.beliefs.states["collected_WOOD"] = 0;
                agent.beliefs.states["returnedTool_Axe"] = 0;

                lumberjackAgent.ConfigureLumberjack(desiredAmount);
                var takeL = GetComponent<GActionTakeTool>();
                takeL.collectStateKey = "collected_WOOD";
                break;

            case NPCRole.Miner:
                agent.beliefs.states["hasTool_Pickaxe"] = 0;
                agent.beliefs.states["collected_STONE"] = 0;
                agent.beliefs.states["returnedTool_Pickaxe"] = 0;

                miner.ConfigureStoneMiner(desiredAmount);
                var takeM = GetComponent<GActionTakeTool>();
                takeM.collectStateKey = "collected_STONE";
                break;
            
            case NPCRole.Hunter:
                agent.beliefs.states["hasTool_Weapon"] = 0;
                agent.beliefs.states["collected_MEAT"] = 0;
                agent.beliefs.states["returnedTool_Weapon"] = 0;
                
                hunterAgent.ConfigureHunter(desiredAmount);
                var takeH = GetComponent<GActionTakeTool>();
                takeH.collectStateKey = "collected_MEAT";
                break;
            case NPCRole.Guard:
                agent.beliefs.states["hasTool_Shield"] = 0;
                agent.beliefs.states["collected_DEFEND"] = 0;
                agent.beliefs.states["returnedTool_Shield"] = 0;
                
                guardAgent.ConfigureGuard();
                var takeS = GetComponent<GActionTakeTool>();
                takeS.collectStateKey = "collected_DEFEND";
                break;
                
        }
    }
    
    
}
