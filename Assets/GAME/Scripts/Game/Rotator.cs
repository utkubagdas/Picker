using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private Vector3 _rot = new Vector3(0, 0, -180);
    [SerializeField] private GameObject RotObject;
    private void Start()
    {
        RotateObject();
    }
    
    private void RotateObject()
    {
        RotObject.transform.DOLocalRotate(_rot,1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
}
