using UnityEngine;

public class Building : MonoBehaviour
{
    [Tooltip("Modelo de la casa que se activará cuando esté construida")]
    public GameObject houseModel;

    private Renderer houseRenderer;
    [HideInInspector] 
    public bool isBuilt = false;
    public bool isUnderConstruction = false;

    void Awake()
    {
        houseModel = this.gameObject;
        houseRenderer = houseModel.GetComponent<Renderer>();
        houseRenderer.enabled = false; 
    }

  
    public void Build()
    {
        isBuilt = true;
        houseRenderer.enabled = true;
    }
}
