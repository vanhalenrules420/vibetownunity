using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }

    private NPCInteraction activeNPC = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanSendText(NPCInteraction npc)
    {
        return activeNPC == npc; // Hanya NPC yang sedang aktif yang boleh menerima input
    }

    public void SetActiveNPC(NPCInteraction npc)
    {
        activeNPC = npc;
    }

    public void ClearActiveNPC(NPCInteraction npc)
    {
        if (activeNPC == npc)
        {
            activeNPC = null;
        }
    }
}
