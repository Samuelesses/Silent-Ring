using System.Text;
using TMPro;
using UnityEngine;

public class ConversationMemoryLog : MonoBehaviour
{
    private static ConversationMemoryLog instance;

    [SerializeField] private TextMeshProUGUI memoryText;

    private readonly StringBuilder conversationMemory = new StringBuilder();

    public static ConversationMemoryLog Instance
    {
        get
        {
            if (instance == null)
            {
                ConversationMemoryLog existing = FindFirstObjectByType<ConversationMemoryLog>();
                if (existing != null)
                {
                    instance = existing;
                }
                else
                {
                    GameObject memoryObject = new GameObject("ConversationMemoryLog");
                    instance = memoryObject.AddComponent<ConversationMemoryLog>();
                }
            }

            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        RefreshMemoryText();
    }

    public static void AppendPlayerLine(string text)
    {
        Instance.AppendLine("Player", text);
    }

    public static void AppendMobsterLine(string speakerName, string text)
    {
        string label = string.IsNullOrWhiteSpace(speakerName) ? "Criminal" : speakerName;
        Instance.AppendLine(label, text);
    }

    public static string GetMemory()
    {
        return Instance.conversationMemory.ToString();
    }

    private void AppendLine(string speakerName, string text)
    {
        string safeText = string.IsNullOrWhiteSpace(text) ? "(empty)" : text.Trim();

        if (conversationMemory.Length > 0)
        {
            conversationMemory.AppendLine();
        }

        conversationMemory.Append(speakerName);
        conversationMemory.Append(": ");
        conversationMemory.Append(safeText);

        RefreshMemoryText();
        Debug.Log("[ConversationMemoryLog] " + speakerName + ": " + safeText);
    }

    private void RefreshMemoryText()
    {
        if (memoryText != null)
        {
            memoryText.SetText(conversationMemory.ToString());
        }
    }
}