using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AutoBattleTextUI : MonoBehaviour
{
    public TextMeshProUGUI autoText;

    public bool autoState;
    // Start is called before the first frame update
    void Start()
    {
        autoState = false;
        UpdateText();
    }

    public void OnClick()
    {
        autoState = !autoState;
        UpdateText();
    }

    private void UpdateText()
    {
        autoText.text = autoState ? "Auto On" : "Auto Off";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
