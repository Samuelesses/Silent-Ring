using UnityEngine;

public class charecterControl : MonoBehaviour
{
    public MobsterData mobsterData;
    public aiManager aimanager;
    public VoiceToText voicett;
    public string voice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        voicett.mobsterData = mobsterData;
        aimanager.ttsVoice = voice;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
