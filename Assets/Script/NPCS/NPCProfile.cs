// Assets/Scripts/Dialogue/NPCProfile.cs
using UnityEngine;

[CreateAssetMenu(
    fileName = "New NPC Profile", 
    menuName = "NPC/NPC Profile", 
    order = 1
)]
public class NPCProfile : ScriptableObject
{
    [Tooltip("Identificador único, usado para guardar la historia")]
    public string npcId;

    [TextArea(3, 8), Tooltip("Historia y personalidad del NPC")]
    public string backstory;

    [Tooltip("Máximo de mensajes guardados en contexto")]
    public int maxHistory = 20;

    [Tooltip("Nombre del modelo a usar (ej: llama2, llama2:7b...)")]
    public string modelName = "llama2";
    
    [Tooltip("Máximo de caracteres en la respuesta del NPC")]
    public int maxResponseLength = 200;
}