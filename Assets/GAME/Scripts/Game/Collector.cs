using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private PlayerFacade _playerFacade;
    public PlayerFacade PlayerFacade => _playerFacade == null ? _playerFacade = GetComponent<PlayerFacade>() : _playerFacade;
    public List<Collectable> Collectables = new List<Collectable>();
    private void OnTriggerEnter(Collider other)
    {
        Collectable collectable = other.GetComponent<Collectable>();
        PropellerUpgrade propellerUpgrade = other.GetComponent<PropellerUpgrade>();

        if (collectable != null)
        {
            Collectables.Add(collectable);
        }

        if (propellerUpgrade != null)
        {
            propellerUpgrade.Collect(this);
            PlayerFacade.PlayerController.TurnOnThePropellers();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Collectable collectable = other.GetComponent<Collectable>();

        if (collectable != null)
        {
            Collectables.Remove(collectable);
        }
    }
}
