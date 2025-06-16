using UnityEngine;

public class SpawnNPC : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform spawnPoint;
    public NPCRoleManager npcRoleManager;
    
    private void SpawnNPCC()
    {
      Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
      int randomRole = Random.Range(0, System.Enum.GetValues(typeof(NPCRole)).Length);
       NPCRole role = (NPCRole)randomRole;
      npcPrefab.GetComponent<NPCRoleAssigner>().SetupRole(role, 65);
      npcRoleManager.UpdateTextStats();
        
    }
    
}
