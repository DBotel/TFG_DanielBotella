using Mono.Cecil;
using System.Linq;
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

    [Header("Misión de recolección")]
    public TownResourcesTypes missionResourceType;
    public string missionResourceTag;
    public bool needTool;          // ¿Necesita herramienta?
    public bool hasTool;          // ¿Tiene la herramienta?
    public string toolTag;           // p.ej. "Axe"
    public int missionTarget;     // 0 = infinito
    
    private SubGoal missionSubGoal;
    private int missionProgress;
    private string missionRecipeID;
    protected override void Start()
    {
        //NewAsignJob();
        base.Start();
    }
    /// <summary>
    /// Asigna una misión de recolectar:
    /// - resourceTag: Tag de los objetos en escena ("Tree", "Ore"...)
    /// - resourceType: enum WOOD, STONE, etc. (para FarmResources y TownHall)
    /// - targetAmount: cuántas unidades quieres (0 = infinito)
    /// </summary>
    public void AssignGatherMission()
    {
        // 1) Limpiar viejas misiones
        goals.Clear();
        

        // 2) Generar el SubGoal
        string goalKey = missionResourceType.ToString();
        missionSubGoal = new SubGoal(goalKey, 1, false);
        goals.Add(missionSubGoal, 1);

        // 3) ELIMINAR acciones viejas de este tipo (por si re‐asignas misión)
        actions.RemoveAll(a => a is GetItem || a is FarmingResources);

        // 4) Si necesita herramienta, la pico primero
        if (needTool)
        {
            var gi = gameObject.AddComponent<GetItem>();
            gi.itemTag = toolTag;
           // gi.duration = Random.Range(0.5f, 1.3f);
            // Muy importante: añadir a la lista de acciones
            actions.Add(gi);
        }

        // 5) Acción de farmear recurso
        var fr = gameObject.AddComponent<FarmingResources>();
        fr.resourceTag = missionResourceTag;
        fr.resourceType = missionResourceType;
       // fr.duration = Random.Range(1, 3); // Setear Duracion de talar 
        // Si necesita herramienta, pone precondición HasTool
        if (needTool)
            fr.preconditions.Add("Has" + toolTag, 1);
        // Añádela también a la lista
        actions.Add(fr);
    }


    /// <summary>
    /// Llamado desde FarmingResources.PostPerform()
    /// para contar cuántas unidades llevamos y parar si es necesario.
    /// </summary>
    public void OnResourceGathered(TownResourcesTypes resourceType , int amountAdded)
    {
        if (resourceType != missionResourceType) return;
        missionProgress+=amountAdded;
        Debug.Log($"{name}: recolectadas {missionProgress}/{(missionTarget == 0 ? "∞" : missionTarget.ToString())} de {resourceType}");

        // Si hay un objetivo finito y lo alcanzamos, cancelamos
        if (missionTarget > 0 && missionProgress >= missionTarget)
        {
            goals.Remove(missionSubGoal);
            Debug.Log($"{name}: ¡Misión completada!");
        }
    }
    public bool HasCompletedMission()
    {
        return missionTarget > 0 && missionProgress >= missionTarget;
    }

    public void ReassignCurrentMission()
    {
        Debug.Log($"{name}: Reasignando misión porque aún no está completa.");
        needTool = false;
        AssignGatherMission();
    }

    public void DropItem(string itemString)
    {
        GameObject itemToDrop = FindItemTagGOInInventory(itemString);
        if (itemToDrop == null) return;
        goals.Clear();
        SubGoal dropItemSg = new SubGoal("droppingItem", 1, false);
        goals.Add(dropItemSg,1);

        DropItem d = gameObject.AddComponent<DropItem>();
        d.item = itemToDrop;
        d.duration = Random.Range(0.5f, 1.3f);
        actions.Add(d);
    }

    GameObject FindItemTagGOInInventory(string tag)
    {
        print("Buscando item con el tag : " + tag);
        foreach (var item in inventory.items)
        {
            if(item.CompareTag(tag)) print("item encontrado"); return item; 
        }
        return null;
    }

    /// <summary>
    /// Asigna misión de fabricar
    /// </summary>
    public void AssignCraftMission(string recipeID, int targetAmount)
    {
        // Limpiar
       // goals.Clear();
        actions.RemoveAll(a => a is GAction);

        missionRecipeID = recipeID;
        missionProgress = 0;
        missionTarget = targetAmount;

        // SubGoal: clave = recipeID+"Crafted"
        missionSubGoal = new SubGoal(recipeID + "Crafted", 1, false);
        goals.Add(missionSubGoal, 1);

        // Acción dinámica
        var craft = gameObject.AddComponent<CraftItem>();
        craft.recipeID = recipeID;
        actions.Add(craft);

        // Forzar replan
        ResetPlan();
    }

    /// <summary>
    /// Llamado por CraftItem.PostPerform()
    /// </summary>
    public void OnItemCrafted(string recipeID)
    {
        if (recipeID + "Crafted" != missionSubGoal.sGoals.Keys.First()) return;
        missionProgress++;
        Debug.Log($"{name}: fabricadas {missionProgress}/{missionTarget} de {recipeID}");

        if (missionTarget > 0 && missionProgress >= missionTarget)
        {
            goals.Remove(missionSubGoal);
            Debug.Log($"{name}: misión de crafting completada");
        }
    }

    private bool wasPlanning = false;

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (currentAction == null && !wasPlanning && actions.Count<1)
        {
           // ResetPlan();
            // Añadir la acción de deambular si no hay plan
            Debug.Log($"{gameObject.name} no tiene plan, añadiendo Wander...");
            Wander wander = gameObject.AddComponent<Wander>();
           // wander.duration = Random.Range(3f, 6f); 
           

            // Crear subobjetivo temporal
            SubGoal wanderGoal = new SubGoal("wander", 1, false);
            goals.Add(wanderGoal,1);
            actions.Add(wander);

            wasPlanning = true; // para evitar que lo añada múltiples veces
        }
        else if (currentAction != null)
        {
            wasPlanning = false;
        }
    }
    [ContextMenu("Job: 1 Hacha")]
    public void Job_Craft5Arrows()
    {
        AssignCraftMission("Axe", 1);
    }
    [ContextMenu("DestroyAllActions")]
    public void DestroyActions()
    {
        actions.Clear();
        goals.Clear();

    }

    [ContextMenu("Misión: 400 Madera con Hacha")]
    public void TestJob1()
    {
       
        missionProgress = 0;
        missionResourceType = TownResourcesTypes.WOOD;
        missionResourceTag = "Tree";
        needTool = true;
        toolTag = "Axe";
        missionTarget = 400;
        AssignGatherMission();
    }

    [ContextMenu("Misión: Madera Infinito sin Hacha")]
    public void TestJob2()
    {
        goals.Clear();
        missionProgress = 0;
        missionResourceType = TownResourcesTypes.WOOD;
        missionResourceTag = "Tree";
        needTool = false;
        missionTarget = 0;
        AssignGatherMission();
    }

    public void AsignJob()
    {
        AssignJobTasks();
    }

    public void NewAsignJob()
    {
        base.Start();
        SubGoal s1 = new SubGoal("choppedTree", 3, false);
        goals.Add(s1, 3);
    }
    void AssignJobTasks()
    {
        base.Start();
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
        SubGoal chopWoodGoal = new SubGoal("choppedTree", 1, true);
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
