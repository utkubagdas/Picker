using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LevelController : BaseLevelController
{
    #region Property
    public LevelFacade LevelFacade => _levelFacade;
    public LevelFacade NextLevelFacade => _nextLevelFacade;
    public LevelFacade OldLevelFacade => _oldLevelFacade;
    #endregion
    

    #region Local
    private GameObject _player;
    private LevelFacade _levelFacade;
    private LevelFacade _nextLevelFacade;
    private LevelFacade _oldLevelFacade;
    private readonly List<LevelFacade> _createdLevelList = new List<LevelFacade>();
    #endregion

    #region Static
    public static bool LevelSuccess;
    public static bool LevelFail;
    public static bool LevelStarted;
    public static float LevelRoadsSizeZ;
    #endregion

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.LevelSuccessEvent.AddListener(() => LevelStarted = false);
        EventManager.LevelFailEvent.AddListener(ResetStaticVariables);
        EventManager.LevelResetEvent.AddListener(ResetStaticVariables);
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.LevelSuccessEvent.RemoveListener(() => LevelStarted = false);
        EventManager.LevelFailEvent.RemoveListener(ResetStaticVariables);
        EventManager.LevelResetEvent.RemoveListener(ResetStaticVariables);
    }

    protected override void LoadLevel()
    {
        base.LoadLevel();
        PrepareLevel();
    }
	
    private void PrepareLevel()
    {
        if(_player != null)
            Destroy(_player);
        _levelFacade = InstantiateAsDestroyable<LevelFacade>(LevelContent.LevelFacade);
        _nextLevelFacade = InstantiateAsDestroyable<LevelFacade>(LevelContent2.LevelFacade);
        _nextLevelFacade.gameObject.transform.position = Vector3.forward * LevelContent.LevelSizeZ;
        _player = PrefabUtility.InstantiatePrefab(
                 AssetDatabase.LoadAssetAtPath<GameObject>(Consts.LevelEditorSettings.PLAYERPREFABPATH)) as GameObject;
        _player.transform.position = _levelFacade.PlayerSpawnPoint.transform.position;
        ResetStaticVariables();
        //this is the place where you should add your in-game logic such as instantiating player etc.
         Transform target = _player.transform;
         if (target != null)
         {
             ControllerHub.Get<CameraManager>().Init(target);
         }
         SendLevelLoadedEvent(_levelFacade);
         _createdLevelList.Add(_levelFacade);
         _createdLevelList.Add(_nextLevelFacade);
    }
    

    public void LevelContinueButton()
    {
        _oldLevelFacade = _nextLevelFacade;
        Vector3 oldPos = _nextLevelFacade.gameObject.transform.position;
        LevelContent2 = GetNextLevelContent();
        float levelRoadsSizeZ = LevelContent2.LevelSizeZ;
        _nextLevelFacade = InstantiateAsDestroyable<LevelFacade>(LevelContent2.LevelFacade);
        _createdLevelList.Add(_nextLevelFacade);
        var beforeLevel = _createdLevelList[0];
        _createdLevelList.RemoveAt(0);
        _destroyOnResetList.Remove(beforeLevel);
        Destroy(beforeLevel.gameObject);
        _nextLevelFacade.transform.position = oldPos + Vector3.forward * levelRoadsSizeZ;
        EventManager.LevelContinueEvent.Invoke();

    }

    private void ResetStaticVariables()
    {
        LevelSuccess = false;
        LevelFail = false;
        LevelStarted = false;
        _createdLevelList.Clear();
    }
}