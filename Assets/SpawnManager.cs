using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : MonoBehaviour
{
    public int poolSize = 5;
    public GameObject prefab;
    public GameObject[] pool;
    public NetworkHash128 assetId { get; set; }

    public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
    public delegate void UnSpawnDelegate(GameObject spawned);

    void Start()
    {
        assetId = prefab.GetComponent<NetworkIdentity>().assetId;
        pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; ++i)
        {
            pool[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            pool[i].name = "PoolObject" + i;
            pool[i].SetActive(false);
        }

        ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
    }

    public GameObject GetFromPool(Vector3 position)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                Debug.Log("Activating object " + obj.name + " at " + position);
                obj.transform.position = position;
                obj.SetActive(true);
                return obj;
            }
        }
        Debug.LogError("Could not grab object from pool, nothing available");
        return null;
    }

    public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId)
    {
        return GetFromPool(position);
    }

    public void UnSpawnObject(GameObject spawned)
    {
        Debug.Log("Re-pooling object " + spawned.name);
        spawned.SetActive(false);
    }
}
