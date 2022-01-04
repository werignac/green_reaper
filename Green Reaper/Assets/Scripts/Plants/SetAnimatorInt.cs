using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SetAnimatorInt : MonoBehaviour
{
    [SerializeField]
    private string intName;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetInt(int value)
    {
        anim.SetInteger(intName, value);
    }
}
