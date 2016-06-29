using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuScoreController : MonoBehaviour
{
    static MainMenuScoreController _instance;

    public static MainMenuScoreController Instance { get { return _instance; } }

    [HideInInspector]
    public int PlayerScore;

    public Text ScoreAmountLabel;

    public GameObject GameServicesButton;

    void Awake()
    {
        _instance = this;

        gameObject.SetActive(false);
        GameServicesButton.SetActive(false);
    }

    public void SetScore(int amount)
    {
        PlayerScore = amount;
        ScoreAmountLabel.text = amount.ToString();
        PlayGameConnector.Instance.PostScoreToLeaderboard(amount);
        gameObject.SetActive(true);
        if (PlayGameConnector.Instance.IsAuthenticated)
            GameServicesButton.SetActive(true);
        
    }

    public void EnableGameServicesButton()
    {
        GameServicesButton.SetActive(true);
    }
}
