using UnityEngine;

namespace Managers
{
    public class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        public static T instance { get; private set; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void Init()
        {
            
        }

        public virtual void Dispose()
        {
        }
    }
}