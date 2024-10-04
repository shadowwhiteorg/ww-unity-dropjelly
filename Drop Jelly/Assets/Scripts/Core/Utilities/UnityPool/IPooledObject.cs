using UnityEngine;
using UnityEngine.Pool;

namespace ww.Utilities.ObjectPooling.UnityPool
{
    internal interface IPooledObject<T> where T : MonoBehaviour
    {
        IObjectPool<T> ObjectPool { set; }

    }
}
