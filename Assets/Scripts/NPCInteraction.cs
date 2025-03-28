using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    //public NPCData npcData; //Scriptable object for NPC

    public float detectionRadius = 2f;
    public LayerMask playerLayer;
    private LLMCharacter llmCharacter;

    public TMP_InputField playerText;
    public TMP_Text AIText;
    public GameObject ChatPanel;
    public Button nextButton;

    public LLMCharacter lLMCharacter;
    //  public GameObject interactionIcon;

    public GameObject npcDialogPanel;

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false;
    private bool playerNearby = false;

    public static bool playerCantMove { get; private set; } = false;

    void Start()
    {
        ChatPanel.SetActive(false);
        npcDialogPanel.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        // interactionIcon.SetActive(false);

        llmCharacter = GetComponent<LLMCharacter>();

        playerText.onSubmit.AddListener(onInputFieldSubmit);
        nextButton.onClick.AddListener(ShowNextLine);
        playerText.Select();

        //lLMCharacter.prompt = npcData.characterPrompt;
        //lLMCharacter.AIName = npcData.npcName;
    }

    void onInputFieldSubmit(string message)
    {
        playerText.interactable = false;
        AIText.text = "...";
        _ = llmCharacter.Chat(message, SetAIText, AIReplyComplete);
    }

    public void SetAIText(string text)
    {
        dialogueQueue.Clear();
        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                dialogueQueue.Enqueue(line.Trim());
            }
        }

        isDialogueActive = true;
        AIText.text = "";
        ShowNextLine();
    }

    public void ShowNextLine()
    {
        if (dialogueQueue.Count > 0)
        {
            AIText.text = dialogueQueue.Dequeue();
            nextButton.gameObject.SetActive(dialogueQueue.Count > 0);
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        playerText.interactable = true;
        playerText.Select();
        playerText.text = "Hello";
        nextButton.gameObject.SetActive(false);
    }

    public void AIReplyComplete()
    {
        if (!isDialogueActive)
        {
            EndDialogue();
        }
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
        //interactionIcon.SetActive(false);
        nextButton.gameObject.SetActive(false);
        AIText.text = "Hello"; //back to default text
        playerText.text = "";
        playerText.interactable = true;
        playerCantMove = false;
    }

    void Update()
    {
        DetectPlayer();

        if (isDialogueActive && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0)))
        {
            ShowNextLine();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitDialogue();
        }
    }

    void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        playerNearby = (player != null);

        //interactionIcon.SetActive(playerNearby);

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
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