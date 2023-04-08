using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : BaseUI
{
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
        EventManager.LevelRedesignEvent.AddListener(OnLevelRedesign);
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
    }

    private void OnLevelFail()
    {
        SetHidden();
    }

    private void OnLevelSuccess()
    {
        SetHidden();
    }

    private void OnLevelRedesign()
    {
        SetShow();
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