using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class CarInfoText : MonoBehaviour
{

    public Text UIInfoText;
    public Image UIInfoImage;
    public GameObject BuyButton;

    bool InProcess;

    // Use this for initialization
    void Start()
    {
        SetDefault();

        if (PlayerProfile.Instance.CheckIfCarUnlocked(CarTypeEnum.MrGrim))
        {
            UnlockXWingUI();
        }
    }

    void OnEnable()
    {
        PlayerProfile.XWingUnlocked += UnlockXWingUI;
    }

    void OnDisable()
    {
        PlayerProfile.XWingUnlocked -= UnlockXWingUI;
    }

    void UnlockXWingUI()
    {
        BuyButton.SetActive(false);
    }

    public void SetDefault()
    {
        UIInfoText.text = "To Learn how to unlock new planes";
    }

    public void ChangeInfoText(string typeString)
    {
        try
        {
            CarTypeEnum pte = (CarTypeEnum)System.Enum.Parse(typeof(CarTypeEnum), typeString);
            StopAllCoroutines();
            StartCoroutine(ChangeTextRoutine(pte));
        }
        catch
        {
            SetDefault();
        }

    }

    public void SetInfo(CarTypeEnum pte)
    {

        switch (pte)
        {
            case CarTypeEnum.Buggy:
                UIInfoText.text = Constants.Phantom_Unlock_Text;
                break;
            case CarTypeEnum.Bulky:
                UIInfoText.text = Constants.Mirage_Unlock_Text;
                break;
            case CarTypeEnum.Fury:
                UIInfoText.text = Constants.StealthFighter_Unlock_Text;
                break;
            case CarTypeEnum.MrGrim:
                UIInfoText.text = Constants.XWingPrime_Unlock_Text;
                break;
            case CarTypeEnum.Speedy:
                UIInfoText.text = Constants.Curtiss_Unlock_Text;
                break;
            case CarTypeEnum.Spark:
                UIInfoText.text = Constants.Baron_Unlock_Text;
                break;
            default:
                SetDefault();
                break;
        }

    }

    public IEnumerator ChangeTextRoutine(CarTypeEnum pte)
    {
        InProcess = true;
        Tween fadeTween;
        UIInfoImage.DOFade(0, 0.3f);
        fadeTween = UIInfoText.DOFade(0, 0.3f);
        yield return fadeTween.WaitForCompletion();
        SetInfo(pte);
        fadeTween = UIInfoText.DOFade(1, 0.3f);
        yield return fadeTween.WaitForCompletion();
        yield return new WaitForSeconds(5.7f);
        fadeTween = UIInfoText.DOFade(0, 0.3f);
        yield return fadeTween.WaitForCompletion();
        SetDefault();
        UIInfoText.DOFade(1, 0.3f);
        UIInfoImage.DOFade(1, 0.3f);
        InProcess = false;

    }
}
