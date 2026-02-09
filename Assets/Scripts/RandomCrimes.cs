using UnityEngine;

public class RandomCrimes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string[] crimes = {
            "Trespassing",
            "Vandalism",
            "Theft",
            "Disturbing the Peace",
            "Breaking and Entering",
            "Suspicious Activity",
            "Loitering"
        };
        
        string[] shirtColors = {
            "Red",
            "Blue",
            "Green",
            "Black",
            "White",
            "Yellow",
            "Gray",
            "Purple"
        };
        
        string[] actions = {
            "Running away",
            "Walking slowly",
            "Standing still",
            "Looking around nervously",
            "Talking on the phone",
            "Carrying a bag",
            "Climbing a fence",
            "Entering a building"
        };
        
        int crimeIndex = Random.Range(0, crimes.Length);
        int shirtIndex = Random.Range(0, shirtColors.Length);
        int actionIndex = Random.Range(0, actions.Length);
        
        string chosenCrime = crimes[crimeIndex];
        string chosenShirtColor = shirtColors[shirtIndex];
        string chosenAction = actions[actionIndex];
        
        Debug.Log("Crime: " + chosenCrime);
        Debug.Log("Shirt Color: " + chosenShirtColor);
        Debug.Log("Action: " + chosenAction);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}