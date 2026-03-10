using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldTest : MonoBehaviour
{
    [Header("°ñµå")]
    [Range(0, 10000)]
    public int testgold;

    public TextMeshProUGUI GoldText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GoldText.text = testgold.ToString();
    }
}
