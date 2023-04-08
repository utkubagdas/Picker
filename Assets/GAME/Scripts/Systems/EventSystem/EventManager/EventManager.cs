using UnityEngine.Events;

public static class EventManager
{
    #region GameCycleEvents
    public static readonly UnityEvent LevelStartEvent = new UnityEvent();
    public static readonly UnityEvent StageStartEvent = new UnityEvent();
    public static readonly UnityEvent LevelSuccessEvent = new UnityEvent();
    public static readonly UnityEvent LevelFailEvent = new UnityEvent();
    public static readonly UnityEvent LevelResetEvent = new UnityEvent();
    public static readonly UnityEvent LevelRedesignEvent = new UnityEvent();
    public static readonly LevelLoadedEvent LevelLoadedEvent = new LevelLoadedEvent();
    #endregion
}

public class LevelLoadedEvent : UnityEvent<LevelLoadedEventData> { }