using UnityEngine;

public class website : MonoBehaviour
{
    [SerializeField] private string url = "https://example.com";

    public void OpenWebsite()
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Debug.LogWarning("No URL set for website button.");
            return;
        }

        Application.OpenURL(url.Trim());
    }
}
