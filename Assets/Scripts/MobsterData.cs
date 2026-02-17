using UnityEngine;

[CreateAssetMenu(fileName = "NewMobster", menuName = "Mobsters/Mobster")]
public class MobsterData : ScriptableObject
{
    public string mobsterName;
    [TextArea(5, 10)]
    public string basePersonality;
    [TextArea(3, 5)]
    public string secretIntel;
}