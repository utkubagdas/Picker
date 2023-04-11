public class SuccessUI : BaseUI
{
    private void OnEnable()
    {
        EventManager.LevelLoadedEvent.AddListener(OnLevelLoaded);
        EventManager.LevelSuccessEvent.AddListener(OnLevelSuccess);
        EventManager.LevelContinueEvent.AddListener(LevelContinue);
    }

    private void OnDisable()
    {
        EventManager.LevelLoadedEvent.RemoveListener(OnLevelLoaded);
        EventManager.LevelSuccessEvent.RemoveListener(OnLevelSuccess);
        EventManager.LevelContinueEvent.RemoveListener(LevelContinue);
    }

    private void OnLevelLoaded(LevelLoadedEventData eventData)
    {
        SetHidden();
    }

    private void OnLevelSuccess()
    {
        SetShow();
    }

    private void LevelContinue()
    {
        SetHidden();
    }
}