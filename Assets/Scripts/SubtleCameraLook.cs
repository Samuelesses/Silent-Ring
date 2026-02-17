using UnityEngine;

public class AbsoluteMouseRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxYaw = 10f;
    public float maxPitch = 5f;

    void Update()
    {

        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        mouseX = Mathf.Clamp(mouseX, -1f, 1f);
        mouseY = Mathf.Clamp(mouseY, -1f, 1f);

        float yaw = mouseX * maxYaw;
        float pitch = -mouseY * maxPitch;

        transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
