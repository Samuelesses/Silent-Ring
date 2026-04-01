using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class interviewButton : MonoBehaviour
{
    public TMP_Text characterNameText;
    public GameObject notificationObject;
    public TMP_Text notificationTitleText;
    public TMP_Text notificationSubtitleText;
    [SerializeField] private bool autoHookToButton = true;

    private Button button;
    private Coroutine notificationCoroutine;

    private void Awake()
    {
        if (characterNameText == null)
        {
            characterNameText = FindFirstObjectByType<TMP_Text>();
        }

        if (!autoHookToButton)
        {
            return;
        }

        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Trigger);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(Trigger);
        }
    }

    public void Trigger()
    {
        if (SceneManager.GetActiveScene().name == "Interigation Room")
        {
            ShowNotification("Can't Interrogate", "Already In Interrogation");
            return;
        }

        string characterName = characterNameText != null ? characterNameText.text : string.Empty;

        if (string.IsNullOrWhiteSpace(characterName))
        {
            Debug.LogWarning("interviewButton: character name is empty.");
            return;
        }

        string interrogatedKey = characterName.Replace(" ", "_") + "_interrogated";
        if (PlayerPrefs.GetInt(interrogatedKey, 0) == 1)
        {
            Debug.Log($"Character '{characterName}' has already been interrogated.");
            ShowNotification("Can't Interrogate", "Already Interrogated Person");
            return;
        }

        gameManager manager = FindFirstObjectByType<gameManager>();
        if (manager == null)
        {
            Debug.LogWarning("interviewButton could not find a gameManager in the scene.");
            return;
        }

        manager.SetCharacter(characterName);
        SceneManager.LoadScene("Interigation Room");
    }

    private void ShowNotification(string title, string subtitle)
    {
        if (notificationTitleText != null)
        {
            notificationTitleText.text = title ?? string.Empty;
        }

        if (notificationSubtitleText != null)
        {
            notificationSubtitleText.text = subtitle ?? string.Empty;
        }

        if (notificationObject != null)
        {
            notificationObject.SetActive(true);

            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }

            notificationCoroutine = StartCoroutine(HideNotificationAfterDelay(3f));
        }
    }

    private System.Collections.IEnumerator HideNotificationAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (notificationObject != null)
        {
            notificationObject.SetActive(false);
        }

        notificationCoroutine = null;
    }
}
