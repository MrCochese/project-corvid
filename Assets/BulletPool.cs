using UnityEngine;
using System;
using System.Collections.Generic;

class BulletPool<T> where T : MonoBehaviour, IPoolable
{
    List<T> bullets = new List<T>();
    int nextBullet;
    Func<T> factory;

    public BulletPool(GameObject prefab, int initialSize)
    {
        factory = () => UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<T>();

        for (var i = 0; i < initialSize; i++)
        {
            bullets.Add(factory());
        }

        nextBullet = 0;
    }

    public T GetNext()
    {
        var freeBullet = bullets[nextBullet];

        nextBullet++;
        if (nextBullet >= bullets.Count)
            nextBullet = 0;

        return freeBullet;
    }
}


