using UnityEngine;

public class MouseHoverLogger : MonoBehaviour
{
    public MobsterData mobsterData;
    private bool isHovering;

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("[MouseHoverLogger] Camera.main is null! Make sure there's a camera tagged as 'MainCamera' in your scene.");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool rayhit = Physics.Raycast(ray, out hit);
        bool hovering = rayhit && hit.collider != null && hit.collider.gameObject == gameObject;
        
        if (rayhit)
        {
            Debug.Log($"[MouseHoverLogger] Raycast hit: {hit.collider.gameObject.name}, checking against: {gameObject.name}");
        }
        
        if (hovering && !isHovering)
        {
            Debug.Log($"[MouseHoverLogger] Now hovering over {gameObject.name}, setting currentMobster to {mobsterData.mobsterName}");
            VoiceToText.currentMobster = mobsterData;
        }
        else if (!hovering && isHovering)
        {
            Debug.Log($"[MouseHoverLogger] Stopped hovering over {gameObject.name}");
        }
        isHovering = hovering;
    }
}
