using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessagingSystem : MonoBehaviour
{
    static MessagingSystem _instance;

    public static MessagingSystem Instance { get { return _instance; } }

    public QuestMessageUIController QuestUIController;
    public Image PlayerMessageBGImageLeft, PlayerMessageBGImageRight;
    public Text PlayerMessageText;
    public int MessageTransitionDuration;
    public float MessageWaitDuration;

    public List<Text> GeneralMessageContainerList;

    int _lastUsedContainerIndex;

    IEnumerator _messageRoutine;

    void Awake()
    {
        _instance = this;

        ResetPlayerMessage();
    }

    public void WriteKillMessage(CombatCarScript destroyedCar, CombatCarScript destroyerCar)
    {
        if (destroyedCar.IsPlayerCar)
            SendMessageToPlayer("Killed by" + destroyerCar.Username.text, Color.red);
        else if (destroyerCar.IsPlayerCar)
            SendMessageToPlayer("You killed " + destroyedCar.Username.text, Color.green);
        else
            WriteToGeneralMessage(destroyedCar.Username.text + " killed by " + destroyerCar.Username.text);
    }

    public void WriteLeaveMessage(CarBase targetCar)
    {
        if (targetCar.EnteredAbandonArea)
            SendMessageToPlayer("Area Abandoned", Color.red);
        else if (targetCar.EnteredWarningArea)
            SendMessageToPlayer("You are abandoning area!", Color.yellow);
    }

    public void WriteQuestMessage()
    {
        QuestUIController.StartQuestStatusRoutine();
    }

    void WriteToGeneralMessage(string message)
    {
        Text targetContainer = GeneralMessageContainerList.Find(t => !t.gameObject.activeInHierarchy);

        if (targetContainer == null)
        {
            if (_lastUsedContainerIndex != GeneralMessageContainerList.Count - 1)
                targetContainer = GeneralMessageContainerList[_lastUsedContainerIndex + 1];
            else
                targetContainer = GeneralMessageContainerList[0];
        }

        _lastUsedContainerIndex = GeneralMessageContainerList.IndexOf(targetContainer);

        ActivateGeneralMessageContainer(targetContainer, message);
    }

    void SendMessageToPlayer(string message, Color textColor)
    {
        if (_messageRoutine != null)
            StopCoroutine(_messageRoutine);

        _messageRoutine = MessageRoutine(message, textColor);

        if (gameObject.activeSelf)
            StartCoroutine(_messageRoutine);
    }

    IEnumerator MessageRoutine(string message, Color textColor)
    {
        PlayerMessageText.text = message;
        PlayerMessageText.color = textColor;

        float _time = 0f;
        Color startingColor = PlayerMessageBGImageLeft.color;

        while (_time < MessageTransitionDuration)
        {
            Color newColor = Color.Lerp(startingColor, new Color(1f, 1f, 1f, 1f), _time / MessageTransitionDuration);
            
            PlayerMessageBGImageLeft.color = newColor;
            PlayerMessageBGImageRight.color = newColor;
            PlayerMessageText.color = new Color(PlayerMessageText.color.r, PlayerMessageText.color.g, PlayerMessageText.color.b, newColor.a);

            yield return new WaitForFixedUpdate();
            _time += Time.fixedDeltaTime;
        }
        
        PlayerMessageBGImageLeft.color = new Color(1f, 1f, 1f, 1f);
        PlayerMessageBGImageRight.color = new Color(1f, 1f, 1f, 1f);
        PlayerMessageText.color = new Color(PlayerMessageText.color.r, PlayerMessageText.color.g, PlayerMessageText.color.b, 1f);

        yield return new WaitForSeconds(MessageWaitDuration);

        _time = 0f;
        startingColor = PlayerMessageBGImageLeft.color;

        while (_time < MessageTransitionDuration)
        {
            Color newColor = Color.Lerp(startingColor, new Color(1f, 1f, 1f, 0f), _time / MessageTransitionDuration);
            
            PlayerMessageBGImageLeft.color = newColor;
            PlayerMessageBGImageRight.color = newColor;
            PlayerMessageText.color = new Color(PlayerMessageText.color.r, PlayerMessageText.color.g, PlayerMessageText.color.b, newColor.a);

            yield return new WaitForFixedUpdate();
            _time += Time.fixedDeltaTime;
        }

        ResetPlayerMessage();
    }

    public void ResetPlayerMessage()
    {
        PlayerMessageBGImageLeft.color = new Color(1f, 1f, 1f, 0f);
        PlayerMessageBGImageRight.color = new Color(1f, 1f, 1f, 0f);
        PlayerMessageText.color = new Color(PlayerMessageText.color.r, PlayerMessageText.color.g, PlayerMessageText.color.b, 0f);
        PlayerMessageText.text = "";

        GeneralMessageContainerList.ForEach(c => DeactivateGeneralMessageContainer(c));
    }

    void DeactivateGeneralMessageContainer(Text textObject)
    {
        textObject.text = "";
        textObject.transform.parent.gameObject.SetActive(false);
    }

    void ActivateGeneralMessageContainer(Text targetContainer, string message)
    {
        targetContainer.text = message;
        targetContainer.transform.parent.gameObject.SetActive(true);
    }
}
