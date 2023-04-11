using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DropAreaController : MonoBehaviour
{
   #region Serialized
   [SerializeField] private TextMeshPro DroppedCountText;
   [SerializeField] private TextMeshPro DesiredDropCountText;
   [SerializeField] private int DroppedCount;
   [SerializeField] private int DesiredCount;
   [SerializeField] private GameObject ConfettiParticle;
   [SerializeField] private Transform ParticleSpawnPoint;
   #endregion
   
   #region Public
   public GameObject RoadMesh;
   public GameObject Gate1;
   public GameObject Gate2;
   #endregion
   
   #region Local
   private readonly List<Collectable> _droppedItem = new List<Collectable>();
   #endregion

   public void SetDesiredDropCountText(int count)
   {
      DesiredDropCountText.SetText(count.ToString());
   }

   public void IncreaseDroppedCountText(int increaseAmount)
   {
      int currentNumber = Convert.ToInt32(DroppedCountText.text);
      DOTween.To(() => currentNumber, x => currentNumber = x, increaseAmount, 0.25f)
         .OnUpdate(() => {
            DroppedCountText.SetText(currentNumber.ToString());
         });
   }

   public void SetDroppedCount(int dropped)
   {
      DroppedCount += dropped;
   }

   public void SetDesiredCount(int desired)
   {
      DesiredCount = desired;
   }

   public bool GetStatus()
   {
      if (DroppedCount >= DesiredCount)
      {
         return true;
      }
      else
      {
         return false;
      }
   }
   
   public void NewRoadMovement()
   {
      RoadMesh.transform.DOLocalMoveY(0.208f, 0.75f).SetEase(Ease.InOutBack).OnComplete(() =>
      {
         Sequence seq = DOTween.Sequence();
         seq.Join(Gate1.transform.DORotate(new Vector3(0, 0, 70), 0.25f).SetEase(Ease.Linear));
         seq.Join(Gate2.transform.DORotate(new Vector3(0, 0, -70), 0.25f).SetEase(Ease.Linear)).OnComplete(() =>
         {
            var particle = Instantiate(ConfettiParticle);
            particle.transform.position = ParticleSpawnPoint.position;
            EventManager.PassedDropArea.Invoke();
         });
      });
   }

   public void DestroyDroppedItems()
   {
      StartCoroutine(DestroyDroppedItemsCo());
   }

   private IEnumerator DestroyDroppedItemsCo()
   {
      for (int i = 0; i < _droppedItem.Count; i++)
      {
         var particle = Instantiate(_droppedItem[i].DestroyParticle);
         particle.transform.position = _droppedItem[i].transform.position;
         var droppedItem = _droppedItem[i];
         this.Run(0.01f, () => Destroy(droppedItem.gameObject));
            yield return new WaitForSeconds(1f / _droppedItem.Count);
      }
      _droppedItem.Clear();
   }

   private void OnTriggerEnter(Collider other)
   {
      Collectable collectable = other.GetComponent<Collectable>();

      if (collectable != null)
      {
         if (!collectable.IsCollected)
            return;
         
         _droppedItem.Add(collectable);
      }
   }
}
