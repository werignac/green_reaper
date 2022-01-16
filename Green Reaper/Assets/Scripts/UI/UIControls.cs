using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControls : MonoBehaviour
{
    [SerializeField]
    private string showControlsName = "Show Controls";

    private void Awake()
    {
        if (PlayerPrefs.GetInt(showControlsName, 1) == 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
