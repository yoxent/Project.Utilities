using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectUtilities.Core
{
    /// <summary>
    /// Simple service locator for dependency injection.
    /// Lives in a bootstrap scene and persists via DontDestroyOnLoad.
    /// </summary>
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator _instance;

        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Global access to the ServiceLocator instance.
        /// </summary>
        public static ServiceLocator Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindFirstObjectByType<ServiceLocator>();
                if (_instance == null)
                {
                    var go = new GameObject("ServiceLocator");
                    _instance = go.AddComponent<ServiceLocator>();
                    if (Application.isPlaying)
                        DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Register a service instance for the given type.
        /// </summary>
        public void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                Debug.LogError("Cannot register null service.");
                return;
            }

            _services[typeof(T)] = service;
        }

        /// <summary>
        /// Retrieve a previously registered service or null if not found.
        /// </summary>
        public T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }

            return null;
        }

        /// <summary>
        /// Unregister a service of the given type.
        /// </summary>
        public void Unregister<T>() where T : class
        {
            _services.Remove(typeof(T));
        }
    }
}

