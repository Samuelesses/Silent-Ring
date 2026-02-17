using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class aiManager : MonoBehaviour
{
    [SerializeField] private string apiKey = "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";

    void Start()
    {
        // Remove automatic request - now called externally
    }

    /// <summary>
    /// Send a request to OpenAI API with a mobster's personality and a prompt
    /// </summary>
    public void SendAIRequest(MobsterData mobster, string prompt)
    {
        StartCoroutine(SendRequest(mobster, prompt));
    }

    IEnumerator SendRequest(MobsterData mobster, string prompt)
    {
        string url = "https://api.openai.com/v1/chat/completions";

        // Create system prompt with mobster personality
        string systemPrompt = $"{mobster.basePersonality}";
        
        string jsonBody = $@"
        {{
            ""model"": ""gpt-4o-mini"",
            ""messages"": [
                {{ ""role"": ""system"", ""content"": ""{EscapeJson(systemPrompt)}"" }},
                {{ ""role"": ""user"", ""content"": ""{EscapeJson(prompt)}"" }}
            ],
            ""max_tokens"": 150
        }}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }
    }

    private string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }}