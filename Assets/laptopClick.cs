using UnityEngine;

public class laptopClick : MonoBehaviour
{
    public GameObject laptopUI;
    public playerMovement movement;

    private bool wasUIOpen;

    private void Start()
    {
        if (laptopUI != null)
        {
            wasUIOpen = laptopUI.activeSelf;
            ApplyUIState(wasUIOpen);
        }
    }

    void Update()
    {
        if (laptopUI == null)
        {
            return;
        }

        bool isUIOpen = laptopUI.activeSelf;

        if (isUIOpen != wasUIOpen)
        {
            ApplyUIState(isUIOpen);
            wasUIOpen = isUIOpen;
        }

        if (isUIOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetLaptopUI(false);
            }

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    SetLaptopUI(true);
                }
            }
        }
    }

    public void CloseLaptopUI()
    {
        SetLaptopUI(false);
    }

    private void SetLaptopUI(bool isOpen)
    {
        if (laptopUI == null)
        {
            return;
        }

        laptopUI.SetActive(isOpen);
        ApplyUIState(isOpen);
        wasUIOpen = isOpen;
    }

    private void ApplyUIState(bool isOpen)
    {
        if (!isOpen)
        {
            return;
        }

        if (movement != null)
        {
            movement.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
