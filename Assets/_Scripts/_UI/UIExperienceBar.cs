using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIExperienceBar : MonoBehaviour
{
    public Image FillBar;
    public Text LevelAmountText;

    void Awake()
    {
        PlayerProfile.ExperienceLoaded += PlayerProfile_ExperienceLoaded;
    }

    void OnEnable()
    {
        if (ExperienceManager.Instance == null)
            return;

        CalculateFillBar();
    }

    void PlayerProfile_ExperienceLoaded(int curXp, int curLvl)
    {
        PlayerProfile.ExperienceLoaded -= PlayerProfile_ExperienceLoaded;

        CalculateFillBar();
    }

    void CalculateFillBar()
    {
        int _curXP = ExperienceManager.Instance.CurXP;
        int _nextLvlXP = ExperienceManager.Instance.NextLevelXP;
        int _prevLvlXP = ExperienceManager.Instance.CalculateXPForLevel(ExperienceManager.Instance.PrevLevel);

        int totalMustGain = _nextLvlXP - _prevLvlXP;
        int curMustGain = _nextLvlXP - _curXP;

        float fillAmount = 1f - ((float)curMustGain / (float)totalMustGain);

        FillBar.fillAmount = fillAmount;
        LevelAmountText.text = ExperienceManager.Instance.CurLevel.ToString();
    }
}
