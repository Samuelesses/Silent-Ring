using UnityEngine;
using TMPro;

public class InfoInput : MonoBehaviour
{
    [SerializeField] private infoSave target;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        if (target == null)
        {
            target = FindFirstObjectByType<infoSave>();
        }
    }

    private void OnEnable()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnValueChanged);
        }
    }

    private void OnDisable()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnValueChanged);
        }
    }

    public void OnValueChanged(string value)
    {
        if (target == null)
        {
            Debug.LogWarning("InfoInput has no infoSave target assigned.");
            return;
        }

        target.OnInputValueChanged(value);
    }
}
