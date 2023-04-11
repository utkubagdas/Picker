using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Property
    private PlayerFacade _playerFacade;
    public PlayerFacade PlayerFacade => _playerFacade == null ? _playerFacade = GetComponent<PlayerFacade>() : _playerFacade;
    #endregion

    #region Local
    private readonly Vector3 _propellerRot = new Vector3(-90,0,360);
    private bool _tweenActive;
    #endregion
    
    

    private void OnTriggerEnter(Collider other)
    {
        DropAreaController dropAreaController = other.GetComponent<DropAreaController>();

        if (dropAreaController != null)
        {
            TurnOffThePropellers();
            PlayerFacade.PlayerMovementController.SetControlable(false);
            int layerIndex = LayerMask.NameToLayer("DroppedCollectable");
            dropAreaController.IncreaseDroppedCountText(PlayerFacade.Collector.Collectables.Count);
            foreach (var collectable in PlayerFacade.Collector.Collectables)
            {
                collectable.gameObject.layer = layerIndex;
                collectable.GetComponent<Rigidbody>().AddForce(Vector3.forward * 10, ForceMode.Impulse);
                
            }
            dropAreaController.SetDroppedCount(PlayerFacade.Collector.Collectables.Count);
            PlayerFacade.Collector.Collectables.Clear();
            if (dropAreaController.GetStatus())
            {
                this.Run(0.5f, () => dropAreaController.DestroyDroppedItems());
                this.Run(1.5f, () => dropAreaController.NewRoadMovement());
            }
            else
            {
                EventManager.LevelFailEvent.Invoke();
            }
        }

        if (other.CompareTag(Consts.Tags.FINISHLINE))
        {
            PlayerFacade.PlayerMovementController.SetControlable(false);
            transform.DOMove(ControllerHub.Get<LevelController>().NextLevelFacade.PlayerSpawnPoint.transform.position, 1.5f).OnComplete(
                () =>
                {
                    EventManager.LevelSuccessEvent.Invoke();
                });
        }
    }

    public void TurnOnThePropellers()
    {
        PlayerFacade.Propeller1.SetActive(true);
        PlayerFacade.Propeller2.SetActive(true);
        if (!_tweenActive)
        {
            PlayerFacade.Propeller1.transform.DOLocalRotate(_propellerRot, 0.25f).SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
            PlayerFacade.Propeller2.transform.DOLocalRotate(_propellerRot, 0.25f).SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear);
            _tweenActive = true;
        }
    }

    public void TurnOffThePropellers()
    {
        PlayerFacade.Propeller1.SetActive(false);
        PlayerFacade.Propeller2.SetActive(false);
    }
}
