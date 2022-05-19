using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AP.UI
{
    public class APColorPickerRectUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IColorPickerComponent
    {
        public Vector2 SV
        {
            get => _sv;
            private set
            {
                _sv = value;
                UpdateRect();
            }
        }
        public float H
        {
            get => _h;
            private set
            {
                _h = value;
                UpdateRect();
            }
        }

        private APColorPickerUI _picker;
        private Vector2 _sv;
        private float _h;
        private RawImage _rect;
        private RectTransform _rt;
        private static readonly int TargetPos = Shader.PropertyToID("_TargetPos");

        private void UpdateRect()
        {
            var pos = _sv;
            pos -= Vector2.one * (Vector2.one - pos);
            _rect.material.SetVector(TargetPos, new Vector4(pos.x, pos.y, _h, 0));
            _picker.UpdateAllByRect(new Vector3(_h, _sv.x, _sv.y));
        }

        private void Awake()
        {
            _rect = GetComponent<RawImage>();
            _rt = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            var pos = APUIMgr.I.MousePos2Rect(_rt).pos;
            SV = APUIMgr.I.Clamp01(pos);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var pos = APUIMgr.I.MousePos2Rect(_rt).pos;
            SV = APUIMgr.I.Clamp01(pos);
        }

        public void SetPicker(APColorPickerUI picker)
        {
            _picker = picker;
        }
        public void UpdateColor()
        {
            _sv = new Vector2(_picker.HSV.y, _picker.HSV.z);
            _h = _picker.HSV.x;
            var pos = _sv;
            pos -= Vector2.one * (Vector2.one - pos);
            _rect.material.SetVector(TargetPos, new Vector4(pos.x, pos.y, _h, 0));
        }
    }
}
