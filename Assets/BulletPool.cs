using UnityEngine;
using System;
using System.Collections.Generic;

class BulletPool<T> where T : MonoBehaviour, IPoolable
{
    List<T> bullets;
    int nextBullet;
    Func<T> factory;

    public BulletPool(GameObject prefab, int initialSize)
    {
        bullets = new List<T>(initialSize);

        T Factory() => UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<T>();

        for (var i = 0; i < initialSize; i++)
        {
            bullets.Add(Factory());
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


