using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class completeReport : MonoBehaviour
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
        int joeInterrogated = PlayerPrefs.GetInt("Joe_Longbottm_interrogated", 0);
        int adrianInterrogated = PlayerPrefs.GetInt("Adrian_Vale_interrogated", 0);

        if (joeInterrogated == 1 && adrianInterrogated == 1)
        {
            SceneManager.LoadScene("Complete");
        }
    }
}
