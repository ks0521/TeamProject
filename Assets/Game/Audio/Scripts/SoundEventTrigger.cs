using Base.Data;
using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEventTrigger : MonoBehaviour
{
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void SendClickSignal()
    {
        var hub = GameManager.Instance.GetGameSystem<EventHub>();
        if(hub != null)
        {
            hub.ButtonClicked(); //OnButtonClicked?.Invoke() 트리거용
        }
    }
    public void SendPopupOpenSignal()
    {
        var hub = GameManager.Instance.GetGameSystem<EventHub>();
        if (hub != null)
        {
            hub.PopupOpen();
        }
    }
}
