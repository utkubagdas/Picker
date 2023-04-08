using System;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHub : PersistentSingleton<ControllerHub>
{
    #region Serialized
    [SerializeField] private List<BaseController> ControllerList;
    #endregion

    public LevelController LevelController;
    
    #region Local
    private readonly Dictionary<Type, BaseController> _controllers = new Dictionary<Type, BaseController>();
    private bool _initializedForEditor;
    #endregion
    
    private void Start()
    {
        PopulateDictionary();
        InitControllers();
        _initializedForEditor = false;
    }

    private void PopulateDictionary()
    {
        foreach (var controller in ControllerList)
        {
            _controllers.Add(controller.GetType(), controller);
        }
    }

    private void InitControllers()
    {
        foreach (var controller in _controllers.Values)
        {
            controller.Init();
        }
    }

    public static T Get<T>() where T : BaseController
    {
        return (T)Instance._controllers[typeof(T)];
    }

    public void InitializeForEditor()
    {
        if (_initializedForEditor)
        {
            return;
        }
        
        _initializedForEditor = true;
        PopulateDictionary();
        InitControllers();
    }
}