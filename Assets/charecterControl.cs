using UnityEngine;

public class charecterControl : MonoBehaviour
{
    public MobsterData mobsterData;
    public VoiceToText voicett;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        voicett.mobsterData = mobsterData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
