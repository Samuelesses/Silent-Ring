using UnityEngine;
using UnityEngine.SceneManagement;

public class InterviewManager : MonoBehaviour
{
    public static InterviewManager Instance { get; private set; }

    public string CurrentName { get; private set; } = string.Empty;
    public string CurrentMobsterInfo { get; private set; } = string.Empty;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetInterigartion(string Name, string mobsterinfo)
    {
        CurrentName = string.IsNullOrWhiteSpace(Name) ? string.Empty : Name.Trim();
        CurrentMobsterInfo = mobsterinfo ?? string.Empty;

        SceneManager.LoadScene("Interigation Room");
    }
}
