using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// If the instance of the class should be persistent between scenes.
    /// </summary>
    [SerializeField]
    private bool dontDestroyOnLoad;

    private static T instance;
    /// <summary>
    /// Reference to the single instance of this class.
    /// </summary>
    public static T Instance => instance;



    protected virtual void Awake()
    {
        // Normal singleton reference
        if (instance == null)
        {
            instance = this as T;
        }
        else
        {
            if (this) { Destroy(gameObject); }
        }

        // If true, allow for scene loading
        if (dontDestroyOnLoad)
        {
            T[] objs = FindObjectsOfType<T>();
            if (objs.Length > 1)
            {
                if (this) { Destroy(gameObject); }
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}
