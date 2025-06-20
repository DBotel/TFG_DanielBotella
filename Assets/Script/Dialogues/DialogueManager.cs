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

    public void StartDialogue(NPCProfile profile,NPCRole role)
    {
        currentNpc = new NPCData {
            id        = profile.npcId,
            backstory = profile.backstory,
            maxHistory= profile.maxHistory,
            modelName = profile.modelName,
            currentRole = role
        };
        LoadNpcHistory(currentNpc);

        _ = SendGreeting();
    }

    private async Task SendGreeting()
    {
        string greeting = await AIChatService.Instance.SendInitialResponseAsync(currentNpc);

        DialogueUIManager.Instance.ShowNPCResponse(greeting);

        SaveNpcHistory(currentNpc);
    }


    public void OnPlayerSpeaks(string playerLine)
    {
        _ = ContinueDialogue(playerLine);
    }

    private async Task ContinueDialogue(string playerLine)
    {
        string reply = await AIChatService.Instance
                              .SendChatCompletionAsync(currentNpc, playerLine);

        DialogueUIManager.Instance.ShowNPCResponse(reply);

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
            var wrapper = JsonUtility.FromJson<HistoryWrapper>(
                File.ReadAllText(path)
            );
            npc.history = wrapper.history;
        }
    }

    [System.Serializable]
    private class HistoryWrapper
    {
        public List<ChatMessage> history;
    }
}
