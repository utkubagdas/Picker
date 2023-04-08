using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DropAreaController dropAreaController = other.GetComponent<DropAreaController>();

        if (dropAreaController != null)
        {
            Debug.Log("drop area");
            EventManager.PassedDropArea.Invoke();
        }
    }
}
