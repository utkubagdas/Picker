public class FailUI : BaseUI
{
    private void OnEnable()
    {
        EventManager.LevelLoadedEvent.AddListener(OnLevelLoaded);
        EventManager.LevelFailEvent.AddListener(OnLevelFail);
        EventManager.LevelRedesignEvent.AddListener(OnLevelRedesign);
    }

    private void OnDisable()
    {
        EventManager.LevelLoadedEvent.RemoveListener(OnLevelLoaded);
        EventManager.LevelFailEvent.RemoveListener(OnLevelFail);
        EventManager.LevelRedesignEvent.RemoveListener(OnLevelRedesign);
    }

    private void OnLevelLoaded(LevelLoadedEventData eventData)
    {
        SetHidden();
    }

    private void OnLevelFail()
    {
        SetShow();
    }

    private void OnLevelRedesign()
    {
        SetHidden();
    }
}