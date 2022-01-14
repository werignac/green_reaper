using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SendToAnimator : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private string toSet;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SendFloatToAnimator(float toSend)
    {
        animator.SetFloat(toSet, toSend);
    }

    public void SendIntToAnimator(int toSend)
    {
        animator.SetInteger(toSet, toSend);
    }
}
