using System.Collections.Generic;
using UnityEngine;

public class ComponentPool<T> : MonoBehaviour where T : Component
{
    public T prefab;

    private Queue<T> pool = new();

    public T GetObject()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();

            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        return Instantiate(prefab);
    }

    public void ReturnObject(T obj)
    {
        if (!pool.Contains(obj))
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
