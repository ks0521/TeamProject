using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePop : MonoBehaviour
{
    public virtual void OpenPop()
    {
        gameObject.SetActive(true);
    }
    public virtual void ClosePop()
    {
        gameObject.SetActive(false);
    }
}
