using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PropellerUpgrade : MonoBehaviour, ICollectable
{
    public void Collect(Collector collector)
    {
        DOTween.Kill(transform);
        Destroy(gameObject);
    }
}
