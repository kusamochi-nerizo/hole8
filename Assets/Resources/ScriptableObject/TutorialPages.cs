using UnityEngine;

[CreateAssetMenu(fileName = "TutorialPages", menuName = "ScriptableObjects/TutorialPages", order = 1)]
public class TutorialPages : ScriptableObject
{
    [TextArea(3,10)]
    public string[] pages;
}