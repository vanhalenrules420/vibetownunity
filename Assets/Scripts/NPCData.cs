using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "NPC/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName = "Default NPC";
    [TextArea(5, 10)] public string characterPrompt = "Describe the NPC personality here.";
}
