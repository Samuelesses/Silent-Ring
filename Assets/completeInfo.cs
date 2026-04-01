using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class completeInfo : MonoBehaviour
{
    public TextMeshProUGUI output;

    private const string OpenAiUrl = "https://api.openai.com/v1/chat/completions";

    private void Start()
    {
        StartCoroutine(GenerateEvaluation());
    }

    private IEnumerator GenerateEvaluation()
    {
        if (output == null)
        {
            yield break;
        }

        string apiKey = PlayerPrefs.GetString("key", string.Empty).Trim();
        if (string.IsNullOrEmpty(apiKey))
        {
            output.SetText("Missing PlayerPrefs key 'key'. Save your OpenAI API key there first.");
            yield break;
        }

        output.SetText("Grading report...");

        string prompt = BuildPrompt();
        ChatRequest requestBody = new ChatRequest
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new ChatMessage
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.2f
        };

        string json = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest(OpenAiUrl, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            output.SetText($"OpenAI request failed: {request.error}\n\n{request.downloadHandler.text}");
            yield break;
        }

        ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
        bool hasChoice = response != null && response.choices != null && response.choices.Length > 0;
        bool hasMessage = hasChoice && response.choices[0].message != null;
        string content = hasMessage ? response.choices[0].message.content : string.Empty;

        if (string.IsNullOrWhiteSpace(content))
        {
            output.SetText("OpenAI returned an empty response.");
            yield break;
        }

        output.SetText(content.Trim());
    }

    private string BuildPrompt()
    {
        string joeL = PlayerPrefs.GetString("Joe Longbottom", string.Empty);
        string adrianV = PlayerPrefs.GetString("Adrian Vale", string.Empty);

        return $@"
You are the Captain of a detective station. Your employee has just finished a report on 2 interrogations.
You will be reviering the players notes, meaning this does not need to be a long report, just be trueful.
Some informatin you have on correct information issnt needed on a players report, such as someone fearing legal trouble. Only core details are needed.
Don't use markdown like ** for bold, this wont work.
Dont grade charecters reports individually, give an overall rating and then write a paragraphy below.
The response should be rude, dont include swear words, but be close. It should be really rude, even if 5/5
Below I will provide what information is correct, and below that will be the players written report on each person. Please grade them like so.


'X/5
Mean description on why they got that rating
Mention below if the person is fired or promoted.'

Correct Information:

Joe Longbottom:
Joe Longbottom was near the warehouse just before the fire started.

He saw at least one person at the scene and realised the fire was deliberate.

He did not see everything clearly, but he knows it wasn’t an accident.

He chose not to report what he saw when first questioned.

He is afraid of getting into legal trouble for withholding information.

He is also afraid of whoever was responsible for the fire.

He suspects the person involved was not acting alone.

He believes he may have been noticed at the scene.

He knows the description of the person he saw was blue, with a red eye.

Adrian Vale:
Ordered the warehouse fire to destroy specific evidence stored inside.

Avoided direct involvement by operating through intermediaries.

Arranged for the job to be carried out by Marcus via a third party.

Was near the area at the time of the fire to ensure it was executed.

Did not fully inform Elena of the true purpose behind the fire.

Knew the fire would appear suspicious but prioritised removing the evidence.

Prepared to distance from and sacrifice Marcus if necessary.

Suspects at least one witness may have seen more than intended.

Is actively managing risk and exposure following the incident.


Players Report:
Joe Longbottom:
{joeL}

Adrian Vale:
{adrianV}

";
    }

    [Serializable]
    private class ChatRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature;
    }

    [Serializable]
    private class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    private class ChatResponse
    {
        public ChatChoice[] choices;
    }

    [Serializable]
    private class ChatChoice
    {
        public ChatMessage message;
    }
}
