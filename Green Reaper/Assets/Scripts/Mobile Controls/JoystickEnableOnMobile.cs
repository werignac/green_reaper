using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickEnableOnMobile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID || UNITY_IOS
            gameObject.SetActive(true);
        #else
            gameObject.SetActive(false);
        #endif
    }
}
