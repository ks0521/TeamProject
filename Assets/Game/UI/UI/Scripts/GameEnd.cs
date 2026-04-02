using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [SerializeField] Button okBtn;
    [SerializeField] Button noBtn;

    private void OnEnable()
    {
        okBtn.onClick.AddListener(() => Application.Quit());
        noBtn.onClick.AddListener(() => Destroy(gameObject));
    }
    private void OnDisable()
    {
        okBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }
}
