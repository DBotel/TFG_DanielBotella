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
            modelName = "llama2:7b",
            history   = new List<ChatMessage>()
        };

        await SendChatCompletionAsync(warmNpc, "hola");
        Debug.Log("[IA] Warm-up completado");
    }
    public async Task<string> SendInitialResponseAsync(NPCData npc)
    {
        // 1) Recorta historial como siempre
        TrimHistory(npc);
        
        var msgList = new List<MessageData>
        {
            new MessageData {
                role = "system",
                content =
                    // contexto general
                    "Estás en un videojuego ambientado en la Edad Media de habla hispana. " +
                    // descripción de quién eres
                    npc.backstory +
                    // instrucciones de estilo
                    "Sólo hablas en español, con vocabulario y tono medieval, " +
                    // límite de caracteres
                    $"y tus respuestas no exceden {npc.maxResponseLength} caracteres."
            }
        };
        
        msgList.Add(new MessageData {
            role = "system",
            content = $"Porfavor , limita mucho tu respuesta , debe de ser un saludo , 100 caracteres máximo."
        });
        foreach (var h in npc.history)
            msgList.Add(new MessageData { role = h.role, content = h.content });


        // 3) Creamos la petición con stream=true para que salga todo de golpe
        var request = new ChatCompletionRequest
        {
            model    = npc.modelName,
            messages = msgList,
            stream   = true
        };
        string jsonPayload = JsonUtility.ToJson(request);
        Debug.Log($"[IA] Enviando saludo inicial: {jsonPayload}");

        // 4) Enviamos y leemos por streaming como antes
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
            catch { /* ignora fragmentos malformados */ }
        }

        // 5) Guarda en historial y devuelve
        npc.history.Add(new ChatMessage { role = "assistant", content = fullReply });
        TrimHistory(npc);
        return fullReply;
    }
    
      public async Task<string> SendChatCompletionAsync(NPCData npc, string playerInput)
    {
        TrimHistory(npc);

        var msgList = new List<MessageData>
        {
            new MessageData {
                role = "system",
                content =
                    // contexto general
                    "Estás en un videojuego ambientado en la Edad Media de habla hispana. " +
                    // descripción de quién eres
                    npc.backstory  +
                    // instrucciones de estilo
                    "Sólo hablas en español, con vocabulario y tono medieval, " +
                    // límite de caracteres
                    $"y tus respuestas no exceden {npc.maxResponseLength} caracteres."
            }
        };
        foreach (var h in npc.history)
            msgList.Add(new MessageData { role = h.role, content = h.content });
        msgList.Add(new MessageData { role = "user", content = playerInput });

        // 2) Crea la petición
        var request = new ChatCompletionRequest
        {
            model    = npc.modelName,
            messages = msgList,
            // stream por defecto es true en Ollama, pero puedes forzarlo:
            // stream = true 
        };
        string jsonPayload = JsonUtility.ToJson(request);
        Debug.LogError($"[IA] Enviando payload: {jsonPayload}");

        // 3) Envía y lee cabeceras primero (streaming)
        var httpReq = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
        };
        var httpResp = await _http.SendAsync(
            httpReq,
            HttpCompletionOption.ResponseHeadersRead
        );

        // 4) Lee el stream línea a línea
        var reader = new System.IO.StreamReader(
            await httpResp.Content.ReadAsStreamAsync()
        );

        string fullReply = "";
        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) 
                continue;

            // Cada línea es un JSON fragmento: {"message":{...},"done":false}
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

        // 5) Actualiza historial
        npc.history.Add(new ChatMessage { role = "user",      content = playerInput });
        npc.history.Add(new ChatMessage { role = "assistant", content = fullReply });
        TrimHistory(npc);

        return fullReply;
    }

    private string ExtractAssistantContent(string json) {
        // Deserializa el JSON en tu objeto
        var response = JsonUtility.FromJson<ChatCompletionResponse>(json);

        // Comprueba que haya al menos una elección
        if (response.choices != null && response.choices.Length > 0 
                                     && response.choices[0].message != null) {
            return response.choices[0].message.content;
        }

        // Fallback si algo falla
        Debug.LogError("ChatCompletionResponse sin contenido esperado:\n" + json);
        return string.Empty;
        
        // Si esto sigue fallando, imprime un snippet reducido
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
