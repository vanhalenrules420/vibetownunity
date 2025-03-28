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
    public Button submitButton; 

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

        llmCharacter = GetComponent<LLMCharacter>();

        playerText.onSubmit.AddListener(onInputFieldSubmit);
        submitButton.onClick.AddListener(SubmitText); 
        submitButton.interactable = true;
        playerText.Select();
    }

    void onInputFieldSubmit(string message)
    {
        SubmitText();
    }

    public void SubmitText()
    {
        if (!string.IsNullOrWhiteSpace(playerText.text))
        {
            playerText.interactable = false;
            submitButton.interactable = false; 
            AIText.text = "...";
            _ = llmCharacter.Chat(playerText.text, SetAIText, AIReplyComplete);
        }
    }

    public void SetAIText(string text)
    {
        AIText.text = text;
        isDialogueActive = true;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        playerText.interactable = true;
        submitButton.interactable = true; 
        playerText.Select();
        playerText.text = ""; 
    }

    public void AIReplyComplete()
    {
        isDialogueActive = false; 
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
        submitButton.interactable = true;
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

        if (playerNearby && Input.GetKeyDown(KeyCode.E) && !playerText.isFocused) 
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
