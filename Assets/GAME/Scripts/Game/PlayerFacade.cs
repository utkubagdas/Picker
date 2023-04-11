using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacade : MonoBehaviour
{
    #region Property
    private PlayerMovementController _playerMovementController;
    public PlayerMovementController PlayerMovementController => _playerMovementController == null ? _playerMovementController = GetComponent<PlayerMovementController>() : _playerMovementController;

    private Collector _collector;
    public Collector Collector => _collector == null ? _collector = GetComponent<Collector>() : _collector;
    
    private PlayerController _playerController;
    public PlayerController PlayerController => _playerController == null ? _playerController = GetComponent<PlayerController>() : _playerController;
    #endregion

    #region Public
    public GameObject Propeller1;
    public GameObject Propeller2;
    #endregion


}
