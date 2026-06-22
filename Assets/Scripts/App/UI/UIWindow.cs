using MVC;
using UnityEngine;

namespace App.UI
{
    public class UIWindow : BaseView
    {
        [SerializeField]
        private string _windowId;

        [SerializeField]
        private UILayer _layer = UILayer.Screen;

        public virtual string WindowId => string.IsNullOrWhiteSpace(_windowId) ? GetType().Name : _windowId;
        public virtual UILayer Layer => _layer;
        public bool IsOpen { get; private set; }

        protected virtual void Awake()
        {
            UIManager.Instance.Register(this);
        }

        protected virtual void OnDestroy()
        {
            UIManager.Instance.Unregister(this);
        }

        public void Open()
        {
            if (IsOpen)
                return;

            IsOpen = true;
            Show();
            OnOpen();
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            IsOpen = false;
            OnClose();
            Hide();
        }

        protected virtual void OnOpen() { }

        protected virtual void OnClose() { }
    }
}
