using UnityEngine;

namespace UI.Screens
{
    public class Panel : MonoBehaviour
    {
        public virtual void Init()
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}