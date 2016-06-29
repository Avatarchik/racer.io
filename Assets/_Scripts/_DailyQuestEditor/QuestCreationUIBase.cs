using UnityEngine;
using System.Collections;

public abstract class QuestCreationUIBase : MonoBehaviour
{
    public DailyQuestType QuestType;

    public abstract void ActivateUI();

    public virtual void DeactivateUI()
    {
        gameObject.SetActive(false);
    }

    public abstract DailyQuestInfo GetQuestInfo(int questID);
}
