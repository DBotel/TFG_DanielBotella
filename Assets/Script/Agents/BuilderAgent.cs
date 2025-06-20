using UnityEngine;

public class BuilderAgent : MonoBehaviour
{
    private GAgent agent;
    private GActionTakeTool takeTool;
    private GActionBuild build;

    void Awake()
    {
        agent = GetComponent<GAgent>();
        takeTool = GetComponent<GActionTakeTool>();
        build = GetComponent<GActionBuild>();
    }

    public void ConfigureBuilder()
    {
        agent.ResetPlan();
        agent.beliefs.states.Clear();
        agent.goals.Clear();

        
        takeTool.toolTag = "Cemento";
        takeTool.collectStateKey = "building";

        agent.beliefs.SetState("hasTool_Cemento",       0);
        agent.beliefs.SetState("build",  0);
        
        agent.goals.Add(new SubGoal("build", 1,  true), 3);
        
       
    }
}
