using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech; // Windows speech only

public class VoiceToText : MonoBehaviour
{
    public MobsterData mobsterData;
    public microphone microphoneState;
    private DictationRecognizer dictationRecognizer;
    private const float RestartDelaySeconds = 0.25f;

    private void Update()
    {
        UpdateListeningState();
    }

    void OnEnable()
    {
        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;
        UpdateListeningState();
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
        if (IsMuted())
            return;

        Debug.Log(text);
        if (mobsterData == null)
        {
            Debug.LogError("[VoiceToText] No mobster data assigned on VoiceToText.");
            return;
        }

        ConversationMemoryLog.AppendPlayerLine(text);

        aiManager aiManagerInstance = GetComponent<aiManager>();
        aiManagerInstance.SendAIRequest(mobsterData, text);
        microphoneState.ForceMute();
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        if (dictationRecognizer != null && !IsMuted())
            StartCoroutine(RestartAfterDelay());
    }

    private void OnDictationError(string error, int hresult)
    {
        if (dictationRecognizer != null && !IsMuted())
            StartCoroutine(RestartAfterDelay());
    }

    private System.Collections.IEnumerator RestartAfterDelay()
    {
        if (dictationRecognizer == null || IsMuted())
            yield break;

        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            dictationRecognizer.Stop();
        yield return new WaitForSeconds(RestartDelaySeconds);
        if (dictationRecognizer != null && !IsMuted() && dictationRecognizer.Status != SpeechSystemStatus.Running)
            dictationRecognizer.Start();
    }

    private bool IsMuted()
    {
        return microphoneState != null && microphoneState.muted;
    }

    private void UpdateListeningState()
    {
        if (dictationRecognizer == null)
            return;

        if (IsMuted())
        {
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();
            return;
        }

        if (dictationRecognizer.Status != SpeechSystemStatus.Running)
            dictationRecognizer.Start();
    }
}