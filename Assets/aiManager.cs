using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class aiManager : MonoBehaviour
{
    // Kept hardcoded by design; not serialized so Inspector cannot override it.
    private const string apiKey = "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";

    void Start()
    {
        // Remove automatic request - now called externally
    }

    /// <summary>
    /// Send a request to OpenAI API with a mobster's personality and a prompt.
    /// </summary>
    public void SendAIRequest(MobsterData mobster, string prompt)
    {
        if (mobster == null)
        {
            Debug.LogError("[aiManager] SendAIRequest called with null mobster.");
            return;
        }

        StartCoroutine(SendRequest(mobster, prompt));
    }

    IEnumerator SendRequest(MobsterData mobster, string prompt)
    {
        string url = "https://api.openai.com/v1/responses";
        string trimmedKey = (apiKey ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(trimmedKey))
        {
            Debug.LogError("[aiManager] API key is empty.");
            yield break;
        }

        string systemPrompt = mobster.basePersonality ?? string.Empty;
        string userPrompt = prompt ?? string.Empty;

        string jsonBody = $@"
        {{
            ""model"": ""gpt-4o-mini"",
            ""instructions"": ""{EscapeJson(systemPrompt)}"",
            ""input"": ""{EscapeJson(userPrompt)}"",
            ""max_output_tokens"": 150
        }}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + trimmedKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string assistantText = ExtractAssistantText(request.downloadHandler.text);
            ConversationMemoryLog.AppendMobsterLine(mobster.mobsterName, assistantText);
            Debug.Log("[aiManager] Response: " + assistantText);
        }
        else
        {
            Debug.LogError("[aiManager] Error: " + request.error + " | HTTP " + request.responseCode);
            Debug.LogError("[aiManager] Error body: " + request.downloadHandler.text);

            if (request.responseCode == 401)
            {
                string keySuffix = trimmedKey.Length >= 4 ? trimmedKey.Substring(trimmedKey.Length - 4) : trimmedKey;
                Debug.LogError("[aiManager] 401 Unauthorized. The key being sent ends with: " + keySuffix + ". In Unity, a serialized field in the Inspector can override code defaults.");
            }
        }
    }

    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    private string ExtractAssistantText(string json)
    {
        OpenAIResponse parsed = JsonUtility.FromJson<OpenAIResponse>(json);

        if (parsed == null || parsed.output == null)
        {
            return "(no assistant text found)";
        }

        foreach (OutputMessage message in parsed.output)
        {
            if (message == null || message.content == null)
            {
                continue;
            }

            foreach (OutputContent content in message.content)
            {
                if (content != null && content.type == "output_text" && !string.IsNullOrEmpty(content.text))
                {
                    return content.text;
                }
            }
        }

        return "(no assistant text found)";
    }

    [System.Serializable]
    private class OpenAIResponse
    {
        public OutputMessage[] output;
    }

    [System.Serializable]
    private class OutputMessage
    {
        public OutputContent[] content;
    }

    [System.Serializable]
    private class OutputContent
    {
        public string type;
        public string text;
    }
}