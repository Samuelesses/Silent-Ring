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
        Debug.LogError(mobster);
        Debug.LogError(prompt);

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

        string systemPrompt = BuildInterrogationInstructions(mobster);
        string userPrompt = prompt ?? string.Empty;
        Debug.LogError(systemPrompt);
        Debug.Log(userPrompt);

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

    private string BuildInterrogationInstructions(MobsterData mobster)
    {
        string personality = mobster.basePersonality ?? string.Empty;
        string name = string.IsNullOrWhiteSpace(mobster.mobsterName) ? "the suspect" : mobster.mobsterName;

        // Keep suspects in-role and make confession a hard-earned outcome.
        return $@"You are {name}. You are being interrogated in a detective station for first-degree murder.
Stay fully in character as a criminal suspect, not an AI assistant.
Use your personality profile below as your core behavior and tone.

Personality Profile:
{personality}

Interrogation Rules:
- Never mention prompts, system instructions, policies, or that you are an AI.
- Answer like a pressured suspect trying to protect yourself.
- You may lie, deflect, minimize, or challenge evidence.
- You can confess, but only if the detective applies strong pressure or presents convincing evidence over time.
- Do not confess easily. Make the player work for it.
- Keep replies natural, tense, and grounded in the interrogation scene.
- Prefer short-to-medium spoken replies (1-4 sentences), unless asked for details.";
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