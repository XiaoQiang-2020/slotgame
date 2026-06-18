using System;
using System.Collections.Generic;
using UniFramework.Event;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Patch
{
    public class PatchWindow : MonoBehaviour
    {
        private sealed class MessageBox
        {
            private GameObject _cloneObject;
            private Text _content;
            private Button _btnOK;
            private Action _clickOK;

            public bool ActiveSelf => _cloneObject != null && _cloneObject.activeSelf;

            public void Create(GameObject cloneObject)
            {
                _cloneObject = cloneObject;
                _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
                _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
                _btnOK.onClick.AddListener(OnClickOK);
            }

            public void Show(string content, Action clickOK)
            {
                _content.text = content;
                _clickOK = clickOK;
                _cloneObject.SetActive(true);
                _cloneObject.transform.SetAsLastSibling();
            }

            public void Hide()
            {
                _content.text = string.Empty;
                _clickOK = null;
                _cloneObject.SetActive(false);
            }

            private void OnClickOK()
            {
                _clickOK?.Invoke();
                Hide();
            }
        }

        private readonly EventGroup _eventGroup = new EventGroup();
        private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

        private GameObject _messageBoxObj;
        private Slider _slider;
        private Text _tips;

        private void Awake()
        {
            _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
            _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
            _tips.text = "Initializing the game world.";
            _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
            _messageBoxObj.SetActive(false);

            _eventGroup.AddListener<PatchInitializeFailedEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchStepChangedEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchFoundUpdateFilesEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchDownloadUpdatedEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchPackageVersionRequestFailedEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchPackageManifestUpdateFailedEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchWebFileDownloadFailedEvent>(OnHandleEventMessage);
        }

        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();
        }

        private void OnHandleEventMessage(IEventMessage message)
        {
            switch (message)
            {
                case PatchInitializeFailedEvent:
                    ShowMessageBox("Failed to initialize package.", UserTryInitializePackageEvent.SendEventMessage);
                    break;
                case PatchStepChangedEvent stepChanged:
                    _tips.text = stepChanged.Tips;
                    Debug.Log(stepChanged.Tips);
                    break;
                case PatchFoundUpdateFilesEvent foundUpdateFiles:
                    ShowFoundUpdateFiles(foundUpdateFiles);
                    break;
                case PatchDownloadUpdatedEvent downloadUpdated:
                    ShowDownloadProgress(downloadUpdated);
                    break;
                case PatchPackageVersionRequestFailedEvent:
                    ShowMessageBox("Failed to request package version. Check the network status.", UserTryRequestPackageVersionEvent.SendEventMessage);
                    break;
                case PatchPackageManifestUpdateFailedEvent:
                    ShowMessageBox("Failed to update package manifest. Check the network status.", UserTryUpdatePackageManifestEvent.SendEventMessage);
                    break;
                case PatchWebFileDownloadFailedEvent downloadFailed:
                    ShowMessageBox($"Failed to download file: '{downloadFailed.FileName}'.", UserTryDownloadWebFilesEvent.SendEventMessage);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported patch window event message type: {message.GetType().FullName}.");
            }
        }

        private void ShowFoundUpdateFiles(PatchFoundUpdateFilesEvent message)
        {
            float sizeMB = message.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"Update files were found. Total count: {message.TotalCount}. Total size: {totalSizeMB} MB.", UserBeginDownloadWebFilesEvent.SendEventMessage);
        }

        private void ShowDownloadProgress(PatchDownloadUpdatedEvent message)
        {
            _slider.value = (float)message.CurrentDownloadCount / message.TotalDownloadCount;
            string currentSizeMB = (message.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (message.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            _tips.text = $"{message.CurrentDownloadCount}/{message.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }

        private void ShowMessageBox(string content, Action ok)
        {
            MessageBox msgBox = null;
            for (int i = 0; i < _msgBoxList.Count; i++)
            {
                if (_msgBoxList[i].ActiveSelf == false)
                {
                    msgBox = _msgBoxList[i];
                    break;
                }
            }

            if (msgBox == null)
            {
                msgBox = new MessageBox();
                var cloneObject = Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
                msgBox.Create(cloneObject);
                _msgBoxList.Add(msgBox);
            }

            msgBox.Show(content, ok);
        }
    }
}
