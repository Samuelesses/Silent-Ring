using UnityEngine;
using UnityEngine.UI;

public class infoButton : MonoBehaviour
{
    [SerializeField] private infoSave target;
    [SerializeField] private string nameValue = string.Empty;
    [SerializeField] private string roleValue = string.Empty;
    [SerializeField] private bool autoHookToButton = true;

    private Button button;

    private void Awake()
    {
        if (target == null)
        {
            target = FindFirstObjectByType<infoSave>();
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
        if (target == null)
        {
            Debug.LogWarning("infoButton has no infoSave target assigned.");
            return;
        }

        Debug.Log($"infoButton.Trigger -> name: '{nameValue}', role: '{roleValue}', target: '{target.gameObject.name}'");

        target.OnButtonClicked(nameValue, roleValue);
    }
}
