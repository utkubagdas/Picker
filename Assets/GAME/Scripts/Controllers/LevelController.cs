using UnityEngine;

public class LevelController : BaseLevelController
{
    #region Property
    // public PlayerController PlayerController => _playerController;
    // public VariableJoystick Joystick => joystick;
    public LevelFacade LevelFacade => _levelFacade;
    #endregion

    #region Serialized
    // [SerializeField] private PlayerController player;
    // [SerializeField] private VariableJoystick joystick;
    #endregion

    #region Local
    private GameObject _player;
    //private PlayerController _playerController;
    private LevelFacade _levelFacade;
    #endregion

    #region Static
    public static bool LevelSuccess;
    public static bool LevelFail;
    public static bool Tutorial;
    #endregion

    protected override void LoadLevel()
    {
        base.LoadLevel();
        PrepareLevel();
    }
	
    private void PrepareLevel()
    {
        _levelFacade = InstantiateAsDestroyable<LevelFacade>(LevelContent.LevelFacade);
        ResetStaticVariables();
        _player = _levelFacade.Player;
            //this is the place where you should add your in-game logic such as instantiating player etc.
         Transform target = _player.transform;
         if (target != null)
         {
             ControllerHub.Get<CameraManager>().Init(target);
         }
        EventManager.LevelStartEvent.Invoke();
        SendLevelLoadedEvent(_levelFacade);
    }
	
    private void ResetStaticVariables()
    {
        LevelSuccess = false;
        LevelFail = false;
    }
}