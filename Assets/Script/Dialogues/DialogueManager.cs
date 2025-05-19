// Assets/Scripts/Dialogue/DialogueManager.cs
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;  // si lo necesitas para UI
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private NPCData currentNpc;

    void Awake()
    {
        if (Instance == null)      Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    /// <summary>
    /// Inicializa la conversación partiendo del ScriptableObject.
    /// </summary>
    /// <summary>
    /// Lanza el diálogo con este perfil y genera saludo automático.
    /// </summary>
    public void StartDialogue(NPCProfile profile)
    {
        // 1) Inicializa datos e historial
        currentNpc = new NPCData {
            id        = profile.npcId,
            backstory = profile.backstory,
            maxHistory= profile.maxHistory,
            modelName = profile.modelName
        };
        LoadNpcHistory(currentNpc);

        // 2) Inicia la corrutina asíncrona para el saludo
        _ = SendGreeting();
    }

    /// <summary>
    /// Coroutine de saludo: pide a la IA y muestra el texto.
    /// </summary>
    private async Task SendGreeting()
    {
        // 1) Genera saludo inicial
        string greeting = await AIChatService.Instance.SendInitialResponseAsync(currentNpc);

        // 2) Muestra en UI
        DialogueUIManager.Instance.ShowNPCResponse(greeting);

        // 3) Guarda historial actualizado
        SaveNpcHistory(currentNpc);
    }


    /// <summary>
    /// Se llama al pulsar “Enviar”: arranca la petición a la IA.
    /// </summary>
    public void OnPlayerSpeaks(string playerLine)
    {
        // Lanzamos la llamada asíncrona
        _ = ContinueDialogue(playerLine);
    }

    private async Task ContinueDialogue(string playerLine)
    {
        // Pide la IA
        string reply = await AIChatService.Instance
                              .SendChatCompletionAsync(currentNpc, playerLine);

        // Muestra por la UI
        DialogueUIManager.Instance.ShowNPCResponse(reply);

        // Guarda el historial
        SaveNpcHistory(currentNpc);
    }

    private void SaveNpcHistory(NPCData npc)
    {
        string path = Path.Combine(
            Application.persistentDataPath,
            $"npc_{npc.id}_history.json"
        );
        File.WriteAllText(path, JsonUtility.ToJson(npc.history));
    }

    private void LoadNpcHistory(NPCData npc)
    {
        string path = Path.Combine(
            Application.persistentDataPath,
            $"npc_{npc.id}_history.json"
        );
        if (File.Exists(path))
        {
            // JsonUtility no serializa listas planas: envolvemos en un wrapper
            var wrapper = JsonUtility.FromJson<HistoryWrapper>(
                File.ReadAllText(path)
            );
            npc.history = wrapper.history;
        }
    }

    // Helper para JsonUtility + List<T>
    [System.Serializable]
    private class HistoryWrapper
    {
        public List<ChatMessage> history;
    }
}
