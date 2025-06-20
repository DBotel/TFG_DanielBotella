using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class AIChatService : MonoBehaviour 
    {
    public static AIChatService Instance;
    private HttpClient _http;
        
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _http = new HttpClient { BaseAddress = new Uri("http://localhost:11434") };
            _http.DefaultRequestHeaders.ExpectContinue = false;
        } else Destroy(gameObject);
    }
    
    async void Start()
    {
        
        var warmNpc = new NPCData {
            backstory = "Warmup",
            modelName = "llama2",
            history   = new List<ChatMessage>()
        };

        await SendChatCompletionAsync(warmNpc, "hola");
        Debug.Log("[IA] Warm-up completado");
    }
    public async Task<string> SendInitialResponseAsync(NPCData npc)
    {
        TrimHistory(npc);
        var systemContent = 
            $"Eres un NPC de un videojuego ambientado en la Edad Media. " +
            $"Tu rol actual es: {npc.currentRole}. " +
            "Sólo hablas en español, con vocabulario medieval, " +
            "y tus respuestas no superan 150 caracteres.";

        var msgList = new List<MessageData>
        {
            new MessageData {
                role    = "system",
                content = systemContent
            }
        };

        // 2) Volcamos el historial anterior…
        foreach (var h in npc.history)
            msgList.Add(new MessageData { role = h.role, content = h.content });

        var request = new ChatCompletionRequest
        {
            model    = npc.modelName,
            messages = msgList
        };
        string jsonPayload = JsonUtility.ToJson(request);
        Debug.Log($"[IA] Enviando saludo inicial: {jsonPayload}");

        var httpReq = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        var httpResp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead);
        var reader  = new System.IO.StreamReader(await httpResp.Content.ReadAsStreamAsync());

        string fullReply = "";
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;
            try
            {
                var chunk = JsonUtility.FromJson<StreamingResponse>(line);
                if (chunk.message?.content != null)
                    fullReply += chunk.message.content;
                if (chunk.done) break;
            }
            catch {  }
        }

        npc.history.Add(new ChatMessage { role = "assistant", content = fullReply });
        TrimHistory(npc);
        return fullReply;
    }
    
      public async Task<string> SendChatCompletionAsync(NPCData npc, string playerInput)
    {
        TrimHistory(npc);
        var systemContent = 
            $"Eres un NPC de un videojuego ambientado en la Edad Media. " +
            $"Tu rol actual es: {npc.currentRole}. " +
            "Sólo hablas en español, con vocabulario medieval, " +
            "y tus respuestas no superan 150 caracteres.";

        var msgList = new List<MessageData>
        {
            new MessageData {
                role    = "system",
                content = systemContent
            }
        };

        // 2) Volcamos el historial anterior…
        foreach (var h in npc.history)
            msgList.Add(new MessageData { role = h.role, content = h.content });

        // 3) Añadimos el mensaje del usuario
        msgList.Add(new MessageData { role = "user", content = playerInput });

        var request = new ChatCompletionRequest
        {
            model    = npc.modelName,
            messages = msgList
        };
        string jsonPayload = JsonUtility.ToJson(request);
        Debug.LogError($"[IA] Enviando payload: {jsonPayload}");

        var httpReq = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        var httpResp = await _http.SendAsync(
            httpReq,
            HttpCompletionOption.ResponseHeadersRead
        );

        var reader = new System.IO.StreamReader(
            await httpResp.Content.ReadAsStreamAsync()
        );

        string fullReply = "";
        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) 
                continue;
            
            try
            {
                var chunk = JsonUtility.FromJson<StreamingResponse>(line);
                if (chunk.message?.content != null)
                {
                    fullReply += chunk.message.content;
                    
                }

                if (chunk.done)
                    break;
            }
            catch (Exception e)
            {
                Debug.LogError($"[IA] Fragmento inválido:\n{line}\n{e}");
            }
        }
        
        npc.history.Add(new ChatMessage { role = "user",      content = playerInput });
        npc.history.Add(new ChatMessage { role = "assistant", content = fullReply });
        TrimHistory(npc);

        return fullReply;
    }

    private string ExtractAssistantContent(string json) {
        var response = JsonUtility.FromJson<ChatCompletionResponse>(json);

        if (response.choices != null && response.choices.Length > 0 
                                     && response.choices[0].message != null) {
            return response.choices[0].message.content;
        }

        Debug.LogError("ChatCompletionResponse sin contenido esperado:\n" + json);
        return string.Empty;
        
#if UNITY_EDITOR
        var snippet = json.Length > 200 ? json.Substring(0,200) + "…" : json;
        Debug.LogWarning($"[IA] formato inesperado:\n{snippet}");
#endif
        return "…";
    }
    

    
    private void TrimHistory(NPCData npc, int maxMessages = 07) {
        if (npc.history.Count > maxMessages) {
            npc.history = npc.history
                .Skip(npc.history.Count - maxMessages)
                .ToList();
        }
    }

}
