using System.Collections.Generic;
using UnityEngine;

namespace ww.Utilities.ObjectPooling.ManualPool
{
    internal class ObjectPool : MonoBehaviour
    {
        [SerializeField] private uint initialPoolSize = 5;
        public uint InitialPoolSize => initialPoolSize;

        [SerializeField] PooledObject objectToPool;
        private Stack<PooledObject> poolStack;
        private GameObject poolParent;

        private void Awake()
        {
            
        }

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            poolParent = new GameObject("ManualObjectPoolParent");

            if(objectToPool == null)
            {
                Debug.Log("Object to pool is not set!");
                return;
            }

            poolStack = new Stack<PooledObject>();

            PooledObject instance = null;

            for (int i = 0; i < initialPoolSize; i++)
            {
                instance = Instantiate(objectToPool);
                instance.ObjectPool = this;
                poolStack.Push(instance);
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(poolParent.transform);
            }
        }

        public PooledObject GetFromPool()
        {
            if (objectToPool == null)
                return null;

            if(poolStack.Count == 0)
            {
                PooledObject newInstance = Instantiate(objectToPool);
                poolStack.Push(newInstance);
                return newInstance;
            }

            PooledObject nextInstance = poolStack.Pop();
            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        public void ReleasePool(PooledObject pooledObject)
        {
            poolStack.Push(pooledObject);
            pooledObject.gameObject.SetActive(false);
        }
    }
}