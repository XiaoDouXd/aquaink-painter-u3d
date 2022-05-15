using AP.Brush;
using AP.Canvas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace AP.UI
{
    public class APCanvasInfo
    {
        public int width;
        public int height;
        public Texture paper;
    }

    public class APCanvasUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IScrollHandler
    {
        [FormerlySerializedAs("color")] public Color brushColor;

        private APCanvas _canvas;
        private APBrush _brush;
        private RectTransform _background;
        private RawImage _surface;
        private double _proportion;

        public void Init(APCanvasInfo data)
        {
            _surface = transform.Find("DrawCanvas").GetComponent<RawImage>();
            _background = GetComponent<RectTransform>();

            _canvas = new APCanvas(data.width, data.height, _surface, data.paper);
            _brush = new APBrush(_canvas);
            
            // 根据窗口大小设置初始大小
            _proportion = (double)data.width / data.height;
            var windowProp = (double)APInitMgr.I.WindowSize.x / APInitMgr.I.WindowSize.y;
            if (windowProp >= _proportion)
            {
                // 找到合适的高
                var h = APInitMgr.I.WindowSize.y * 0.8f;
                _background.sizeDelta = new Vector2((float)(h * _proportion) , h);

            }
            else
            {
                // 找到合适的宽
                var w = APInitMgr.I.WindowSize.x * 0.8f;
                _background.sizeDelta = new Vector2(w , (float)(w / _proportion));
            }
        }
        public void Update()
        {
            _brush?.SetColor(brushColor);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                Input.GetKey(KeyCode.Alpha0))
            {
                _proportion = (double)_canvas.Width / _canvas.Height;
                var windowProp = (double)APInitMgr.I.WindowSize.x / APInitMgr.I.WindowSize.y;
                if (windowProp >= _proportion)
                {
                    // 找到合适的高
                    var h = APInitMgr.I.WindowSize.y * 0.8f;
                    _background.sizeDelta = new Vector2((float)(h * _proportion) , h);

                }
                else
                {
                    // 找到合适的宽
                    var w = APInitMgr.I.WindowSize.x * 0.8f;
                    _background.sizeDelta = new Vector2(w , (float)(w / _proportion));
                }
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (APHotKeyMgr.I.dragging || eventData.button == PointerEventData.InputButton.Middle)
            {
                _background.anchoredPosition += eventData.delta;
                return;
            }
            
            if (eventData.button == PointerEventData.InputButton.Left)
                Draw(eventData.position);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (APHotKeyMgr.I.dragging)
            {
                _background.anchoredPosition += eventData.delta;
                return;
            }
            
            if (eventData.button == PointerEventData.InputButton.Left)
                Draw(eventData.position);
        }
        
        private void Draw(Vector2 pos)
        {
            var posC = APBrush.Window2Canvas(_background, pos);
            if (posC.Item2)
            {
                _brush.DoWrite(posC.Item1);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            _background.localScale += new Vector3(0.1f * eventData.scrollDelta.y, 0.1f * eventData.scrollDelta.y, 0);
        }
    }
}

