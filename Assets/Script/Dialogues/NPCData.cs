using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Net.Http;           
using System.Threading.Tasks;      
[Serializable]
public class NPCData 
{
    public string id;
    public string backstory;
    public int    maxHistory;
    public string modelName;
    public int maxResponseLength = 200;
    public List<ChatMessage> history = new List<ChatMessage>();
    public GameObject npc;
    public NPCRole currentRole;
    
}