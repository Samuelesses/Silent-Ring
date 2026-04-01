using UnityEngine;

public class tabletInitialLoad : MonoBehaviour
{
    [SerializeField] private GameObject tutorialObject;

    private const string TutorialKey = "tutorial";

    void Start()
    {
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1 && tutorialObject != null)
        {
            tutorialObject.SetActive(true);
        }
    }

    public void SetTutorialTrue()
    {
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();

        if (tutorialObject != null)
        {
            tutorialObject.SetActive(true);
        }
    }
}
