using System;
using System.Collections.Generic;
using UnityEngine;


namespace AP.UI
{
    [Serializable]
    public class APPanelWidgetInfo
    {
        public string name;
        public List<string> stringParams;
        public List<float> numParams;
    }
    
    public class APPanelWidgetUI : MonoBehaviour
    {
        public float Height => _rt.sizeDelta.y;
        
        private string _name;
        private RectTransform _rt;
        private APLeftPanelContentItemUI _contentItem;

        public void SetContent(APLeftPanelContentItemUI contentItem, Vector2 pos)
        {
            var sizeDelta = _rt.sizeDelta;
            var aspect = sizeDelta.x / sizeDelta.y;

            _rt.sizeDelta = new Vector2(contentItem.Width, contentItem.Width / aspect);
            _contentItem = contentItem;

            _rt.anchoredPosition3D = new Vector3(pos.x, pos.y, 0);
        }
        public void SetHide(float time, float delay = 0.0f, Action onComplete = null)
        {
            var desc = _rt.LeanMoveX(-_contentItem.Width, time);
            desc.setDelay(delay);
            desc.setOnComplete(onComplete);
        }
        public void SetShow(float time, float delay = 0.0f, Action onComplete = null)
        {
            _rt.anchoredPosition = new Vector2(_contentItem.Width, _rt.anchoredPosition.y);
            
            var desc = _rt.LeanMoveX(0, time);
            desc.setDelay(delay);
            desc.setOnComplete(onComplete);
        }
        public void OnHideComplete()
        {
            _rt.anchoredPosition = new Vector2(0, _rt.anchoredPosition.y);
        }
        public virtual void Init(APPanelWidgetInfo info)
        {
            
        }

        private void Awake()
        {
            _name = gameObject.name;
            _rt = GetComponent<RectTransform>();
        }
    }
}

