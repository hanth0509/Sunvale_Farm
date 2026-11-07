using UnityEngine;

namespace Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [Header("Singleton")]
        [SerializeField] private bool dontDestroyOnLoad;
        
        private static T instance;
        public static T Instance { get { return instance; } }

        private void Awake()
        {
            // If instance is null, assigned reference to it
            if (instance == null)
            {
                instance = this as T;

                // Check if don't destroy on load is enable, meaning the object will take to next scene
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                AwakeSingleton(); // Trigger the singleton awake function
            }
            else
            {
                // Destroy the object if the instance is already available
                Destroy(gameObject);
            }
        }

        protected abstract void AwakeSingleton();
    }
}
