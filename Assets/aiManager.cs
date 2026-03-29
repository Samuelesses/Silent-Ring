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

        ResponseRequestBody payload = new ResponseRequestBody
        {
            model = "gpt-4o-mini",
            instructions = systemPrompt,
            input = userPrompt,
            max_output_tokens = 150
        };

        string jsonBody = JsonUtility.ToJson(payload);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        bool shouldRetry = false;
        bool completed = false;

        for (int attempt = 1; attempt <= 2; attempt++)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = 60;

                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + trimmedKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string assistantText = ExtractAssistantText(request.downloadHandler.text);
                    ConversationMemoryLog.AppendMobsterLine(mobster.mobsterName, assistantText);
                    Debug.Log("[aiManager] Response: " + assistantText);

                    // Generate TTS audio for the response
                    StartCoroutine(GenerateTTS(assistantText));
                    completed = true;
                    break;
                }

                long code = request.responseCode;
                string requestId = request.GetResponseHeader("x-request-id");
                string errorBody = request.downloadHandler != null ? request.downloadHandler.text : "(empty body)";

                Debug.LogError("[aiManager] Error: " + request.error + " | HTTP " + code + " | RequestId " + (string.IsNullOrEmpty(requestId) ? "(none)" : requestId));
                Debug.LogError("[aiManager] Error body: " + errorBody);

                if (code == 401)
                {
                    string keySuffix = trimmedKey.Length >= 4 ? trimmedKey.Substring(trimmedKey.Length - 4) : trimmedKey;
                    Debug.LogError("[aiManager] 401 Unauthorized. The key being sent ends with: " + keySuffix + ". In Unity, a serialized field in the Inspector can override code defaults.");
                }

                shouldRetry = code >= 500 && code <= 599 && attempt == 1;
            }

            if (shouldRetry)
            {
                Debug.LogWarning("[aiManager] Retrying once after server error...");
                yield return new WaitForSeconds(0.8f);
            }
        }

        if (!completed)
        {
            Debug.LogError("[aiManager] Request failed after retry. Payload size: " + bodyRaw.Length + " bytes.");
        }
    }

    private string EscapeJson(string s)
    {
        return (s ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    private string BuildInterrogationInstructions(MobsterData mobster)
    {
        string scene = mobster.sceneInfo ?? string.Empty;
        string personality = mobster.basePersonality ?? string.Empty;
        string name = string.IsNullOrWhiteSpace(mobster.mobsterName) ? "the suspect" : mobster.mobsterName;
        string conversationHistory = ConversationMemoryLog.GetMemory();

        return $@"
Name:
{name}

Scene:
{scene}

Personality Profile:
{personality}

Conversation History:
{(string.IsNullOrEmpty(conversationHistory) ? "(conversation just started)" : conversationHistory)}";
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

    IEnumerator GenerateTTS(string text)
    {
        string url = "https://api.openai.com/v1/audio/speech";
        string trimmedKey = (apiKey ?? string.Empty).Trim();

        TtsRequestBody payload = new TtsRequestBody
        {
            model = "gpt-4o-mini-tts",
            input = text ?? string.Empty,
            voice = "alloy",
            response_format = "mp3"
        };

        string jsonBody = JsonUtility.ToJson(payload);

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = 60;

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "audio/mpeg");
        request.SetRequestHeader("Authorization", "Bearer " + trimmedKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] audioData = request.downloadHandler.data;
            Debug.Log("[aiManager] TTS audio generated successfully. " + audioData.Length + " bytes.");

            // Get or create AudioSource
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Save to temp file and load as AudioClip
            string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "tts_" + System.Guid.NewGuid() + ".mp3");
            System.IO.File.WriteAllBytes(tempPath, audioData);

            UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file:///" + tempPath, AudioType.MPEG);
            yield return audioRequest.SendWebRequest();

            if (audioRequest.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                audioSource.clip = audioClip;
                audioSource.Play();
                Debug.Log("[aiManager] Playing TTS audio...");
            }
            else
            {
                Debug.LogError("[aiManager] Failed to load audio clip: " + audioRequest.error);
            }
        }
        else
        {
            Debug.LogError("[aiManager] TTS Error: " + request.error + " | HTTP " + request.responseCode);
            Debug.LogError("[aiManager] TTS Error body: " + request.downloadHandler.text);
        }
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

    [System.Serializable]
    private class ResponseRequestBody
    {
        public string model;
        public string instructions;
        public string input;
        public int max_output_tokens;
    }

    [System.Serializable]
    private class TtsRequestBody
    {
        public string model;
        public string input;
        public string voice;
        public string response_format;
    }
}