using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockInstance = new object();
    private static bool applicationIsQuitting = false;

    /// <summary>
    /// Runtime instance reference of T.
    /// </summary>
    public static T GetRuntimeInstance()
    {
        if (applicationIsQuitting)
        {
            Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");
            return null;
        }

        lock (lockInstance)
        {
            if (instance == null)
            {
                T[] instances = FindObjectsOfType<T>();

                if (instances != null && instances.Length > 0)
                {
                    instance = instances[0];

                    if (instances.Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return instance;
                    }
                }

                if (instance == null)
                {
                    GameObject singleton = new GameObject();
                    instance = singleton.AddComponent<T>();
                    singleton.name = string.Format("[Singleton] - {0}", typeof(T).Name);
                }
            }

            return instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}
