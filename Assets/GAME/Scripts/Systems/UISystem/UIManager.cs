using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : PersistentSingleton<UIManager>
{
    [SerializeField] private List<BaseUI> uiList;

    private readonly Dictionary<Type, BaseUI> uis = new Dictionary<Type, BaseUI>();

    protected override void Awake()
    {
        base.Awake();
        PopulateDictionary();
    }

    private void PopulateDictionary()
    {
        foreach (BaseUI ui in uiList)
        {
            uis.Add(ui.GetType(), ui);
        }
    }

    public static T Get<T>() where T : BaseUI
    {
        return (T)Instance.uis[typeof(T)];
    }

}