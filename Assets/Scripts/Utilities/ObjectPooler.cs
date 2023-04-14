/*
 * (Jacob Welch)
 * (ObjectPooler)
 * (Singleton and Object Pooling Pattern)
 * (Description: Handles the pooling and limiting of object instances to save processinig resources. )
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPooler : Singleton<ObjectPooler>
{
    #region Fields
    [Tooltip("The pools to be spawned when this object is spawned")]
    [SerializeField] private List<Pool> pools;

    /// <summary>
    /// A dictionary of every pool that has been initialized by this object pooler.
    /// Given a prefab name, that pool may be accessed.
    /// </summary>
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    #endregion

    #region Functions
    /// <summary>
    /// Calls for initial list of pools to be initialized.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        SpawnInitialObjectPools();
    }

    /// <summary>
    /// Spawns the initial list of pools.
    /// </summary>
    private void SpawnInitialObjectPools()
    {
        foreach (Pool pool in pools)
        {
            InitializeNewPool(pool);
        }
    }

    /// <summary>
    /// Initializes a new pool.
    /// </summary>
    /// <param name="newPool">The new pool to be spawned and initialized.</param>
    public static void InitializeNewPool(Pool newPool)
    {
        if (CheckIfInstanceDoesntExists()) return;

        if(newPool == null)
        {
            Debug.Log("The given pool is null; it will not be spawned");
            return;
        }

        if(newPool.Prefab == null)
        {
            Debug.Log("The given pool has a null prefab; it will not be spawned");
            return;
        }

        if (!Instance.poolDictionary.ContainsKey(newPool.Prefab.name))
        {
            // The pool of objects being spawned.
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Spawns objects up to the pool size and adds them to the pool queue
            for (int i = 0; i < newPool.Size; i++)
            {
                GameObject obj = Instantiate(newPool.Prefab, Instance.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            Instance.poolDictionary.Add(newPool.Prefab.name, objectPool);
        }
    }

    /// <summary>
    /// Spawns a given prefab from a pool.
    /// </summary>
    /// <param name="prefabName">The name of the prefab to spawn.</param>
    /// <param name="position">The position to spawn the prefab at.</param>
    /// <param name="rotation">The roation to spawn the prefab at.</param>
    /// <param name="parent">The parent to spawn the object onto.</param>
    /// <returns></returns>
    public static GameObject SpawnFromPool(string prefabName, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (CheckIfInstanceDoesntExists()) return null;

        if (!Instance.poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning("Pool with tag " + prefabName + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = Instance.poolDictionary[prefabName].Dequeue();
        Instance.poolDictionary[prefabName].Enqueue(objectToSpawn);

        // Set object to spawn to inactive first and then active so OnEnable events are still fired
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);

        // Basically instantiates the object except awake and start are not called
        if (parent == null) parent = Instance.transform;
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.transform.parent = parent;

        return objectToSpawn;
    }

    /// <summary>
    /// Returns a given object back into the pool.
    /// </summary>
    /// <param name="prefabName">The name of the prefab to be returned to the pool</param>
    /// <param name="objectToReturn">The object to return to the pool.</param>
    public static void ReturnObjectToPool(string prefabName, GameObject objectToReturn)
    {
        if (CheckIfInstanceDoesntExists()) return;

        objectToReturn.SetActive(false);
        objectToReturn.transform.parent = Instance.transform;

        // Resets the pool so only one instance of this object is in it and enqueues it
        var queue = Instance.poolDictionary[prefabName];
        queue = new Queue<GameObject>(queue.Where(x => x != objectToReturn));
        Instance.poolDictionary[prefabName] = queue;
        Instance.poolDictionary[prefabName].Enqueue(objectToReturn);

    }

    /// <summary>
    /// Chacks if the ObjectPooler instance doesn't exist.
    /// </summary>
    /// <returns></returns>
    private static bool CheckIfInstanceDoesntExists()
    {
        if (Instance == null)
        {
            Debug.Log("No object pooler found");
            return true;
        }

        return false;
    }
    #endregion
}
