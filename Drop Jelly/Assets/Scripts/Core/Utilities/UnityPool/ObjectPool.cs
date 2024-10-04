using System;
using UnityEngine;
using UnityEngine.Pool;


namespace ww.Utilities.ObjectPooling.UnityPool
{

    internal class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        private IObjectPool<T> objectPool;
        public IObjectPool<T> SharedObjectPool { get => objectPool; }

        private bool collectionCheck = true;
        private GameObject objectPoolParent;

        [SerializeField] T pooledObjectPrefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxCapacity = 100;

        private void Awake()
        {
            objectPoolParent = new GameObject($"{typeof(T).Name}UnityObjectPoolParent");
            objectPool = new UnityEngine.Pool.ObjectPool<T>(CreatePooledObject, OnRetrieveFromPool, OnReturnToPool, OnDestroyPooledObject, collectionCheck, defaultCapacity, maxCapacity);
        }


        private T CreatePooledObject()
        {
            T pooledObjectInstance = Instantiate(pooledObjectPrefab);
            pooledObjectInstance.transform.SetParent(objectPoolParent.transform);

            if(pooledObjectInstance is IPooledObject<T> pooledObject)
            {
                pooledObject.ObjectPool =  objectPool ;
            }
            return pooledObjectInstance;
        }

        private void OnRetrieveFromPool(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        private void OnReturnToPool(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }
        private void OnDestroyPooledObject(T pooledObject)
        {
            Destroy(pooledObject);
        }

        
    }
}
