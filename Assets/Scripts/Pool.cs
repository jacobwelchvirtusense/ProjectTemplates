/*
 * (Jacob Welch)
 * (Pool)
 * (Singleton and Object Pooling Pattern)
 * (Description: Holds data for a pool that can be used by an object pooler.)
 */
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Pool", menuName = "Object Pool", order = 0)]
public class Pool: ScriptableObject
{
    [field:Tooltip("The gameobject that all of the objects in the pool should be")]
    [field:SerializeField]
    public GameObject Prefab { get; private set; }

    [field: Range(1, 10000)]
    [field: Tooltip("The max amount of objects in the pool and that can be spawned at once")]
    [field: SerializeField]
    public int Size { get; private set; } = 1;

    /// <summary>
    /// Initializes base settings of any pool.
    /// </summary>
    private Pool()
    {
        Size = 1;
    }

    /// <summary>
    /// A constructor for a new pool
    /// </summary>
    /// <param name="prefab">The prefab that should be spawned.</param>
    /// <param name="size">The size of the prefab pool that should be spawned.</param>
    public Pool(GameObject prefab, int size)
    {
        Prefab = prefab;
        Size = size;
    }
}