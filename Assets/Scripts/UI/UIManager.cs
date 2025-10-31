using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for handling anything UI related.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [SerializeField, Tooltip("The points text object")]
    private TextMeshProUGUI _pointsText;

    [SerializeField]
    private Image _enduranceBar;

    [SerializeField]
    private Image _enduranceBarOutline;

    private bool _enduraceBarShouldFlicker = false;
    private Coroutine _enduranceBarFlicker;

    /// <summary>
    /// Function responsible for updating the points text.
    /// </summary>
    /// <param name="points">The points to set the text to.</param>
    public void SetPointsText(int points)
    {
        _pointsText.text = $"Points: {points}";
    }

    /// <summary>
    /// Function responsible for updating the endurance bar image fill.
    /// </summary>
    /// <param name="endurance">The amount of endurance to set the image fill to.</param>
    public void SetEnduranceBarFill(float endurance)
    {
        if (endurance < 0.0f)
        {
            endurance = 0.0f;
        }
        else if (endurance > 1.0f)
        {
            endurance = 1.0f;
        }

        _enduraceBarShouldFlicker = endurance < 0.35f;
        _enduranceBar.fillAmount = endurance;

        if (_enduraceBarShouldFlicker)
        {
            _enduranceBarFlicker ??= StartCoroutine(enduranceBarFlicker(endurance));
        }
    }

    private IEnumerator enduranceBarFlicker(float endurance)
    {
        bool isRed = true;

        while (_enduraceBarShouldFlicker)
        {
            _enduranceBarOutline.color = isRed ? Color.red : Color.white;
            isRed = !isRed;

            yield return new WaitForSeconds(endurance / 2.0f);
        }

        _enduranceBarFlicker = null;
        _enduranceBarOutline.color = Color.white;
    }
}