using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;

public class GiftPackage : MonoBehaviour
{
    public List<ParticleSystem> Splats;

    public event Action<GiftPackage> GiftSelected;

    Button _button;

    ParticleSystem _particle;

    SkeletonGraphic _animation;

    CanvasRenderer _renderer;

    Vector3 _zoomScale = new Vector3(0.75f, 0.75f, 0.75f);
    Vector3 _lowerScale = new Vector3(0.55f, 0.55f, 0.55f);
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
        Color color32;
        switch (color)
        {
            case RandomGiftMachineManager.RandomGiftColor.blue:
                color32 = new Color32(4, 48, 246, 255);
                break;
            case RandomGiftMachineManager.RandomGiftColor.cyan:
                color32 = new Color32(4, 246, 198, 255);
                break;
            case RandomGiftMachineManager.RandomGiftColor.green:
                color32 = new Color32(8, 251, 1, 255);
                break;
            case RandomGiftMachineManager.RandomGiftColor.purple:
                color32 = new Color32(247, 0, 255, 255);
                break;
            case RandomGiftMachineManager.RandomGiftColor.red:
                color32 = new Color32(255, 0, 32, 255);
                break;
            default :
                color32 = Color.white;
                break;
        }
        _animation.Skeleton.FindSlot("racerio_car_bag").SetColor(color32);
        Splats.ForEach(x => x.startColor = color32);


    }

    void OnEnable()
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.5f, 5, 0.6f);
    }

    public void SelectGift()
    {
        _button.interactable = false;
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
