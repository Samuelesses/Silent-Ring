using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private static gameManager instance;

    private string currentCharacterName = string.Empty;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    public void SetCharacter(string characterName)
    {
        currentCharacterName = string.IsNullOrWhiteSpace(characterName) ? string.Empty : characterName.Trim();
        
        if (!string.IsNullOrEmpty(currentCharacterName))
        {
            string interrogatedKey = currentCharacterName.Replace(" ", "_") + "_interrogated";
            PlayerPrefs.SetInt(interrogatedKey, 1);
            PlayerPrefs.Save();
        }
        
        TryEnableCharacter(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryEnableCharacter(scene);
    }

    private void TryEnableCharacter(Scene scene)
    {
        if (!scene.IsValid() || scene.name != "Interigation Room" || string.IsNullOrEmpty(currentCharacterName))
        {
            return;
        }

        GameObject characterObject = FindCharacterObject(scene, currentCharacterName);
        if (characterObject != null)
        {
            characterObject.SetActive(true);
        }
    }

    private GameObject FindCharacterObject(Scene scene, string characterName)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject rootObject in rootObjects)
        {
            Transform match = FindTransformByName(rootObject.transform, characterName);
            if (match != null)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindTransformByName(Transform parent, string targetName)
    {
        if (parent.name == targetName)
        {
            return parent;
        }

        foreach (Transform child in parent)
        {
            Transform match = FindTransformByName(child, targetName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
