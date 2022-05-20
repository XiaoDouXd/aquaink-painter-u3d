using System.Collections.Generic;
using UnityEngine;

namespace AP.UI
{
    public class APLeftPanelContentItemUI : MonoBehaviour
    {
        #region Inspector
        public bool dragEnable;
        public float hideOffset;
        [Tooltip("组件间隔")]
        public float interval = 10;
        [Space(4)] 
        [Tooltip("遮挡用平面")]
        public RectTransform maskPlane;

        [Space]
        [Tooltip("面板中的具体组件")]
        public List<APPanelWidgetInfo> widgets = new List<APPanelWidgetInfo>();
        #endregion
        
        public float Width => _width;
        
        private RectTransform _rt;
        private bool _isActive;
        private float _width;
        private List<APPanelWidgetUI> _widgets = new List<APPanelWidgetUI>();
        private APLeftPanelContentUI _content;
        private APLeftPanelTabItemUI _tab;
    
        public void Show()
        {
            _isActive = true;
            gameObject.SetActive(_isActive);
            SetShow();
            maskPlane.gameObject.SetActive(true);
        }
        public void OnShowComplete()
        {
            _content.OnItemShowComplete(_tab, this);
            maskPlane.gameObject.SetActive(false);
        }
        public void Exit()
        {
            SetHide();
            maskPlane.gameObject.SetActive(true);
        }
        public void OnExitComplete()
        {
            _isActive = false;
            _content.OnItemExitComplete(_tab, this);
            gameObject.SetActive(_isActive);
            maskPlane.gameObject.SetActive(false);
        }

        public void SetContent(APLeftPanelContentUI content, APLeftPanelTabItemUI tab)
        {
            _content = content;
            _tab = tab;
        }
    
        #region Build_in
        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _isActive = false;
        }
        private void Start()
        {
            _width = _rt.sizeDelta.x;
            
            // 加载游戏物体
            foreach (var widget in widgets)
            {
                var obj = APAssetObjMgr.UIObjs.Clone(widget.name, gameObject);
                if (obj.TryGetComponent(typeof(APPanelWidgetUI), out var widgetUI))
                {
                    var w = widgetUI as APPanelWidgetUI;
                    if (w)
                    {
                        w.Init(widget);
                        _widgets.Add(w);
                    }
                }
                    
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning("APPanelContent.Start: 有未加载物体！");
                }
#endif
            }
            
            // 设置游戏物体
            var height = 0.0f;
            foreach (var widget in _widgets)
            {
                widget.SetContent(this, new Vector2(0, height));
                height -= widget.Height + interval;
            }
            
            SetHideImmediately();
        }
        #endregion

        private void SetHide()
        {
            var delay = 0.0f;
            for (var i = 0; i < _widgets.Count; i++)
            {
                if (i == _widgets.Count - 1)
                    _widgets[i].SetHide(0.1f, delay, OnExitComplete);
                else
                    _widgets[i].SetHide(0.1f, delay);
                delay += 0.01f;
            }
        }
        private void SetShow()
        {
            gameObject.SetActive(true);
            
            var delay = 0.0f;
            for (var i = 0; i < _widgets.Count; i++)
            {
                if (i == _widgets.Count - 1)
                    _widgets[i].SetShow(0.1f, delay, OnShowComplete);
                else
                    _widgets[i].SetShow(0.1f, delay);
                delay += 0.01f;
            }
            
            _rt.anchoredPosition = new Vector2(0, _rt.anchoredPosition.y);
        }
        private void SetHideImmediately()
        {
            _rt.anchoredPosition = new Vector2(hideOffset, _rt.anchoredPosition.y);
            foreach (var widget in _widgets)
            {
                widget.OnHideComplete();
            }
            gameObject.SetActive(false);
        }
    }
}

