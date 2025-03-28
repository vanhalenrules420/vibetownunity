using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using LLMUnity;

public class NPCChat : MonoBehaviour
{
    public GameObject chatBoxUI;  // Panel untuk chat box
    public TextMeshProUGUI npcText; // Text NPC
    public TMP_InputField playerInput; // Input dari pemain
    public Button sendButton; // Tombol Kirim
    public float detectionRadius = 2f;
    public LayerMask playerLayer;

    private LLMCharacter llmCharacter;
    private bool isTalking = false;

    void Start()
    {
        llmCharacter = GetComponent<LLMCharacter>();
        chatBoxUI.SetActive(false);
        sendButton.onClick.AddListener(SendMessageToNPC);
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player != null && Input.GetKeyDown(KeyCode.E) && !isTalking)
        {
            StartConversation();
        }
    }

    void StartConversation()
    {
        isTalking = true;
        chatBoxUI.SetActive(true);
        npcText.text = "Hello!";
    }

    void SendMessageToNPC()
    {
        string playerMessage = playerInput.text;
        playerInput.text = "";

        StartCoroutine(GetNPCResponse(playerMessage));
    }

    IEnumerator GetNPCResponse(string playerMessage)
    {
        string npcResponse = "I'm sleepy and can't talk right now!"; // Default response if server is down
        bool serverAvailable = true; // Simulasi server (ubah ke false untuk mengetes fallback message)

        if (serverAvailable)
        {
            yield return StartCoroutine(SendToLLM(playerMessage, response => npcResponse = response));
        }

        npcText.text = npcResponse;
    }

    IEnumerator SendToLLM(string message, System.Action<string> callback)
    {
        // Simulasi request ke LLM dengan delay
        yield return new WaitForSeconds(2);

        // Hardcoded response jika LLM aktif
        callback("I am doing grrrrreeeat!");
    }
}
