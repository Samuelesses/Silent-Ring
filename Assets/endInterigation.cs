using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class endInterigation : MonoBehaviour
{
    [SerializeField] private bool autoHookToButton = true;

    private Button button;

    private void Awake()
    {
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
        if (SceneManager.GetActiveScene().name == "Police Station")
        {
            return;
        }

        SceneManager.LoadScene("Police Station");
    }
}
