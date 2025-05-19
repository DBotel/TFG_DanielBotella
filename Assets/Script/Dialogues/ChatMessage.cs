using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class ChatMessage {
    public string role;    // "system", "user" o "assistant"
    public string content;
}

[Serializable]
public class ResponseMessage {
    public string role;
    public string content;
}

[Serializable]
public class Choice {
    public int index;
    public ResponseMessage message;
    public string finish_reason;
}

[Serializable]
public class ChatCompletionResponse {
    public string id;
    public string model;
    public Choice[] choices;

}

[Serializable]
public class MessageData
{
    public string role;
    public string content;
}


[Serializable]
public class ChatCompletionRequest {
    public string model;
    public List<MessageData> messages;
    public bool stream = true;
    public int max_tokens = 120;
    public float temperature = 0.3f;
}

[Serializable]
public class StreamingResponse
{
    public ResponseMessage message;
    public bool done;
}