using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerFacade _playerFacade;
    public PlayerFacade PlayerFacade => _playerFacade == null ? _playerFacade = GetComponent<PlayerFacade>() : _playerFacade;

    private Vector3 _propellerRot = new Vector3(-90,0,360);
    

    private void OnTriggerEnter(Collider other)
    {
        DropAreaController dropAreaController = other.GetComponent<DropAreaController>();

        if (dropAreaController != null)
        {
            PlayerFacade.PlayerMovementController.SetControlable(false);
            EventManager.PassedDropArea.Invoke();
            int layerIndex = LayerMask.NameToLayer("DroppedCollectable");
            dropAreaController.IncreaseDroppedCountText(PlayerFacade.Collector.Collectables.Count);
            foreach (var collectable in PlayerFacade.Collector.Collectables)
            {
                collectable.gameObject.layer = layerIndex;
                collectable.GetComponent<Rigidbody>().AddForce(Vector3.forward * 5, ForceMode.Impulse);
                
            }
            dropAreaController.SetDroppedCount(PlayerFacade.Collector.Collectables.Count);
            if (dropAreaController.GetStatus())
            {
                this.Run(1.5f, () => NewRoadMovement(dropAreaController));
            }
            else
            {
                EventManager.LevelFailEvent.Invoke();
            }
        }

        if (other.CompareTag(Consts.Tags.FINISHLINE))
        {
            EventManager.LevelSuccessEvent.Invoke();
        }
    }

    private void NewRoadMovement(DropAreaController dropAreaController)
    {
        dropAreaController.RoadMesh.transform.DOLocalMoveY(0.208f, 0.1f).OnComplete(() =>
        {
            Sequence seq = DOTween.Sequence();
            seq.Join(dropAreaController.Gate1.transform.DORotate(new Vector3(0, 0, 70), 0.25f).SetEase(Ease.Linear));
            seq.Join(dropAreaController.Gate2.transform.DORotate(new Vector3(0, 0, -70), 0.25f).SetEase(Ease.Linear)).OnComplete(() =>
            {
                PlayerFacade.PlayerMovementController.SetControlable(true);  
            });
        });
    }

    public void TurnOnThePropellers()
    {
        PlayerFacade.Propeller1.SetActive(true);
        PlayerFacade.Propeller2.SetActive(true);
        PlayerFacade.Propeller1.transform.DOLocalRotate(_propellerRot, 0.25f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
        PlayerFacade.Propeller2.transform.DOLocalRotate(_propellerRot, 0.25f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

    }
}
