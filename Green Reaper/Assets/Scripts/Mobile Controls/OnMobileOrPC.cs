using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnMobileOrPC : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnMobile;
    [SerializeField]
    private UnityEvent OnPC;

    void Awake()
    {
        #if UNITY_ANDROID || UNITY_IOS
        OnMobile?.Invoke();
        #else
        OnPC?.Invoke();
        #endif
    }
}
