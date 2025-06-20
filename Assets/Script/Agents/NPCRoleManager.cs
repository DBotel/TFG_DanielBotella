using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class NPCRoleManager : MonoBehaviour
{
    public TownHall townHall;
    List<NPCBasicNeeds> allNPCSNeeds;

    
    public TextMeshProUGUI statsText;
    private void Awake()
    {
        allNPCSNeeds = new List<NPCBasicNeeds>();
        if (townHall == null)
        {
            townHall = GetComponent<TownHall>();
            if (townHall == null)
            {
                Debug.LogError("TownHall component not found on NPCRoleManager.");
            }
        }
    }

    private void Start()
    {
        UpdateTextStats();
    }

    public void ReassignRoles()
    {
            var world           = GWorld.Instance.GetWorld();
            int food             = world.GetState("food");
            int wood             = world.GetState("town_resources_Wood");
            int stone            = world.GetState("town_resources_Stone");
            int pendingBuildings = world.GetState("pending_buildings");

           // List<GAgent> agents = townHall.GetNPC();
           List<GAgent> agents = FindObjectsOfType<GAgent>().ToList();
            int N = agents.Count;
            allNPCSNeeds.Clear();
            foreach (var a in agents)
                if (a.TryGetComponent(out NPCBasicNeeds n))
                    allNPCSNeeds.Add(n);
            if (N == 0) return;

            float hungerTotal = allNPCSNeeds.Sum(n => n.hunger);
            float sleepTotal  = allNPCSNeeds.Sum(n => n.sleep);
            float happyTotal  = allNPCSNeeds.Sum(n => n.happiness);

            float maxStat       = 100f * N;
            float uHunger       = 1f - (hungerTotal / maxStat);        // Hunter
            float uBuilder      = Mathf.Clamp01(pendingBuildings / (float)N);
            float woodUrgency   = wood  < 50 ? 1f : Mathf.Clamp01((100f - wood)  / 100f);
            float stoneUrgency  = stone < 30 ? 1f : Mathf.Clamp01((80f  - stone) / 80f);

            float wHunter      = uHunger      * 1.5f;
            float wBuilder     = uBuilder     * 1.2f;
            float wLumberjack  = woodUrgency  * 1.3f;
            float wMiner       = stoneUrgency * 1.2f;
            float wGuard       = 0.1f;  // siempre algunos guardianes

            float sumW = wHunter + wBuilder + wLumberjack + wMiner + wGuard;
            if (sumW < 1e-4f)
            {
                wHunter = wBuilder = wLumberjack = wMiner = wGuard = 1f;
                sumW = 5f;
            }
            float pHunter     = wHunter     / sumW;
            float pBuilder    = wBuilder    / sumW;
            float pLumberjack = wLumberjack / sumW;
            float pMiner      = wMiner      / sumW;
            float pGuard      = wGuard      / sumW;

            int tHunter      = Mathf.Max(1, Mathf.RoundToInt(N * pHunter));
            int tBuilder     = Mathf.Max(1, Mathf.RoundToInt(N * pBuilder));
            int tLumberjack  = Mathf.Max(1, Mathf.RoundToInt(N * pLumberjack));
            int tMiner       = Mathf.Max(1, Mathf.RoundToInt(N * pMiner));

            int assigned     = tHunter + tBuilder + tLumberjack + tMiner;
            if (assigned > N) {
                float scale = N / (float)assigned;
                tHunter      = Mathf.FloorToInt(tHunter * scale);
                tBuilder     = Mathf.FloorToInt(tBuilder * scale);
                tLumberjack  = Mathf.FloorToInt(tLumberjack * scale);
                tMiner       = Mathf.FloorToInt(tMiner * scale);
                assigned     = tHunter + tBuilder + tLumberjack + tMiner;
            }
            int tGuard = Mathf.Max(0, N - assigned);

            var rnd      = new System.Random();
            var shuffled = agents.OrderBy(_ => rnd.Next()).ToList();
            int idx = 0;
            for (int i = 0; i < tHunter      && idx < N; i++, idx++)
                shuffled[idx].GetComponent<NPCRoleAssigner>()?.SetupRole(NPCRole.Hunter, 5);
            for (int i = 0; i < tBuilder     && idx < N; i++, idx++)
                shuffled[idx].GetComponent<NPCRoleAssigner>()?.SetupRole(NPCRole.Builder, 1);
            for (int i = 0; i < tLumberjack && idx < N; i++, idx++)
                shuffled[idx].GetComponent<NPCRoleAssigner>()?.SetupRole(NPCRole.Lumberjack, 5);
            for (int i = 0; i < tMiner       && idx < N; i++, idx++)
                shuffled[idx].GetComponent<NPCRoleAssigner>()?.SetupRole(NPCRole.Miner, 5);
            for (; idx < N; idx++)
                shuffled[idx].GetComponent<NPCRoleAssigner>()?.SetupRole(NPCRole.Guard, 1);

            Debug.Log($"Roles → Hunter:{tHunter} Builder:{tBuilder} Lumberjack:{tLumberjack} Miner:{tMiner} Guard:{tGuard}");
            UpdateTextStats();
        }
    
    public void UpdateTextStats()
    {
        if (statsText == null) return;
        
        var agents = FindObjectsOfType<GAgent>().ToList();
        
        var roles = System.Enum.GetValues(typeof(NPCRole)).Cast<NPCRole>();
        string result = "";
        foreach (var role in roles)
        {
            int count = agents.Count(a => a.GetComponent<NPCRoleAssigner>()?.role == role);
            float percent = (count * 100f) / agents.Count;
            result += $"- {role}: {percent:0.#}%\n";
        }
        statsText.text = result;
    }
}
        
        
    
        

