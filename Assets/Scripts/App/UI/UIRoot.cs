using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace App.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public sealed class UIRoot : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _screenLayer;

        [SerializeField]
        private RectTransform _popupLayer;

        [SerializeField]
        private RectTransform _overlayLayer;

        public RectTransform ScreenLayer => _screenLayer;
        public RectTransform PopupLayer => _popupLayer;
        public RectTransform OverlayLayer => _overlayLayer;

        private void Awake()
        {
            EnsureCanvas();
            EnsureEventSystem();
            EnsureLayers();
        }

        public RectTransform GetLayerRoot(UILayer layer)
        {
            EnsureLayers();

            switch (layer)
            {
                case UILayer.Screen:
                    return _screenLayer;
                case UILayer.Popup:
                    return _popupLayer;
                case UILayer.Overlay:
                    return _overlayLayer;
                default:
                    return _screenLayer;
            }
        }

        public void EnsureLayers()
        {
            _screenLayer = EnsureLayer(_screenLayer, "ScreenLayer", 0);
            _popupLayer = EnsureLayer(_popupLayer, "PopupLayer", 1);
            _overlayLayer = EnsureLayer(_overlayLayer, "OverlayLayer", 2);
        }

        private void EnsureCanvas()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();
        }

        private static void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
                return;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private RectTransform EnsureLayer(RectTransform current, string layerName, int siblingIndex)
        {
            if (current == null)
            {
                var child = transform.Find(layerName);
                current = child != null ? child.GetComponent<RectTransform>() : null;
            }

            if (current == null)
            {
                var layerObject = new GameObject(layerName, typeof(RectTransform));
                current = layerObject.GetComponent<RectTransform>();
                current.SetParent(transform, false);
            }

            StretchToParent(current);
            current.SetSiblingIndex(Mathf.Min(siblingIndex, transform.childCount - 1));
            return current;
        }

        private static void StretchToParent(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.localScale = Vector3.one;
        }
    }
}
