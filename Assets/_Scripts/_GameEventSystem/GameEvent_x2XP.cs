using UnityEngine;
using System.Collections;

public class GameEvent_x2XP : GameEventBase
{
    public float XPCoef;

    protected override void StartEvent()
    {
        base.StartEvent();

        ExperienceManager.Instance.SetXPCoef(XPCoef);
    }

    protected override void EndEvent()
    {
        base.EndEvent();

        ExperienceManager.Instance.SetXPCoef(1.0f / XPCoef);
    }
}
