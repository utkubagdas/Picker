using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerUpgrade : MonoBehaviour, ICollectable
{
    public void Collect(Collector collector)
    {
        Destroy(gameObject);
    }
}
