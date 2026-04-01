using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playButton : MonoBehaviour
{
    private const string PlayerPrefsKey = "key";
    private const string ValidateUrl = "https://api.openai.com/v1/models";
    private const string TargetSceneName = "Police Station";

    private Button button;

    private bool isValidating;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnPlayButtonPressed);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnPlayButtonPressed);
        }
    }

    public void OnPlayButtonPressed()
    {
        if (isValidating)
        {
            return;
        }

        StartCoroutine(ValidateSavedKey());
    }

    private System.Collections.IEnumerator ValidateSavedKey()
    {
        isValidating = true;

        string apiKey = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
        apiKey = string.IsNullOrWhiteSpace(apiKey) ? string.Empty : apiKey.Trim();

        if (string.IsNullOrEmpty(apiKey))
        {
            isValidating = false;
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(ValidateUrl))
        {
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            bool isSuccess = request.result == UnityWebRequest.Result.Success;
            long statusCode = request.responseCode;

            if (isSuccess && statusCode >= 200 && statusCode < 300)
            {
                SceneManager.LoadScene(TargetSceneName);
            }
        }

        isValidating = false;
    }
}
