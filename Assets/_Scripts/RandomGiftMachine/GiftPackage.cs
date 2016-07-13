using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using Spine.Unity;

public class GiftPackage : MonoBehaviour
{
    public event Action<GiftPackage> GiftSelected;

    Button _button;

    ParticleSystem _particle;

    SkeletonGraphic _animation;

    CanvasRenderer _renderer;

    string _animColor;

    Vector3 _zoomScale = new Vector3(0.6f, 0.6f, 0.6f);
    Vector3 _lowerScale = new Vector3(0.4f, 0.4f, 0.4f);
    Vector3 _defaultScale;

    void Awake()
    {
        _defaultScale = transform.localScale;
        _particle = transform.FindChild("GiftExplosion").GetComponent<ParticleSystem>();
        _animation = GetComponent<SkeletonGraphic>();
        gameObject.SetActive(false);
        _button = GetComponent<Button>();
        _button.interactable = true;
        _renderer = GetComponent<CanvasRenderer>();
    }

    void Animation_state_End(Spine.AnimationState state, int trackIndex)
    {
        _particle.Play();
    }

    public void SetColor(RandomGiftMachineManager.RandomGiftColor color)
    {
        gameObject.SetActive(true);
        switch (color)
        {
            case RandomGiftMachineManager.RandomGiftColor.blue:
                _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(new Color32(4, 48, 246, 255));
                break;
            case RandomGiftMachineManager.RandomGiftColor.cyan:
                _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(new Color32(4, 246, 198, 255));
                break;
            case RandomGiftMachineManager.RandomGiftColor.green:
                _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(new Color32(8, 251, 1, 255));
                break;
            case RandomGiftMachineManager.RandomGiftColor.purple:
                _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(new Color32(247, 0, 255, 255));
                break;
            case RandomGiftMachineManager.RandomGiftColor.red:
                _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(new Color32(255, 0, 32, 255));
                break;
        }


    }

    void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.5f, 5, 0.6f);
    }

    public void SelectGift()
    {
        if (GiftSelected != null)
            GiftSelected(this);
        transform.DOScale(_zoomScale, 0.5f)
            .SetEase(Ease.OutCubic);
        _animation.AnimationState.SetAnimation(0, "blow_blue", false);
        _animation.AnimationState.End += Animation_state_End;
        StartCoroutine(UnlockRandomCar());
    }

    public void OtherGiftSelected()
    {
        _button.interactable = false;
        transform.DOScale(_lowerScale, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
                gameObject.SetActive(false));
    }

    IEnumerator UnlockRandomCar()
    {
        yield return new WaitForSeconds(1);
        PlayerProfile.Instance.UnlockCar(Utilities.GetRandomEnum<CarTypeEnum>());
    }
}
