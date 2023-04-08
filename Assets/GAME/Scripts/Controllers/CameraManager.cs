using Cinemachine;
using UnityEngine;

public class CameraManager : BaseController
{
    #region Public
    public CinemachineVirtualCamera gameplayCam;
    public CinemachineVirtualCamera successCam;
    public CinemachineVirtualCamera failCam;
    #endregion

    #region Local
    private LevelController _levelController;
    private Transform _targetTransform;
    private float _shakeTimer;
    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
    #endregion

    private void OnEnable()
    {
        EventManager.LevelLoadedEvent.AddListener(SelectGameplayCam);
        EventManager.LevelRedesignEvent.AddListener(SelectGameplayCam);
        EventManager.LevelSuccessEvent.AddListener(SelectSuccessCam);
        EventManager.LevelFailEvent.AddListener(SelectFailCam);
    }

    private void OnDisable()
    {
        EventManager.LevelLoadedEvent.RemoveListener(SelectGameplayCam);
        EventManager.LevelRedesignEvent.RemoveListener(SelectGameplayCam);
        EventManager.LevelSuccessEvent.RemoveListener(SelectSuccessCam);
        EventManager.LevelFailEvent.RemoveListener(SelectFailCam);
    }

    private void SelectGameplayCam(LevelLoadedEventData arg0)
    {
        SelectGameplayCam();
    }

    public void Init(Transform target)
    {
        _targetTransform = target;

        var transform1 = _targetTransform.transform;
        successCam.Follow = transform1;
        //successCam.LookAt = transform1;
        gameplayCam.Follow = transform1;
        failCam.Follow = transform1;
        failCam.LookAt = transform1;

        SelectGameplayCam();
    }

    public void SelectGameplayCam()
    {
        failCam.Priority = 10;
        successCam.Priority = 10;
        gameplayCam.Priority = successCam.Priority + 1;
        

    }

    public void SelectFailCam()
    {
        gameplayCam.Priority = 10;
        successCam.Priority = 10;
        failCam.Priority = gameplayCam.Priority + 1;
    }

    public void SelectSuccessCam()
    {
        gameplayCam.Priority = 10;
        failCam.Priority = 10;
        successCam.Priority = gameplayCam.Priority + 1;
    }
}
