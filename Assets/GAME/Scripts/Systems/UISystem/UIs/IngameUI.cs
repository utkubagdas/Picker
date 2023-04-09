using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : BaseUI
{
    [SerializeField] private GameObject TutorialHand;
    [SerializeField] private TextMeshProUGUI CurrentLevelText;
    [SerializeField] private TextMeshProUGUI NextLevelText;
    [SerializeField] private Image Slot1Image;
    [SerializeField] private Image Slot2Image;
    [SerializeField] private Image Slot3Image;
    private int _passCount;
    private void OnEnable()
    {
        EventManager.LevelLoadedEvent.AddListener(OnLevelLoaded);
        EventManager.LevelFailEvent.AddListener(OnLevelFail);
        EventManager.LevelSuccessEvent.AddListener(OnLevelSuccess);
        EventManager.PassedDropArea.AddListener(OnPassDropArea);
    }

    private void OnDisable()
    {
        EventManager.LevelLoadedEvent.RemoveListener(OnLevelLoaded);
        EventManager.LevelFailEvent.RemoveListener(OnLevelFail);
        EventManager.LevelSuccessEvent.RemoveListener(OnLevelSuccess);
        EventManager.PassedDropArea.RemoveListener(OnPassDropArea);
    }

    private void OnLevelLoaded(LevelLoadedEventData eventData)
    {
        SetShow();
        ResetVariables();
        CurrentLevelText.SetText(ControllerHub.Get<LevelController>().LevelNo.ToString());
        NextLevelText.SetText((ControllerHub.Get<LevelController>().LevelNo + 1).ToString());
    }

    private void OnLevelFail()
    {
        SetHidden();
    }

    private void OnLevelSuccess()
    {
        SetHidden();
    }
    
    public void LevelStart()
    {
        if (LevelController.LevelStarted)
            return;
        LevelController.LevelStarted = true;
        EventManager.LevelStartEvent.Invoke();
        TutorialHand.SetActive(false);
    }

    public void ResetVariables()
    {
        TutorialHand.SetActive(true);
        _passCount = 0;
        Slot1Image.color = Color.white;
        Slot2Image.color = Color.white;
        Slot3Image.color = Color.white;
    }

    private void OnPassDropArea()
    {
        if (_passCount == 0)
        {
            Slot1Image.color = new Color32(229, 209, 161, 255);
            _passCount++;
        }
        else if (_passCount == 1)
        {
            Slot2Image.color = new Color32(229, 209, 161, 255);
            _passCount++;
        }
        else if (_passCount == 2)
        {
            Slot3Image.color = new Color32(229, 209, 161, 255);
            _passCount++;
        }
    }
}