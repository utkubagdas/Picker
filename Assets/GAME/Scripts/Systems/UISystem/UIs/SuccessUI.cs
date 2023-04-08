public class SuccessUI : BaseUI
{
    private void OnEnable()
    {
        EventManager.LevelLoadedEvent.AddListener(OnLevelLoaded);
        EventManager.LevelSuccessEvent.AddListener(OnLevelSuccess);
        EventManager.LevelRedesignEvent.AddListener(OnLevelRedesign);
    }

    private void OnDisable()
    {
        EventManager.LevelLoadedEvent.RemoveListener(OnLevelLoaded);
        EventManager.LevelSuccessEvent.RemoveListener(OnLevelSuccess);
        EventManager.LevelRedesignEvent.RemoveListener(OnLevelRedesign);
    }

    private void OnLevelLoaded(LevelLoadedEventData eventData)
    {
        SetHidden();
    }

    private void OnLevelSuccess()
    {
        SetShow();
    }

    private void OnLevelRedesign()
    {
        SetHidden();
    }
}