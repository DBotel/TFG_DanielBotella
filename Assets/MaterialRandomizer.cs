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
            
            var renderer = obj.GetComponent<SkinnedMeshRenderer>();
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
        npcMaterial.SetColor("_SkinColor", Random.ColorHSV());
        npcMaterial.SetFloat("_SkinSmoothness", Random.Range(0f, 1f));
        npcMaterial.SetColor("_EyesColor", Random.ColorHSV());
        npcMaterial.SetFloat("_EyesSmoothness", Random.Range(0f, 1f));
        npcMaterial.SetColor("_HairColor", Random.ColorHSV());
        npcMaterial.SetFloat("_HairSmoothness", Random.Range(0f, 1f));
        npcMaterial.SetColor("_ScleraColor", Random.ColorHSV());
        npcMaterial.SetFloat("_ScleraSmoothness", Random.Range(0f, 1f));
        npcMaterial.SetColor("_LipsColor", Random.ColorHSV());
        npcMaterial.SetFloat("_LipsSmoothness", Random.Range(0f, 1f));
        npcMaterial.SetColor("_OtherColor", Random.ColorHSV());
        npcMaterial.SetFloat("_OtherSmoothness", Random.Range(0f, 1f));

        for (int i = 1; i <= 4; i++)
        {
            npcMaterial.SetColor($"_Metal{i}Color", Random.ColorHSV());
            npcMaterial.SetFloat($"_Metal{i}Metallic", Random.Range(0f, 1f));
            npcMaterial.SetFloat($"_Metal{i}Smoothness", Random.Range(0f, 1f));
        }

        for (int i = 1; i <= 3; i++)
        {
            npcMaterial.SetColor($"_Leather{i}Color", Random.ColorHSV());
            npcMaterial.SetFloat($"_Leather{i}Smoothness", Random.Range(0f, 1f));
        }
    }
}
