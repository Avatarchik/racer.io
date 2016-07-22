using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardContainer : MonoBehaviour
{
    public Image CrownImage;
    public Text Rank, Username, Score;

    CombatCarScript _car;

    public void Set(CombatCarScript targetCar)
    {
        Color targetColor = Color.white;

        if (targetCar == null)
        {
            Username.text = "";
            Score.text = "0";

            Username.color = targetColor;
            Score.color = targetColor;
            Rank.color = targetColor;

            return;
        }

        if (CrownImage != null)
            CrownImage.gameObject.SetActive(true);

        _car = targetCar;

        if (_car.Username.text == "")
            Username.text = "<Unnamed>";
        else
            Username.text = _car.Username.text;

        Score.text = _car.Score.ToString();

        if (_car.IsPlayerCar)
            targetColor = LeaderboardController.Instance.PlayerContainerTextColor;

        Username.color = targetColor;
        Score.color = targetColor;
        Rank.color = targetColor;
    }

    public void Set(CombatCarScript targetCar, int rank)
    {
        Rank.text = rank.ToString();

        Set(targetCar);
    }
}
