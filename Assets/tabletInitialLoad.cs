using UnityEngine;

public class tabletInitialLoad : MonoBehaviour
{
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private GameObject interrogatedObject;

    private const string TutorialKey = "tutorial";
    private const string JoeInterrogatedKey = "Joe_Longbottm_interrogated";
    private const string AdrianInterrogatedKey = "Adrian_Vale_interrogated";

    void Start()
    {
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1 && tutorialObject != null)
        {
            tutorialObject.SetActive(true);
        }

        if (interrogatedObject != null)
        {
            bool bothInterrogated = PlayerPrefs.GetInt(JoeInterrogatedKey, 0) == 1
                && PlayerPrefs.GetInt(AdrianInterrogatedKey, 0) == 1;
            interrogatedObject.SetActive(bothInterrogated);
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
