using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech; // Windows speech only

public class VoiceToText : MonoBehaviour
{
    public TextMeshProUGUI playerSubtitle;
    public static MobsterData currentMobster;
    private DictationRecognizer dictationRecognizer;
    private const float RestartDelaySeconds = 0.25f;

    void OnEnable()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;
        dictationRecognizer.Start();
    }

    void OnDisable()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.DictationResult -= OnDictationResult;
            dictationRecognizer.DictationComplete -= OnDictationComplete;
            dictationRecognizer.DictationError -= OnDictationError;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
            dictationRecognizer = null;
        }
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log(text);
        playerSubtitle.SetText($"You Said:\n{text}");
        if (currentMobster == null)
        {
            Debug.LogError("[VoiceToText] No mobster selected! Hover over a character first.");
            return;
        }
        aiManager aiManagerInstance = GetComponent<aiManager>();
        aiManagerInstance.SendAIRequest(currentMobster, text);
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        if (dictationRecognizer != null)
            StartCoroutine(RestartAfterDelay());
    }

    private void OnDictationError(string error, int hresult)
    {
        if (dictationRecognizer != null)
            StartCoroutine(RestartAfterDelay());
    }

    private System.Collections.IEnumerator RestartAfterDelay()
    {
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            dictationRecognizer.Stop();
        yield return new WaitForSeconds(RestartDelaySeconds);
        if (dictationRecognizer.Status != SpeechSystemStatus.Running)
            dictationRecognizer.Start();
    }
}