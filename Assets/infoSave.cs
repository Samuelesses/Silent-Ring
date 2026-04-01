using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class infoSave : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private RawImage portraitImage;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private string playerPrefsKey = string.Empty;

    public string selectedName;
    public string selectedRole;
    public Sprite selectedSprite;

    public void OnButtonClicked(string name, string role, Sprite sprite)
    {
        selectedName = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
        selectedRole = role ?? string.Empty;
        selectedSprite = sprite;

        Debug.Log($"infoSave.OnButtonClicked -> name: '{selectedName}', role: '{selectedRole}', sprite: '{(selectedSprite != null ? selectedSprite.name : "none")}' on '{gameObject.name}'");

        if (string.IsNullOrWhiteSpace(selectedName))
        {
            Debug.LogWarning("Name is empty. Button click ignored.");
            return;
        }

        if (nameText != null)
        {
            nameText.text = selectedName;
        }

        if (roleText != null)
        {
            roleText.text = selectedRole;
        }

        if (portraitImage != null)
        {
            portraitImage.texture = selectedSprite != null ? selectedSprite.texture : null;
        }

        playerPrefsKey = selectedName;

        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                inputField.SetTextWithoutNotify(PlayerPrefs.GetString(playerPrefsKey, string.Empty));
            }
            else
            {
                inputField.SetTextWithoutNotify(string.Empty);
            }
        }
    }

    public void OnInputValueChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(playerPrefsKey))
        {
            Debug.LogWarning("No playerPrefs key set yet. Click a name/role button first.");
            return;
        }

        PlayerPrefs.SetString(playerPrefsKey, value ?? string.Empty);
        PlayerPrefs.Save();
        Debug.Log("Pref saved");
    }
}
