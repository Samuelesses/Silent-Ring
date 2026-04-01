using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class personList : MonoBehaviour
{
    public string Name;
    public string Role;
    public Sprite ProfileImage;

    private void Awake()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child == transform)
            {
                continue;
            }

            if (child.CompareTag("image"))
            {
                child.GetComponent<RawImage>().texture = ProfileImage.texture;
            }
            else if (child.CompareTag("name"))
            {
                child.GetComponent<TextMeshProUGUI>().text = Name;
            }
            else if (child.CompareTag("role"))
            {
                child.GetComponent<TextMeshProUGUI>().text = Role;
            }
        }
    }
}
