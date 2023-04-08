using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class LevelEditor : EditorWindow
{

    private ObjectType _objectType;
    private PlatformType _platformType;
    private CollectableType _collectableType;
    private readonly List<GameObject> _platforms = new List<GameObject>();
    private readonly List<GameObject> _allRoads = new List<GameObject>();
    private readonly List<GameObject> _collectables = new List<GameObject>();
    private int _selected = -1;
    private Vector3[] _spawnPoints;
    private int _spawnCount = 1;
    private bool _manualCollectable;
    private float _offset;
    private LevelController _levelController;
    private GameObject _levelHolder;
    private bool _hasLevelEnd;
    private bool _levelRepeat;
    
    private void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    private void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    [MenuItem("Level Editors/Level Editor")]
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
        // Sol fare tıklandığında
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && _manualCollectable)
        {
            // Tıklanan noktanın dünya koordinatlarını bulun
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

            Collider[] colliders = new Collider[1]; // önceden tanımlanmış bir dizi
            int numColliders = 0;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
            {
                numColliders = Physics.OverlapSphereNonAlloc(hit.point, 0.1f, colliders); // collider'ları diziye yazar ve sayısını 
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
        if (_levelHolder == null)
        {
            _levelHolder = new GameObject("LevelHolder");
        }
        
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

        GUILayout.Label("Select for spawn");
        //GUILayout.Space(10);

        _objectType = (ObjectType)EditorGUILayout.EnumPopup(_objectType);
        if (_objectType == ObjectType.Platform)
        {
            _selected = 100;
            _platformType = (PlatformType)EditorGUILayout.EnumPopup(_platformType);
           
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
                if (GUILayout.Button("Spawn"))
                {
                    var gameObject = PrefabUtility.InstantiatePrefab(GetPrefab(_platformType, "DropAreaPlatform"), _levelHolder.transform) as GameObject;
                    gameObject.transform.position = Vector3.forward * _offset;
                    _offset += gameObject.GetComponentInChildren<MeshRenderer>().bounds.size.z;
                    _allRoads.Add(gameObject);
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

        if (_allRoads.Count > 0 && _platforms.Count > 0 && _collectables.Count > 0 && _hasLevelEnd)
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
                GameObject levelPrefab =
                    PrefabUtility.SaveAsPrefabAsset(_levelHolder, Consts.LevelEditorSettings.LEVELFACADESPATH + "Level" + (_levelController.GetLevelIndex() + 2) + ".prefab");
                LevelFacade levelFacade = levelPrefab.AddComponent<LevelFacade>();
                
                LevelContent levelContent = ScriptableObject.CreateInstance<LevelContent>();
                levelContent.LevelFacade = levelFacade;
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
        
        GUILayout.EndArea();
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
                    float minY = position1.y;
                    float maxY = position1.y + position1.y;
                    
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

    private void OnDestroy()
    {
        _platforms.Clear();
        _allRoads.Clear();
        DestroyImmediate(_levelHolder);
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