using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// make sure the T is a Singleton type class
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T singletonInstance { get { return instance; } }

    // only allowed the child class to see the methods 
    protected virtual void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = (T)this;
    }

    public static bool isSingletonCreated { get { return instance != null; } }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
