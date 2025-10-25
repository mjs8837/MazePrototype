using UnityEngine;

/// <summary>
/// Abstract Singleton class respoinsible for creating only one instance of a MonoBehaviour class.
/// </summary>
/// <typeparam name="T">MonoBeahaviour class this Singleton will generate an instance of.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"An instance of type {typeof(T)} already exists. Deleting extra game object.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"An instance of type {typeof(T)} has been created.");
            Instance = this as T;
        }
    }
}
