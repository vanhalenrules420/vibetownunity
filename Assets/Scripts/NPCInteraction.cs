using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    public float detectionRadius = 2f;
    public LayerMask playerLayer;
    private LLMCharacter llmCharacter;

    public TMP_InputField playerText;
    public TMP_Text AIText;
    public GameObject ChatPanel;
    public Button nextButton;

    public string defaultDialog;
    public LLMCharacter lLMCharacter;
    public GameObject npcDialogPanel;

    private bool isDialogueActive = false;
    private bool playerNearby = false;

    public static bool playerCantMove { get; private set; } = false;

    void Start()
    {
        ChatPanel.SetActive(false);
        npcDialogPanel.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);

        llmCharacter = GetComponent<LLMCharacter>();

        playerText.onSubmit.AddListener(onInputFieldSubmit);
        playerText.Select();
    }

    void onInputFieldSubmit(string message)
    {
        playerText.interactable = false;
        AIText.text = "...";
        _ = llmCharacter.Chat(message, SetAIText, AIReplyComplete);
    }

    public void SetAIText(string text)
    {
        AIText.text = text; // Menampilkan seluruh teks langsung tanpa queue
        isDialogueActive = true;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        playerText.interactable = true;
        playerText.Select();
        playerText.text = ""; // Mengosongkan teks setelah dialog selesai
    }

    public void AIReplyComplete()
    {
        isDialogueActive = false; // Memastikan dialog benar-benar berakhir
        EndDialogue();
    }

    public void CancelRequests()
    {
        llmCharacter.CancelRequests();
        AIReplyComplete();
    }

    public void ExitDialogue()
    {
        isDialogueActive = false;
        ChatPanel.SetActive(false);
        npcDialogPanel.SetActive(false);
        AIText.text = "Hello";
        playerText.text = "";
        playerText.interactable = true;
        playerCantMove = false;
    }

    void Update()
    {
        DetectPlayer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitDialogue();
        }
    }

    void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        playerNearby = (player != null);

        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !playerText.isFocused) // Tambahkan pengecekan agar tidak mengganggu input
        {
            AIText.text = defaultDialog;
            ShowChatPanel();
            playerCantMove = true;
        }
    }

    void ShowChatPanel()
    {
        ChatPanel.SetActive(true);
        npcDialogPanel.gameObject.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
