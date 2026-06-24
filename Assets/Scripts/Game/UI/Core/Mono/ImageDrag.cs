
namespace Game
{
    using UnityGameObject = UnityEngine.GameObject;
    using UnityEngineTransform = UnityEngine.Transform;
    using UnityEngineMonoBehaviour = UnityEngine.MonoBehaviour;
    using UnityEngineRequireComponent = UnityEngine.RequireComponent;
    using UnityEngineRectTransform = UnityEngine.RectTransform;
    using UnityEngineRectTransformUtility = UnityEngine.RectTransformUtility;
    using UnityEngineCanvasGroup = UnityEngine.CanvasGroup;
    using UnityEngineGameObject = UnityEngine.GameObject;
    using UnityEngineVector3 = UnityEngine.Vector3;
	using UnityEngineVector2 = UnityEngine.Vector2;
    using UnityEngineEventSystemsPointerEventData = UnityEngine.EventSystems.PointerEventData;
    using UnityEngineEventSystemsIBeginDragHandler = UnityEngine.EventSystems.IBeginDragHandler;
    using UnityEngineEventSystemsIDragHandler = UnityEngine.EventSystems.IDragHandler;
    using UnityEngineEventSystemsIEndDragHandler = UnityEngine.EventSystems.IEndDragHandler;
    using UnityEngineEventSystemsIPointerDownHandler = UnityEngine.EventSystems.IPointerDownHandler;
    using UnityEngineEventSystemsIPointerUpHandler = UnityEngine.EventSystems.IPointerUpHandler;

    using UnityEngineUIImage = UnityEngine.UI.Image;
    using System;
    using UnityEngine.EventSystems;

    [UnityEngineRequireComponent(typeof(UnityEngineUIImage))]
    public class ImageDrag : UnityEngineMonoBehaviour, UnityEngineEventSystemsIBeginDragHandler, UnityEngineEventSystemsIDragHandler, UnityEngineEventSystemsIEndDragHandler, UnityEngineEventSystemsIPointerDownHandler, UnityEngineEventSystemsIPointerUpHandler
    {
        #region ��Ա����
        /// <summary>
        /// �ƶ��е�ͼƬ�Ƿ��뵱ǰ���������ڵ�ͼ�ķ��򱣳�һ��
        /// </summary>
        private bool m_isRotateByBack = true;

        /// <summary>
        /// �϶��е����
        /// </summary>
        private UnityEngineGameObject m_movingImage;

        /// <summary>
        /// �ƶ�����ķ���ο���
        /// </summary>
        private UnityEngineRectTransform m_draggingBack;

        /// <summary>
        /// ui���ڵ�
        /// </summary>
        private UnityEngineTransform m_uiRootNode = null;

        /// <summary>
        /// ��ק��ͼƬ
        /// </summary>
        private UnityEngineUIImage m_imageDrag = null;

        /// <summary>
        ///  ��ק��ͼƬ����
        /// </summary>
        private UnityEngineUIImage m_imageDragBg = null;

        /// <summary>
        /// ��ק�¼��Ļص�����
        /// </summary>
        private Action<bool,UnityEngineVector3> m_dragEventCallBack = null;

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        private bool m_isVertical = false;
		
		/// <summary>
        /// ��קƫ��
        /// </summary>
        private UnityEngineVector2 m_dragOffset = UnityEngineVector2.zero;

        /// <summary>
        /// ����ƫ��
        /// </summary>
        /// <param name="value"></param>
        public UnityEngineVector2 DragOffset
        {
            set
            {
                m_dragOffset = value;
            }
        }

        /// <summary>
        /// �Ƿ��
        /// </summary>
        private bool m_isOpen = false;
        #endregion

        #region ��������
        /// <summary>
        /// ������ק���
        /// </summary>
        /// <param name="rootNode">���ص�</param>
        /// <param name="ImageDrag">��ק��ͼƬ</param>
        /// <param name="startCall">�ص�����</param>
        /// <param name="slotOrIndex">��λ�����</param>
        /// <returns>��ק���</returns>
        public static ImageDrag CreatImageDrag(UnityEngineTransform rootNode, UnityEngineUIImage ImageDrag, UnityEngineUIImage ImageDragBg, Action<bool,UnityEngineVector3> callBack, bool isVertical = false)
        {
            if (null == rootNode || null == ImageDrag || null == callBack || null == ImageDragBg)
            {
                return null;
            }
            if (null == rootNode.GetComponent<UnityEngineUIImage>())
            {
                return null;
            }

            ImageDrag dragScript = rootNode.GetComponent<ImageDrag>();
            if (null == dragScript)
            {
                dragScript = rootNode.gameObject.AddComponent<ImageDrag>();
                if (null == dragScript)
                {
                    return null;
                }
            }
            dragScript.m_imageDragBg = ImageDragBg;
            dragScript.m_imageDrag = ImageDrag;
            dragScript.m_dragEventCallBack = callBack;
            dragScript.m_isOpen = true;
            dragScript.m_isVertical = isVertical;
            return dragScript;
        }
        #endregion

        #region �϶��Ĵ���
        /// <summary>
        /// ��ʼ�϶�
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(UnityEngineEventSystemsPointerEventData eventData)
        {
           
        }
        /// <summary>
        /// ����ȥ��λ������
        /// </summary>
        private UnityEngine.Vector2 firstPos;
        /// <summary>
        /// ̧��ʱ��λ������
        /// </summary>
        private UnityEngine.Vector2 lastPos;
        /// <summary>
        /// ��ק��λ������
        /// </summary>
        private UnityEngine.Vector2 dragPos;

        /// <summary>
        /// ��ק��ͼƬ
        /// </summary>
        private void CreateDragImage()
        {
            // ��ȡUI���ڵ�
            if (null == m_uiRootNode)
            {
                UnityGameObject rootGo = UnityGameObject.Find(GlobalVar.GAME_OBJECT_CANVAS);
                if (null == rootGo)
                {
                    return;
                }
                m_uiRootNode = rootGo.transform;
                if (null == m_uiRootNode)
                {
                    return;
                }
            }
            UnityEngineGameObject imgValue = null;
            if (null == m_movingImage)
            {
                // �����ƶ��ڵ�
                m_movingImage = new UnityEngineGameObject("MovingImage");
                imgValue = new UnityEngineGameObject("MovingImageValue");
                imgValue.transform.SetParent(m_movingImage.transform);
            }
            
            UnityEngineUIImage image = m_movingImage.AddComponent<UnityEngineUIImage>();
            UnityEngineCanvasGroup group = m_movingImage.AddComponent<UnityEngineCanvasGroup>();
			m_movingImage.transform.SetParent(m_uiRootNode, false);
            m_movingImage.transform.SetAsLastSibling();
            group.blocksRaycasts = false;
            image.sprite = m_imageDragBg.sprite;
            image.SetNativeSize();
            UnityEngineUIImage imageValue = imgValue.AddComponent<UnityEngineUIImage>();
            imageValue.sprite = m_imageDrag.sprite;
            imageValue.SetNativeSize();

            if (m_isRotateByBack)
            {
                m_draggingBack = transform.GetComponent<UnityEngineRectTransform>();
            }
            else
            {
                m_draggingBack = m_uiRootNode.GetComponent<UnityEngineRectTransform>();
            }

        }

        /// <summary>
        /// ��ָ����
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(UnityEngineEventSystemsPointerEventData eventData)
        {
            if (!m_isOpen)
            {
                return;
            }
            isOnClick = true;
            // startDrag = false;
            maxDragLength = 0;
            firstPos = GetMousePos();
        }
        /// <summary>
        /// �����ק����
        /// </summary>
        private float maxDragLength;
        /// <summary>
        /// ����¼�
        /// </summary>
        private bool isOnClick = true;
        // private bool startDrag = false;
        /// <summary>
        /// ����С����������ǵ��
        /// </summary>
        private const float ONCLICK_LENGTH = 10;
        /// <summary>
        /// �����϶�
        /// </summary>
        /// <param name="data"></param>
        public void OnDrag(UnityEngineEventSystemsPointerEventData data)
        {
            if (!m_isOpen)
            {
                return;
            }
           
            dragPos = GetMousePos();

            // �������󻬶���������
            if (m_isVertical)
            {
                if ((dragPos.x - firstPos.x) > maxDragLength)
                {
                    maxDragLength = dragPos.x - firstPos.x;
                }
            }
            //�������ϻ�����������
            else
            {                
                if ((dragPos.y - firstPos.y) > maxDragLength)
                {
                    maxDragLength = dragPos.y - firstPos.y;
                }
            }
            
            isOnClick = (ONCLICK_LENGTH > maxDragLength) && (maxDragLength > 0);

            if (!isOnClick && (maxDragLength > 0))
            {
                if (m_movingImage == null)
                { // ��ʼ�¼�֪ͨ
                    if (null != m_dragEventCallBack)
                    {
                        m_dragEventCallBack.Invoke(true,UnityEngineVector3.zero);
                    }
                    CreateDragImage();
                }
                SetDraggedPosition(data);
            }

        }

        /// <summary>
        /// ��ָ̧��
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(UnityEngineEventSystemsPointerEventData eventData)
        {
            if (!m_isOpen)
            {
                return;
            }
            lastPos = GetMousePos();

            if (isOnClick)
            {
                if (null != m_dragEventCallBack)
                {
                    m_dragEventCallBack.Invoke(false,UnityEngineVector3.zero);
                }
            }
            else
            {
                if (m_movingImage == null)
                {
                    return;
                }
                if (null != m_dragEventCallBack)
                {
                    UnityEngineVector3 nowPos = m_movingImage.transform.position;
                    m_dragEventCallBack.Invoke(false, nowPos);
                }
                Destroy(m_movingImage);
                m_movingImage = null;
            }
        }
        /// <summary>
        /// �϶�����
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(UnityEngineEventSystemsPointerEventData eventData)
        {
            //if (!m_isOpen)
            //{
            //    return;
            //}
            //if (m_movingImage == null)
            //{
            //    return;
            //}
            //if (null != m_dragEventCallBack)
            //{
            //    UnityEngineVector3 nowPos = m_movingImage.transform.position;
            //    m_dragEventCallBack.Call(false,nowPos);
            //}
            //Destroy(m_movingImage);
            //m_movingImage = null;
        }
        UnityEngine.Canvas canvase;
        /// <summary>
        /// ��ȡ���ĵ�ǰλ��
        /// </summary>
        /// <returns></returns>
        private UnityEngine.Vector2 GetMousePos()
        {
            if (null == canvase)
            {
                canvase = UnityGameObject.Find(GlobalVar.GAME_OBJECT_CANVAS).GetComponent<UnityEngine.Canvas>();
            }
            UnityEngine.Vector2 _pos = UnityEngine.Vector2.one;
            UnityEngineRectTransformUtility.ScreenPointToLocalPointInRectangle(canvase.transform as UnityEngineRectTransform, UnityEngine.Input.mousePosition, canvase.worldCamera, out _pos);
            return _pos;
        }

        /// <summary>
        /// �����ƶ�ͼƬ��λ��
        /// </summary>
        /// <param name="data"></param>
        private void SetDraggedPosition(UnityEngineEventSystemsPointerEventData data)
        {
            // ���²ο�����
            if (true == m_isRotateByBack && null != data.pointerEnter)
            {
                UnityEngineRectTransform rect = data.pointerEnter.GetComponent<UnityEngineRectTransform>();
                if (null != rect)
                {
                    m_draggingBack = rect;
                }
            }

            if (null == m_draggingBack)
            {
                return;
            }

            UnityEngineRectTransform imageRect = m_movingImage.GetComponent<UnityEngineRectTransform>();
            if (null == imageRect)
            {
                return;
            }
            // ת������
            UnityEngineVector3 globalMousePos;
            if (UnityEngineRectTransformUtility.ScreenPointToWorldPointInRectangle(m_draggingBack, data.position + m_dragOffset, data.pressEventCamera, out globalMousePos))
            {
                imageRect.position = globalMousePos;
                imageRect.rotation = m_draggingBack.rotation;
            }
        }
        #endregion

        #region �϶�ͼƬ�Ĵ���

        /// <summary>
        /// ���ٴ����Ļ���image
        /// </summary>
        public void DestroyMovingImage()
        {
            if (null != m_movingImage)
            {
                Destroy(m_movingImage);
                m_movingImage = null;
            }
        }

        /// <summary>
        /// �Ƿ񼤻��϶��ƶ�ͼƬ
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveMovingImage(bool isActive)
        {
            if (null == m_movingImage)
            {
                return;
            }
            m_movingImage.SetActive(isActive);
        }

        #endregion

        #region ����
        public bool IsOpen
        {
            set { m_isOpen = value; }
        }
        #endregion

        #region �ݴ��Դ���

        /// <summary>
        /// MonoBehaviour����
        /// </summary>
        private void OnDestroy()
        {
            DestroyMovingImage();
        }

        /// <summary>
        /// MonoBehaviour����   ������ͣ ����  ʱ����
        /// </summary>
        /// <param name="pause">�Ƿ���ͣ</param>
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                DestroyMovingImage();
                UnityEngine.Screen.sleepTimeout = UnityEngine.SleepTimeout.SystemSetting;
            }
            else
            {
                UnityEngine.Screen.sleepTimeout = UnityEngine.SleepTimeout.NeverSleep;
            }
        }

        /// <summary>
        /// MonoBehaviour����     ����ʧȥ  ���   ����ʱ����
        /// </summary>
        /// <param name="focus">�Ƿ��ý���</param>
        private void OnApplicationFocus(bool focus)
        {
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor)
            {
                if (!focus)
                {
                    DestroyMovingImage();
                }
            }
        }
     

        /// <summary>
        /// ��������
        /// </summary>
        ~ImageDrag()
        {
            DestroyMovingImage();
        }
        #endregion
    }
}
 