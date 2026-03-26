using Base.Data;
using Base.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//유저의 조작으로 발생하는 사운드 이벤트를 담당하는 파일
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
            hub.PopupOpened();
        }
    }
    public void SendPopupCloseSignal()
    {
        var hub = GameManager.Instance.GetGameSystem<EventHub>();
        if (hub != null)
        {
            hub.PopupClosed();
        }
    }
}
