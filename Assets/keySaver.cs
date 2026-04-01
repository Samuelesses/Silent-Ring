using UnityEngine;

public class keySaver : MonoBehaviour
{
    private const string PlayerPrefsKey = "key";

    public void OnEndEdit(string enteredValue)
    {
        PlayerPrefs.SetString(PlayerPrefsKey, enteredValue ?? string.Empty);
        PlayerPrefs.Save();

        Debug.Log("Saved PlayerPrefs key 'key'.");
    }
}
