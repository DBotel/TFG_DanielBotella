using UnityEngine;
public enum JobType
{
    Lumberjack,  // Leñador
    Miner,       // Minero
    Farmer,      // Agricultor
    Builder,     // Constructor
    Healer       // Sanador
}


public class NPC : GAgent
{
    public JobType job;

    [ContextMenu("AsignJob")]
    public void AsignJob()
    {
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
        goals.Add(chopWoodGoal, 1);  // Asignamos la meta con prioridad
    }

    void AddMinerTasks()
    {
        // Agregar tareas de minar
        SubGoal mineGoal = new SubGoal("Mine", 1, false);
        goals.Add(mineGoal, 1);  // Asignamos la meta con prioridad
    }

    void AddFarmerTasks()
    {
        // Agregar tareas de sembrar y cosechar
        SubGoal farmGoal = new SubGoal("Farm", 1, false);
       goals.Add(farmGoal, 1);
    }

    void AddBuilderTasks()
    {
        // Agregar tareas de construcción
        SubGoal buildGoal = new SubGoal("Build", 1, false);
        goals.Add(buildGoal, 1);
    }

    void AddHealerTasks()
    {
        // Agregar tareas de curación
        SubGoal healGoal = new SubGoal("Heal", 1, false);
        goals.Add(healGoal, 1);
    }
}
