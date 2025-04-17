using UnityEngine;
public enum JobType
{
    Lumberjack,  // Leñador
    Miner,       // Minero
    Farmer,      // Agricultor
    Builder,     // Constructor
    Healer       // Sanador
}


public class NPC : MonoBehaviour
{
    public JobType job;  
    private GAgent agent; 

    void Start()
    {
        agent = GetComponent<GAgent>();
        AssignJobTasks();
    }

    void AssignJobTasks()
    {
        switch (job)
        {
            case JobType.Lumberjack:
                AddLumberjackTasks();
                break;
            case JobType.Miner:
                AddMinerTasks();
                break;
            case JobType.Farmer:
                AddFarmerTasks();
                break;
            case JobType.Builder:
                AddBuilderTasks();
                break;
            case JobType.Healer:
                AddHealerTasks();
                break;
        }
    }


    void AddLumberjackTasks()
    {
        // Agregar tareas de cortar árboles
        SubGoal chopWoodGoal = new SubGoal("ChopWood", 1, false);
        agent.goals.Add(chopWoodGoal, 1);  // Asignamos la meta con prioridad
    }

    void AddMinerTasks()
    {
        // Agregar tareas de minar
        SubGoal mineGoal = new SubGoal("Mine", 1, false);
        agent.goals.Add(mineGoal, 1);  // Asignamos la meta con prioridad
    }

    void AddFarmerTasks()
    {
        // Agregar tareas de sembrar y cosechar
        SubGoal farmGoal = new SubGoal("Farm", 1, false);
        agent.goals.Add(farmGoal, 1);
    }

    void AddBuilderTasks()
    {
        // Agregar tareas de construcción
        SubGoal buildGoal = new SubGoal("Build", 1, false);
        agent.goals.Add(buildGoal, 1);
    }

    void AddHealerTasks()
    {
        // Agregar tareas de curación
        SubGoal healGoal = new SubGoal("Heal", 1, false);
        agent.goals.Add(healGoal, 1);
    }
}
