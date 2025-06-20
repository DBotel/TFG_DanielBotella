using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnNPC : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public NPCRoleManager npcRoleManager;
    TownHall townHall;
    public NPCProfile[] profiles;
    private void Start()
    {
        townHall = FindObjectOfType<TownHall>();
        if (townHall == null)
        {
            Debug.LogError("TownHall component not found in the scene.");
            return;
        }
        
        if (npcRoleManager == null)
        {
            npcRoleManager = FindObjectOfType<NPCRoleManager>();
            if (npcRoleManager == null)
            {
                Debug.LogError("NPCRoleManager component not found in the scene.");
                return;
            }
        }
    }

    public void SpawnNPCC()
    {
      GameObject npc  = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
      int randomRole = Random.Range(0, System.Enum.GetValues(typeof(NPCRole)).Length);
       NPCRole role = (NPCRole)randomRole;
       NPCRoleAssigner npcRoleAssigner = npc.GetComponent<NPCRoleAssigner>();
       npcRoleAssigner.role = role;
       npcRoleAssigner.desiredAmount = Random.Range(1,6);
       npcRoleAssigner.profile = profiles[Random.Range(0, profiles.Length)];
     
      npcRoleManager.UpdateTextStats();
      townHall.AddNPC(npc.GetComponent<GAgent>());
        
    }
    
}
