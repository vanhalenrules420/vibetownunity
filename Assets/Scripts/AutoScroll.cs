using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect; // Assign ScrollRect dari inspector
    public TextMeshProUGUI textMeshPro; // Assign TextMeshPro dari inspector

    void Start()
    {
        ScrollToBottom();
    }

    public void UpdateText(string newText)
    {
        textMeshPro.text += "\n" + newText; // Menambah teks baru
        Canvas.ForceUpdateCanvases(); // Memaksa update layout
        ScrollToBottom();
    }

    void ScrollToBottom()
    {
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
