using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public class BaseLevelController : BaseController
{

    #region Levels
    [InfoBox("Extend this class according to your level controller.", InfoMessageType.Info,
        "IsBaseLevelController")]
    [SerializeField, FoldoutGroup("Levels")]
    protected List<LevelContent> allLevels = new List<LevelContent>();
    [SerializeField, FoldoutGroup("Levels")]
    protected List<LevelContent> levelsToRepeat = new List<LevelContent>();
    #endregion

    #region Debug
    [FoldoutGroup("Debug"), Tooltip("Works on build!"), GUIColor("GetDebugModeColor"), HideInInspector]
    public bool DebugMode;
    [FoldoutGroup("Debug"), PropertyOrder(100), InlineEditor, GUIColor("GetDebugModeColor")]
    public LevelContent LevelContent;
    [FoldoutGroup("Debug"), PropertyOrder(100), InlineEditor, GUIColor("GetDebugModeColor")]
    public LevelContent LevelContent2;
    [FoldoutGroup("Debug"), PropertyOrder(100), ReadOnly, GUIColor("GetDebugModeColor")]
    public int LevelNo;

    public bool OneTourCompleted;
    #endregion

    [SerializeField]
    private Transform _levelParent;

    public List<Object> _destroyOnResetList = new List<Object>();

    #region ODIN
#if UNITY_EDITOR
    private Color GetDebugModeColor()
    {
        return DebugMode ? Color.green : Color.white;
    }

    [FoldoutGroup("Debug"), HideIf("DebugMode"), Button("Activate Debug Mode",ButtonSizes.Large)]
    private void ActivateDebugMode()
    {
        DebugMode = !DebugMode;
        DebugController debugPrefab = Resources.Load<DebugController>("DebugController");
        DebugController debugController = Instantiate(debugPrefab);
        debugController.gameObject.hideFlags = HideFlags.HideInHierarchy;
    }
    
    [FoldoutGroup("Debug"), ShowIf("DebugMode"), Button("Disable Debug Mode",ButtonSizes.Large), GUIColor(1, 0, 0)]
    private void DisableDebugMode()
    {
        DebugMode = !DebugMode;
        GameObject[] debugObjects = GameObject.FindGameObjectsWithTag("Debug");
        foreach (GameObject debugObject in debugObjects)
        {
            DestroyImmediate(debugObject);
        }
    }
    
    private bool IsBaseLevelController()
    {
        return GetType() == typeof(BaseLevelController);
    }
#endif
    #endregion

    protected virtual void OnEnable()
    {
        EventManager.LevelSuccessEvent.AddListener(OnLevelSuccess);
    }

    protected virtual void OnDisable()
    {
        EventManager.LevelSuccessEvent.RemoveListener(OnLevelSuccess);
    }

    public override void Init()
    {
        base.Init();
        
        LoadLevel();
    }

    protected virtual void LoadLevel()
    {
        if (!DebugMode)
        {
            LevelContent = GetLevelContent();
            LevelContent2 = GetNextLevelContent();
        }
        UpdateLightingSettings();
    }

    protected virtual LevelContent GetLevelContent()
    {
        LevelSaveManager levelSaveManager = ControllerHub.Get<DataManager>().LevelSaveManager;
        LevelNo = levelSaveManager.CurrentLevelNo;
        int currentLevelIndex = levelSaveManager.CurrentLevelIndex;
        if (currentLevelIndex < allLevels.Count)
        {
            return allLevels[currentLevelIndex];
        }
        levelSaveManager.SetCurrentLevelIndex(0);
        return allLevels[0];
        //return levelsToRepeat[currentLevelIndex];
    }
    
    protected virtual LevelContent GetNextLevelContent()
    {
        LevelSaveManager levelSaveManager = ControllerHub.Get<DataManager>().LevelSaveManager;
        LevelNo = levelSaveManager.CurrentLevelNo;
        int currentLevelIndex = levelSaveManager.CurrentLevelIndex;
        if (!OneTourCompleted)
        {
            if (currentLevelIndex + 1 < allLevels.Count)
            {
                return allLevels[currentLevelIndex + 1];
            }
        }
        else
        {
            if (currentLevelIndex < allLevels.Count)
            {
                Debug.Log(currentLevelIndex);
                return allLevels[currentLevelIndex];
            }
        }

        if (!OneTourCompleted)
        {
            OneTourCompleted = true;
        }
        levelSaveManager.SetCurrentLevelIndex(0);
        return allLevels[0];
    }

    /// <summary>
    /// Increases and saves LevelNo.
    /// </summary>
    protected virtual void IncreaseLevelNo()
    {
        LevelSaveManager levelSaveManager = ControllerHub.Get<DataManager>().LevelSaveManager;
        if (levelSaveManager.CurrentLevelIndex >= allLevels.Count)
        {
            // List<int> loop = levelSaveManager.LevelIndicesToRepeat;
            // if (loop.Count == 0)
            // {
            //     loop = Enumerable.Range(0, levelsToRepeat.Count).ToList();
            //     loop.Shuffle();
            // }
            // levelSaveManager.SetCurrentLevelIndex(loop[0], false);
            // loop.RemoveAt(0);
            // levelSaveManager.SetLevelIndicesToRepeat(loop, false);
            
            levelSaveManager.SetCurrentLevelIndex(0);
        }
        else
        {
            levelSaveManager.SetCurrentLevelIndex(levelSaveManager.CurrentLevelIndex + 1, false);
        }
        levelSaveManager.SetCurrentLevelNo(levelSaveManager.CurrentLevelNo + 1, false);
        levelSaveManager.SaveData();
    }

    protected virtual void DecreaseLevelNo()
    {
        LevelSaveManager levelSaveManager = ControllerHub.Get<DataManager>().LevelSaveManager;
        if (levelSaveManager.CurrentLevelIndex - 1 <= 0)
        {

            return;
        }
        levelSaveManager.SetCurrentLevelIndex(levelSaveManager.CurrentLevelIndex + - 1, false);
        levelSaveManager.SaveData();
    }

    protected virtual void LevelContinue()
    {
        IncreaseLevelNo();
    }

    /// <summary>
    /// This method collects all the Objects(GameObjects and Components) instantiated in the game so that they can be destroyed when ResetLevel is called.
    /// </summary>
    /// <returns>Return the initiated object.</returns>
    protected T InstantiateAsDestroyable<T>(Object obj) where T : Object
    {
        T t = Instantiate(obj, _levelParent) as T;
        _destroyOnResetList.Add(t);
        return t;
    }
    
    protected virtual void OnLevelSuccess()
    {
        IncreaseLevelNo();
    }
 
    protected void SendLevelLoadedEvent(LevelFacade facade)
    {
        EventManager.LevelLoadedEvent.Invoke(new LevelLoadedEventData(
            LevelContent,
            facade,
            LevelNo));
    }
    
    /// <summary>
    /// Resets the current level and loads the new one.
    /// If level success is received, it will load the next level.
    /// </summary>
    [FoldoutGroup("Debug"), Button]
    public virtual void RestartLevel()
    {
        ResetLevel();
        EventManager.LevelResetEvent.Invoke();
        LoadLevel();
    }
    
    protected virtual void ResetLevel()
    {
        foreach (Object obj in _destroyOnResetList)
        {
            switch (obj)
            {
                case GameObject go:
                    Destroy(go);
                    break;
                case Component component:
                    Destroy(component.gameObject);
                    break;
            }
        }
        _destroyOnResetList.Clear();
    }
    
    private void UpdateLightingSettings()
    {
        //Environment
        RenderSettings.skybox = LevelContent.LightingSettings.SkyboxMaterial;
        RenderSettings.sun = LevelContent.LightingSettings.SunSource;
        
        //Environment Lighting
        RenderSettings.ambientMode = LevelContent.LightingSettings.LightingSource;
        switch (LevelContent.LightingSettings.LightingSource)
        {
            case AmbientMode.Skybox:
                RenderSettings.ambientIntensity = LevelContent.LightingSettings.LightingIntensityMultiplier;
                break;
            case AmbientMode.Trilight:
                RenderSettings.ambientSkyColor = LevelContent.LightingSettings.SkyColor;
                RenderSettings.ambientEquatorColor = LevelContent.LightingSettings.EquatorColor;
                RenderSettings.ambientGroundColor = LevelContent.LightingSettings.GroundColor;
                break;
            case AmbientMode.Flat:
                RenderSettings.ambientLight = LevelContent.LightingSettings.AmbientColor;
                break;
        }
        
        //Environment Reflections
        RenderSettings.defaultReflectionMode = LevelContent.LightingSettings.ReflectionSource;
        switch (LevelContent.LightingSettings.ReflectionSource)
        {
            case DefaultReflectionMode.Skybox:
                RenderSettings.defaultReflectionResolution = LevelContent.LightingSettings.Resolution;
                break;
            case DefaultReflectionMode.Custom:
                RenderSettings.customReflection = LevelContent.LightingSettings.Cubemap;
                break;
        }
        RenderSettings.reflectionIntensity = LevelContent.LightingSettings.ReflectionIntensityMultiplier;
        RenderSettings.reflectionBounces = LevelContent.LightingSettings.Bounces;
        
        //Other Settings
        RenderSettings.fog = LevelContent.LightingSettings.Fog;
        if (LevelContent.LightingSettings.Fog)
        {
            RenderSettings.fogColor = LevelContent.LightingSettings.FogColor;
            RenderSettings.fogMode = LevelContent.LightingSettings.Mode;
            switch (LevelContent.LightingSettings.Mode)
            {
                case FogMode.Linear:
                    RenderSettings.fogStartDistance = LevelContent.LightingSettings.Start;
                    RenderSettings.fogEndDistance = LevelContent.LightingSettings.End;
                    break;
                case FogMode.Exponential:
                case FogMode.ExponentialSquared:
                    RenderSettings.fogDensity = LevelContent.LightingSettings.Density;
                    break;
            }
        }
    }

    public void AddLevelFromLevelEditor(LevelContent levelContent)
    {
        allLevels.Add(levelContent);
    }

    public void AddRepeatLevelFromLevelEditor(LevelContent levelContent)
    {
        levelsToRepeat.Add(levelContent);
    }

    public int GetTotalLevelIndex()
    {
        return allLevels.Count - 1;
    }

    public int GetCurrentLevelIndex()
    {
        return ControllerHub.Get<DataManager>().LevelSaveManager.CurrentLevelIndex;
    }

    public int GetTotalLevelCount()
    {
        return allLevels.Count;
    }

    public LevelFacade GetLevelFromIndex(int index)
    {
        return allLevels[index].LevelFacade;
    }

    public void RemoveLevelFromEditor(int index)
    {
        if (levelsToRepeat.Contains(allLevels[index]))
        {
            levelsToRepeat.Remove(allLevels[index]);
        }
        
        var tempLevel = allLevels[index];
        allLevels.RemoveAt(index);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(tempLevel));
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(tempLevel.LevelFacade));
    }
    public List<string> GetLevelsName()
    {
        List<string> levelNameList = new List<string>();
        foreach (var level in allLevels)
        {
            levelNameList.Add(level.name);
        }

        return levelNameList;
    }

    public List<LevelContent> GetLevelList()
    {
        return allLevels;
    }
    
}
