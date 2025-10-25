using TMPro;
using UnityEngine;

/// <summary>
/// Class responsible for handling anything UI related.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [SerializeField, Tooltip("The points text object")]
    private TextMeshProUGUI _pointsText;

    /// <summary>
    /// Function responsible for updating the points text.
    /// </summary>
    /// <param name="points">The points to set the text to.</param>
    public void SetPointsText(int points)
    {
        _pointsText.text = $"Points: {points}";
    }
}