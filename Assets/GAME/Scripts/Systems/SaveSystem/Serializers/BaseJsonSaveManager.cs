using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;

    public abstract class BaseJsonSaveManager<T> where T : class, new()
    {

        protected abstract JsonSerializer<T> Serializer { get; }

        protected JsonSerializer<T> _serializer;

        public virtual void Init(string dataName)
        {
            string path = Application.persistentDataPath + Path.DirectorySeparatorChar + dataName;
            _serializer = new JsonSerializer<T>(path);
        }

        public virtual void SaveData()
        {
            Serializer.SaveData();
        }

        [Button, PropertyOrder(100)]
        public virtual void ClearData()
        {
            Serializer.ClearData();
        }

    }