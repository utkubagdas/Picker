using System.Collections.Generic;
using UnityEngine;

public class Consts : MonoBehaviour
{
    
    public static readonly Dictionary<int, string> NumberAbbrs = new Dictionary<int, string>
    {
        {1000000000, "B" },
        {1000000, "M" },
        {1000,"K"}
    };

    public struct FileNames
    {
        public const string LEVELDATA = "level.dat";
    }

    public struct PrefKeys
    {
        public const string HAPTIC = "Haptic";
    }
    
    public struct Tags
    {
       
    }

    public struct LevelEditorSettings
    {
        public const int MINDROPCOUNT = 1;
        public const int MAXDROPCOUNT = 100;
        public const string CollectableCubePrefabName = "CollectableCube";
        public const string CollectableSpherePrefabName = "CollectableSphere";
        public const string CollectableCylinderPrefabName = "CollectableCylinder";
        public const string CollectableCapsulePrefabName = "CollectableCapsule";
        public const string LEVELSPATH = "Assets/GAME/Prefabs/Levels/";
    }

    public struct AnimatorKeywords
    {
        public const string X_INPUT = "Xinput";
        public const string Y_INPUT = "Yinput";
        public const string IDLE = "Idle";
        public const string WALK = "Walk";
    }
}