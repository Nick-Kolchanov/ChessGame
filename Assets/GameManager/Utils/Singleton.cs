using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    protected virtual void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("[Singleton] Trying to create another instance of singleton");
        }
        else
        {
            _instance = (T) this;
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
