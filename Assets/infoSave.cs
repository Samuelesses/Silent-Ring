using UnityEngine;
using TMPro;

public class infoSave : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private string playerPrefsKey = string.Empty;

    public string selectedName;
    public string selectedRole;

    public void OnButtonClicked(string name, string role)
    {
        selectedName = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
        selectedRole = role ?? string.Empty;

        Debug.Log($"infoSave.OnButtonClicked -> name: '{selectedName}', role: '{selectedRole}' on '{gameObject.name}'");

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
