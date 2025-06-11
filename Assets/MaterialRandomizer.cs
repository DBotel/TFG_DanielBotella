using UnityEngine;

public class MaterialRandomizer : MonoBehaviour
{
    public Material baseMaterial;
    public GameObject[] objectsToRandomize; 

    void Start()
    {
        Material newMat = Instantiate(baseMaterial);
        RandomizeMaterial(newMat);
        foreach (var obj in objectsToRandomize)
        {
            
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMat;
            }
        }
        
    }

    void RandomizeMaterial(Material npcMaterial)
    {
        if (npcMaterial == null) return;

        // Randomiza los mismos parámetros que antes:
        npcMaterial.SetColor("_SKINCOLOR", RandomColor());
        npcMaterial.SetFloat("_SKINSMOOTHNESS", Random.Range(0.5f, 1f));
        npcMaterial.SetColor("_EYESCOLOR", RandomColor());
        npcMaterial.SetFloat("_EYESSMOOTHNESS", Random.Range(0.5f, 1f));
        npcMaterial.SetColor("_HAIRCOLOR", RandomColor());
        npcMaterial.SetFloat("_HAIRSMOOTHNESS", Random.Range(0.5f, 1f));
        npcMaterial.SetColor("_SCLERACOLOR", RandomColor());
        npcMaterial.SetFloat("_SCLERASMOOTHNESS", Random.Range(0.5f, 1f));
        npcMaterial.SetColor("_LIPSCOLOR", RandomColor());
        npcMaterial.SetFloat("_LIPSSMOOTHNESS", Random.Range(0.5f, 1f));
        npcMaterial.SetColor("_OTHERCOLOR", RandomColor());
        npcMaterial.SetFloat("_OTHERSMOOTHNESS", Random.Range(0f, 1f));

        for (int i = 1; i <= 4; i++)
        {
            npcMaterial.SetColor($"_Metal{i}Color", RandomColor());
            npcMaterial.SetFloat($"_Metal{i}Metallic", Random.Range(0f, 1f));
            npcMaterial.SetFloat($"_Metal{i}Smoothness", Random.Range(0f, 1f));
        }

        for (int i = 1; i <= 3; i++)
        {
            npcMaterial.SetColor($"_Leather{i}Color", RandomColor());
            npcMaterial.SetFloat($"_Leather{i}Smoothness", Random.Range(0f, 1f));
        }
    }
    Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}
    
    
