using UnityEngine;

namespace MVC
{
    public abstract class BaseController : MonoBehaviour
    {
        protected virtual void OnInit() { }
        protected virtual void OnDispose() { }

        protected virtual void Awake()
        {
            OnInit();
        }

        protected virtual void OnDestroy()
        {
            OnDispose();
        }
    }
}