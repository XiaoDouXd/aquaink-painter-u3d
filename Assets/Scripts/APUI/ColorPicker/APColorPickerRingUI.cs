using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace AP.UI
{
    public class APColorPickerRingUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IColorPickerComponent
    {
        public float H
        {
            get => _h;
            private set
            {
                _h = value;
                UpdateRing();
            }
        }


        private APColorPickerUI _picker;
        private float _h;
        private RectTransform _rt;
        private RawImage _ring;

        private static readonly int Hue = Shader.PropertyToID("_Hue");

        private void UpdateRing()
        {
            _ring.material.SetFloat(Hue, _h);
            _picker.UpdateAllByRing(new Vector3(_h, _picker.HSV.y, _picker.HSV.z));
        }

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _ring = GetComponent<RawImage>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            var pos = APUIMgr.I.MousePos2Rect(_rt).pos;
            pos = APUIMgr.I.Clamp01(pos);
            pos -= Vector2.one * (Vector2.one - pos);

            var sAngle = Vector2.SignedAngle(Vector2.right, pos);
            var angle = sAngle < 0 ? sAngle + 360.0f : sAngle;
            H = angle / 360.0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var pos = APUIMgr.I.MousePos2Rect(_rt).pos;
            pos = APUIMgr.I.Clamp01(pos);
            pos -= Vector2.one * (Vector2.one - pos);

            var sAngle = Vector2.SignedAngle(Vector2.right, pos);
            var angle = sAngle < 0 ? sAngle + 360.0f : sAngle;
            H = angle / 360.0f;
        }

        public void SetPicker(APColorPickerUI picker)
        {
            _picker = picker;
        }

        public void UpdateColor()
        {
            _h = _picker.HSV.x;
            _ring.material.SetFloat(Hue, _h);
        }
    }
}
