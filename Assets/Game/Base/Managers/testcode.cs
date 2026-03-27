using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A : MonoBehaviour
{
    public string value;
    void Awake() => value = FindFirstObjectByType<B>().value;
}
public class B : MonoBehaviour
{
    public string value;
    void Awake() => value = FindFirstObjectByType<C>().value;
}
public class C : MonoBehaviour
{
    public string value;
    void Awake() => value = FindFirstObjectByType<A>().value;
}
