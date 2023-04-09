using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class LevelEditor : EditorWindow
{

    private ObjectType _objectType;
    private PlatformType _platformType;
    private CollectableType _collectableType;
    private readonly List<GameObject> _platforms = new List<GameObject>();
    private readonly List<GameObject> _allRoads = new List<GameObject>();
    private readonly List<GameObject> _collectables = new List<GameObject>();
    private int _selected;
    private Vector3[] _spawnPoints;
    private int _spawnCount = 1;
    private bool _manualCollectable;
    private float _offset;
    private LevelController _levelController;
    private GameObject _levelHolder;
    private bool _hasLevelEnd;
    private bool _levelRepeat;
    private int _desiredDropCount = 1;
    private bool _designLevel;
    private bool _editLevel;
    
    private GameObject _shownLevel;
    private int _levelIndex;
    private bool _hasSceneChanged;
    private Transform _selectedTransform;
    private int _dropAreaCount;
    
    
    void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChange;
        SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChange;
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSelectionChange()
    {
        // Seçili nesnenin transformunu alın

        if (_selectedTransform != null)
        {
            // Seçili nesnenin transformunda bir değişiklik olduğunda hasObjectChanged değerini true yapın
            if (_selectedTransform.hasChanged)
            {
                _hasSceneChanged = true;
                _selectedTransform.hasChanged = false;
            }
        }
    }

    [MenuItem("Level Editor/Level Editor")]
    static void Init()
    {
        LevelEditor window = (LevelEditor)EditorWindow.GetWindow(typeof(LevelEditor));
        window.Show();
    }

    void OnGUI()
    {
        // ControllerHub.Instance.InitializeForEditor();
        _levelController = ControllerHub.Instance.LevelController;
        DrawEditorGUI();
    }
    
    
    void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && _manualCollectable)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            int layer = 1 << 9;
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layer))
            {
                var collectable = PrefabUtility.InstantiatePrefab(GetPrefab(_collectableType, GetCollectablePrefabName(_collectableType)), _levelHolder.transform) as GameObject;
                _collectables.Add(collectable);
                if (!(collectable is null)) collectable.transform.position = hit.point;
            }
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && _manualCollectable)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            int layer = 1 << 8;
            int layerHolder = 8;

            Collider[] colliders = new Collider[1]; 
            int numColliders = 0;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
            {
                numColliders = Physics.OverlapSphereNonAlloc(hit.point, 0.1f, colliders); 
            }

            for (int i = 0; i < numColliders; i++)
            {

                if (colliders[i].gameObject.layer != layerHolder)
                    return;
                
                DestroyImmediate(colliders[i].transform.parent.gameObject);
            }

        }
    }

    void OnGridClick(int index) {
        _selected = index;
    }
    private void DrawEditorGUI()
    {
        if (EditorGUILayout.Toggle("Design a new level", _designLevel))
        {
            _designLevel = true;
            _editLevel = false;
        }
        if (EditorGUILayout.Toggle("Edit existing levels", _editLevel))
        {
            _editLevel = true;
            _designLevel = false;
        }
        
        GUILayout.Space(30);

        if (_designLevel)
        {
            if (_shownLevel != null)
            {
                DestroyImmediate(_shownLevel);
            }
            
            if (_levelHolder == null)
            {
                _levelHolder = new GameObject("LevelHolder");
            }
        
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUIStyle headStyle = new GUIStyle();
            headStyle.fontSize = 30;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(Screen.width / 3, 0, 300, 50), "Design a new level", headStyle);
            GUILayout.Space(50);

            GUILayout.Label("Select object type for spawn");
            GUILayout.Space(10);
            _objectType = (ObjectType)EditorGUILayout.EnumPopup(_objectType);
            if (_objectType == ObjectType.Platform)
            {
                _selected = 100;
                _platformType = (PlatformType)EditorGUILayout.EnumPopup(_platformType);
               
                GUILayout.Space(10);
                
                if (_platformType == PlatformType.StraightPlatform)
                {
                    if (GUILayout.Button("Spawn"))
                    {
                        var gameObject = PrefabUtility.InstantiatePrefab(GetPrefab(_platformType, "StraightPlatform"),_levelHolder.transform) as GameObject;
                        gameObject.transform.position = Vector3.forward * _offset;
                        _offset += gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.z;
                        _platforms.Add(gameObject);
                        _allRoads.Add(gameObject);
                    } 
                }
                else if (_platformType == PlatformType.DropAreaPlatform)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("Desired Drop");
                    GUILayout.Label(_desiredDropCount.ToString());
                    _desiredDropCount = (int)GUILayout.HorizontalSlider(_desiredDropCount, 1, 99);
                    GUILayout.Space(10);
                    
                    if (GUILayout.Button("Spawn"))
                    {
                        var gameObject = PrefabUtility.InstantiatePrefab(GetPrefab(_platformType, "DropAreaPlatform"), _levelHolder.transform) as GameObject;
                        var dropArea = gameObject.GetComponent<DropAreaController>();
                        gameObject.transform.position = Vector3.forward * _offset;
                        dropArea.SetDesiredDropCountText(_desiredDropCount);
                        dropArea.SetDesiredCount(_desiredDropCount);
                        _offset += gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.z;
                        _allRoads.Add(gameObject);
                        _dropAreaCount++;
                    } 
                }
                else if (_platformType == PlatformType.LevelEndPlatform)
                {
                    if (GUILayout.Button("Spawn"))
                    {
                        var gameObject = PrefabUtility.InstantiatePrefab(GetPrefab(_platformType, "LevelEndPlatform"),_levelHolder.transform) as GameObject;
                        gameObject.transform.position = Vector3.forward * _offset;
                        _offset += gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.z;
                        _allRoads.Add(gameObject);
                        if (!_hasLevelEnd)
                        {
                            _hasLevelEnd = true;
                        }
                    } 
                }
                
                if (_allRoads.Count > 0)
                {
                    if (GUILayout.Button("Undo"))
                    {
                        int lastIndex = _allRoads.Count - 1;
                        var tempObject = _allRoads[lastIndex];
                        _offset -= tempObject.GetComponentInChildren<MeshRenderer>().bounds.size.z;
                        if (_platforms.Contains(tempObject))
                        {
                            _platforms.Remove(tempObject);
                        }
                        _allRoads.RemoveAt(lastIndex);
                        DestroyImmediate(tempObject);
                    }

                    if (GUILayout.Button("Clear"))
                    {
                        for (int i = 0; i < _allRoads.Count; i++)
                        {
                            DestroyImmediate(_allRoads[i]);
                        }
                        _allRoads.Clear();
                        _platforms.Clear();
                        _offset = 0;
                    }
                }
            }
            else
            {
                _collectableType = (CollectableType)EditorGUILayout.EnumPopup(_collectableType);
                string[] spawnType = new[] { "Random", "Manual" };
                GUILayout.BeginVertical("Box");
                _selected = GUILayout.SelectionGrid(_selected, spawnType, 2);
                GUILayout.EndVertical();
                if (_collectableType == CollectableType.Capsule)
                {
                    if (_selected == 0)
                    {
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        else
                        {
                            RandomColllectableGenerate(Consts.LevelEditorSettings.CollectableCapsulePrefabName); 
                        }
                    }
                    else if (_selected == 1)
                    {
                        EditorGUILayout.HelpBox("You can instantiate collectable by left click on the platform.", MessageType.Info);
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("You can delete collectable by right-clicking on the collectable.", MessageType.Info);
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        _manualCollectable = true;
                    }
                     
                }
                else if (_collectableType == CollectableType.Cube)
                {
                    if (_selected == 0)
                    {
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        else
                        {
                            RandomColllectableGenerate(Consts.LevelEditorSettings.CollectableCubePrefabName); 
                        }
                    }
                    else if (_selected == 1)
                    {
                        EditorGUILayout.HelpBox("You can instantiate collectable by left click on the platform.", MessageType.Info);
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("You can delete collectable by right-clicking on the collectable.", MessageType.Info);
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        _manualCollectable = true;
                    }
                }
                if (_collectableType == CollectableType.Cylinder)
                {
                    if (_selected == 0)
                    {
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        else
                        {
                            RandomColllectableGenerate(Consts.LevelEditorSettings.CollectableCylinderPrefabName); 
                        }
                    }
                    else if (_selected == 1)
                    {
                        EditorGUILayout.HelpBox("You can instantiate collectable by left click on the platform.", MessageType.Info);
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("You can delete collectable by right-clicking on the collectable.", MessageType.Info);
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        _manualCollectable = true;
                    }
                }
                if (_collectableType == CollectableType.Sphere)
                {
                    if (_selected == 0)
                    {
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        else
                        {
                            RandomColllectableGenerate(Consts.LevelEditorSettings.CollectableSpherePrefabName); 
                        }
                    }
                    else if (_selected == 1)
                    {
                        EditorGUILayout.HelpBox("You can instantiate collectable by left click on the platform.", MessageType.Info);
                        GUILayout.Space(10);
                        EditorGUILayout.HelpBox("You can delete collectable by right-clicking on the collectable.", MessageType.Info);
                        if (_platforms.Count <= 0)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.HelpBox("Before you can create a Collectable, you must create a platform!", MessageType.Error);
                        }
                        _manualCollectable = true;
                    }
                }
            }

            if (_allRoads.Count > 0 && _platforms.Count > 0 && _collectables.Count > 0 && _hasLevelEnd && _dropAreaCount == 3)
            {
                GUILayout.BeginVertical("Box");
                GUI.color = Color.red;
                EditorGUILayout.HelpBox("When the whole level loop is over, will this level also loop while the levels are replayed?", MessageType.Info);
                GUILayout.Space(10);
                if (EditorGUILayout.Toggle("Level Repeat", _levelRepeat))
                {
                    _levelRepeat = true;
                }
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("After clicking create level, the level will integrate itself into the level system, you don't need to do anything else.", MessageType.Info);
                GUILayout.Space(10);
                if (GUILayout.Button("Create Level"))
                {
                    GameObject player = PrefabUtility.InstantiatePrefab(
                        AssetDatabase.LoadAssetAtPath<GameObject>(Consts.LevelEditorSettings.PLAYERPREFABPATH), _levelHolder.transform) as GameObject;
                    player.transform.position = new Vector3(0, 0.073f, 3f);
                    LevelFacade levelFacade = _levelHolder.AddComponent<LevelFacade>();
                    levelFacade.Player = player;
                    
                    
                    GameObject levelPrefab =
                        PrefabUtility.SaveAsPrefabAsset(_levelHolder, Consts.LevelEditorSettings.LEVELFACADESPATH + "Level" + (_levelController.GetLevelIndex() + 2) + ".prefab");
                    LevelFacade levelFacade2 = levelPrefab.GetComponent<LevelFacade>();
                    
                    
                    LevelContent levelContent = ScriptableObject.CreateInstance<LevelContent>();
                    levelContent.LevelFacade = levelFacade2;
                    levelContent.LightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>("Assets/GAME/ScriptableObjects/LightingSettings/LightingSettings.asset");
                    AssetDatabase.CreateAsset(levelContent, Consts.LevelEditorSettings.LEVELCONTENTSPATH + "Level" + (_levelController.GetLevelIndex() + 2) + ".asset");
                    AssetDatabase.SaveAssets();
                    _levelController.AddLevelFromLevelEditor(levelContent);
                    if (_levelRepeat)
                    {
                        _levelController.AddRepeatLevelFromLevelEditor(levelContent);
                    }
                    //Close();
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.Space(20);
                EditorGUILayout.HelpBox("To create a level, you need to add a straight platform, level end platform, 3 drop area platform and collectables.", MessageType.Info);
            }
            
            GUILayout.EndArea();
        }

        else if(_editLevel)
        {
            if(_levelHolder != null)
                DestroyImmediate(_levelHolder);
            
            GUIStyle headStyle = new GUIStyle();
            headStyle.fontSize = 30;
            headStyle.fontStyle = FontStyle.Bold;
            headStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(Screen.width / 3, 0, 300, 50), "Edit existing levels", headStyle);
            
            GUILayout.BeginArea(new Rect(0, 50, Screen.width, Screen.height));

            if (_levelController.GetTotalLevelCount() <= 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.HelpBox("No level available! Please create level from level editor.", MessageType.Error);
                if (GUILayout.Button("OK, take me there."))
                {
                    _editLevel = false;
                    _designLevel = true;
                }
                GUILayout.Space(10);
                GUILayout.EndArea();
                return;
               
            }
            else
            {
                _levelIndex = EditorGUILayout.Popup("Level", _levelIndex, _levelController.GetLevelsName().ToArray());
                if (GUILayout.Button("Show Level"))
                {
                    if (_shownLevel != null)
                    {
                        DestroyImmediate(_shownLevel);
                    }
                    _shownLevel = PrefabUtility.InstantiatePrefab(
                        _levelController.GetLevelFromIndex(_levelIndex).gameObject) as GameObject;
                    _selectedTransform = _shownLevel.transform;
                }



                if (_shownLevel != null)
                {
                    if (GUILayout.Button("Delete Level"))
                    {
                        DestroyImmediate(_shownLevel);
                        _levelController.RemoveLevelFromEditor(_levelIndex);
                        RefreshLevelList();
                    }

                    GUILayout.Space(10);
                    
                    EditorGUILayout.HelpBox("If you make a change to the level prefab and then return to this editor, the 'Apply changes' button will appear here. With this button, you can apply the changes you have made here directly to the level.", MessageType.Info);

                    GUILayout.Space(10);
                    if (_hasSceneChanged)
                    {
                        if (GUILayout.Button("Apply Changes"))
                        {
                            PrefabUtility.ApplyPrefabInstance(_shownLevel, InteractionMode.AutomatedAction);
                        }
                    }
                }
            }
            GUILayout.EndArea();
        }
        
    }

    private void RandomColllectableGenerate(string typeName)
    {
        _manualCollectable = false;
        GUILayout.Space(10);
        GUILayout.Label("Number of collectables for each platform");
        GUILayout.Label(_spawnCount.ToString());
        _spawnCount = (int)GUILayout.HorizontalSlider(_spawnCount, 1, 50);
        GUILayout.Space(10);
        if (GUILayout.Button("Spawn"))
        {
            if (_selected == 0)
            {
                var refObject = PrefabUtility.InstantiatePrefab(GetPrefab(_collectableType, typeName),_levelHolder.transform) as GameObject;
                var objectBounds = refObject.GetComponentInChildren<MeshRenderer>().bounds;
                DestroyImmediate(refObject);
                
                foreach (GameObject platform in _platforms)
                {
                    MeshRenderer platformMesh = platform.GetComponentInChildren<MeshRenderer>();
                    var bounds = platformMesh.bounds;
                    float minX = bounds.min.x + (objectBounds.size.x / 2f);
                    float maxX = bounds.max.x - (objectBounds.size.x / 2f);
                    float minZ = bounds.min.z + (objectBounds.size.z / 2f);
                    float maxZ = bounds.max.z - (objectBounds.size.z / 2f);
                    var position1 = platform.transform.position;

                    for (int i = 0; i < _spawnCount; i++)
                    {
                        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), position1.y ,Random.Range(minZ, maxZ));
                        var collectable = PrefabUtility.InstantiatePrefab(GetPrefab(_collectableType, typeName),_levelHolder.transform) as GameObject;

                        _collectables.Add(collectable);
                        if (!(collectable is null)) collectable.transform.position = randomPosition;
                    }
                }

            }
        }
    }
    
    private GameObject GetPrefab<T>(T type, string typeName)
    {
        if (type is PlatformType)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GAME/Prefabs/PlatformPrefabs/" + typeName + ".prefab");
        }
        if (type is CollectableType)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GAME/Prefabs/CollectablePrefabs/" + typeName + ".prefab");
        }
        else
        {
            Debug.LogError("Invalid type!");
            return null;
        }
    }

    private string GetCollectablePrefabName(CollectableType collectableType)
    {
        switch (collectableType)
        {
            case CollectableType.Cube:
                String colType = Consts.LevelEditorSettings.CollectableCubePrefabName;
                return colType;
            case CollectableType.Capsule:
                String colType2 = Consts.LevelEditorSettings.CollectableCapsulePrefabName;
                return colType2;
            case CollectableType.Cylinder:
                String colType3 = Consts.LevelEditorSettings.CollectableCylinderPrefabName;
                return colType3;
            case CollectableType.Sphere:
                String colType4 = Consts.LevelEditorSettings.CollectableSpherePrefabName;
                return colType4;
            default:
                return null;
        }
    }
    
    private void RefreshLevelList()
    {
        for (int i = 0; i < _levelController.GetTotalLevelCount(); i++)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_levelController.GetLevelList()[i]), "Level"+ (i+1));
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_levelController.GetLevelList()[i].LevelFacade), "Level"+ (i+1));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void OnHierarchyChanged()
    {
        _hasSceneChanged = true;
    }

    private void OnDestroy()
    {
        if (_shownLevel != null)
        {
            DestroyImmediate(_shownLevel);
        }
        _platforms.Clear();
        _allRoads.Clear();
        if (_levelHolder != null)
        {
            DestroyImmediate(_levelHolder);
        }
    }
    

    public enum ObjectType
    {
        Platform,
        Collectable
    }

    public enum PlatformType
    {
        StraightPlatform,
        LevelEndPlatform,
        DropAreaPlatform
    }

    public enum CollectableType
    {
        Cube,
        Sphere,
        Capsule,
        Cylinder
    }

    
}