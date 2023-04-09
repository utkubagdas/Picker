using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DropAreaController : MonoBehaviour
{
   [SerializeField] private TextMeshPro DroppedCountText;
   [SerializeField] private TextMeshPro DesiredDropCountText;
   [SerializeField] private int DroppedCount;
   [SerializeField] private int DesiredCount;
   public GameObject RoadMesh;
   public GameObject Gate1;
   public GameObject Gate2;

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
}
