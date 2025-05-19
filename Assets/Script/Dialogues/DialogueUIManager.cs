
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIManager : MonoBehaviour
{
    public static DialogueUIManager Instance;
    

    [Tooltip("Dónde escribe el jugador")]
    public TMP_InputField playerInput;

    [Tooltip("Botón para enviar")]
    public Button sendButton;

    [Tooltip("Texto donde aparece la respuesta del NPC")]
    public TMP_Text npcResponseText;

    void Awake()
    {
        if (Instance == null)      Instance = this;
        else if (Instance != this) Destroy(gameObject);
        
    }


    public void ShowDialogue(NPCRoleAssigner assigner)
    {
        DialogueManager.Instance.StartDialogue(assigner.profile);
        
        playerInput.text    = "";
        npcResponseText.text = "";
        
        sendButton.onClick.RemoveAllListeners();
        sendButton.onClick.AddListener(OnSend);
    }

    void OnSend()
    {
        var text = playerInput.text.Trim();
        if (string.IsNullOrEmpty(text)) return;
        
        DialogueManager.Instance.OnPlayerSpeaks(text);
        
        playerInput.text = "";
    }
    
    public void ShowNPCResponse(string response)
    {
        npcResponseText.text = response;
    }
    
}