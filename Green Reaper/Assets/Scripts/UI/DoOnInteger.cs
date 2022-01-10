using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoOnInteger : MonoBehaviour
{
    [SerializeField]
    private UnityEvent toDo;

    [SerializeField]
    private int toLookFor;

    public void CheckToDo(int toCheck)
    {
        if (toCheck == toLookFor)
            toDo?.Invoke();
    }

}
