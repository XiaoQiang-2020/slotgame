using System.Collections.Generic;
using UnityEngine;

namespace App.UI
{
    public sealed class UIManager
    {
        private static readonly UIManager s_instance = new UIManager();

        private readonly Dictionary<string, UIWindow> _windows = new Dictionary<string, UIWindow>();
        private readonly List<UIWindow> _popupStack = new List<UIWindow>();

        public static UIManager Instance => s_instance;

        private UIManager() { }

        public void Register(UIWindow window)
        {
            if (window == null)
                return;

            string windowId = window.WindowId;
            if (string.IsNullOrWhiteSpace(windowId))
            {
                Debug.LogError($"Cannot register UI window with empty id: {window.name}");
                return;
            }

            if (_windows.TryGetValue(windowId, out var existing) && existing != null && existing != window)
            {
                Debug.LogWarning($"UI window id '{windowId}' is already registered. Replacing {existing.name} with {window.name}.");
            }

            _windows[windowId] = window;
        }

        public void Unregister(UIWindow window)
        {
            if (window == null)
                return;

            if (_windows.TryGetValue(window.WindowId, out var existing) && existing == window)
                _windows.Remove(window.WindowId);

            _popupStack.Remove(window);
        }

        public void Open(string windowId)
        {
            if (!TryGetWindow(windowId, out var window))
                return;

            window.Open();

            if (window.Layer == UILayer.Popup)
            {
                _popupStack.Remove(window);
                _popupStack.Add(window);
            }
        }

        public void Close(string windowId)
        {
            if (!TryGetWindow(windowId, out var window))
                return;

            window.Close();
            _popupStack.Remove(window);
        }

        public void CloseTopPopup()
        {
            for (int i = _popupStack.Count - 1; i >= 0; i--)
            {
                var window = _popupStack[i];
                if (window == null || !window.IsOpen)
                {
                    _popupStack.RemoveAt(i);
                    continue;
                }

                window.Close();
                _popupStack.RemoveAt(i);
                return;
            }
        }

        private bool TryGetWindow(string windowId, out UIWindow window)
        {
            if (string.IsNullOrWhiteSpace(windowId))
            {
                Debug.LogError("UI window id is empty.");
                window = null;
                return false;
            }

            if (_windows.TryGetValue(windowId, out window) && window != null)
                return true;

            Debug.LogWarning($"UI window '{windowId}' is not registered.");
            return false;
        }
    }
}
