using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class NPCRoleUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject npcPanel;
    public TMP_Dropdown roleDropdown;
    public TMP_InputField amountInput;
    public Button farmLimitedButton;     
    public Button farmInfiniteButton;    
    public Button cancelButton;
    public GameObject[] otherPanels;

    private NPCRoleAssigner selectedAssigner;
    private NPCRole lastSelectedRole;
    

    [Header("NPC Picking")]
    [Tooltip("Layer de los NPCs")]
    public LayerMask npcLayerMask;
    [Tooltip("Radio para detectar NPCs alrededor del punto clicado")]
    public float pickRadius = 0.5f;
    void Start()
    {
        npcPanel.SetActive(false);

        farmLimitedButton.onClick.AddListener(OnFarmLimited);
        farmInfiniteButton.onClick.AddListener(OnFarmInfinite);
        cancelButton.onClick.AddListener(HidePanel);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) )
        {
            Vector3 worldPos = Mouse3D.GetMouseWorldPosition();
            
            Collider[] hits = Physics.OverlapSphere(worldPos, pickRadius, npcLayerMask);
            if (hits.Length > 0)
            {
                var assigner = hits[0].GetComponent<NPCRoleAssigner>();
                if (assigner != null)
                {
                    HideOtherPanels();
                    ShowPanel(assigner);
                    return;
                }
            }
            
            //HidePanel();
        }
    }
    /*
    bool SameRole()
    {
        
    }*/
    void HideOtherPanels()
    {
        foreach (var panel in otherPanels)
        {
            if (panel != npcPanel)
                panel.SetActive(false);
        }
    }
    bool IsPointerOverUI()
        => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    void ShowPanel(NPCRoleAssigner assigner)
    {

        selectedAssigner = assigner;
        lastSelectedRole=assigner.role;
        npcPanel.SetActive(true);
        
        
        roleDropdown.value = (int)assigner.role;
        amountInput.text   = assigner.desiredAmount.ToString();
        
        
    }

    void HidePanel()
    {
        npcPanel.SetActive(false);
        selectedAssigner = null;
        otherPanels[0].SetActive(true);
    }

    void OnFarmLimited()
    {
        Debug.LogError("OnFarmLimited");
        if (selectedAssigner == null) return;
        Debug.LogError("OnFarmLimitedCheck");

        selectedAssigner.role = (NPCRole)roleDropdown.value;
        if (int.TryParse(amountInput.text, out int amt))
            selectedAssigner.desiredAmount = Mathf.Max(1, amt);

        ConfigureAndResetAgent();
    }

    void OnFarmInfinite()
    {
        if (selectedAssigner == null) return;
        
        selectedAssigner.role = (NPCRole)roleDropdown.value;
        selectedAssigner.desiredAmount = int.MaxValue;

        ConfigureAndResetAgent();
    }

    void ConfigureAndResetAgent()
    {
        foreach (var action in selectedAssigner.GetComponents<GAction>())
            action.SetupAction();
       selectedAssigner.ApplyRole();
       
        HidePanel();
        
    }
}