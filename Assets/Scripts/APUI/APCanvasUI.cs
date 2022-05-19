using AP.Brush;
using AP.Canvas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        public bool Inited => _inited;
        
        private APCanvas _canvas;
        private APBrush _brush;
        private RectTransform _background;
        private RawImage _surface;
        private double _proportion;
        private bool _moving;
        private bool _inited;

        public void Init(APCanvasInfo data)
        {
            _surface = transform.Find("DrawCanvas").GetComponent<RawImage>();
            _background = GetComponent<RectTransform>();

            _canvas = new APCanvas(data.width, data.height, _surface, data.paper);
            _brush = new APBrush(_canvas);
            
            // 根据窗口大小设置初始大小
            _proportion = (double)_canvas.Width / _canvas.Height;
            var windowProp = (double)APInitMgr.I.WindowSize.x / APInitMgr.I.WindowSize.y;

            _background.anchoredPosition = new Vector2(0, 0);
            _background.localScale = new Vector3(1, 1, 1);
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
            
            _inited = true;
        }

        //--------------------------------------------------------- UI刷新
        private void ResetSize()
        {
            if (!_inited) return;
            
            _proportion = (double)_canvas.Width / _canvas.Height;
            var windowProp = (double)APInitMgr.I.WindowSize.x / APInitMgr.I.WindowSize.y;

            _background.anchoredPosition = new Vector2(0, 0);
            _background.localScale = new Vector3(1, 1, 1);
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
        private void LerpResetSize()
        {
            if (!_inited) return;
            if (_moving) return;
            
            _moving = true;
            LeanTween.scale(_background, Vector3.one, 0.1f).setEase(LeanTweenType.easeInOutSine);
            var desc = LeanTween.move(_background, Vector3.zero, 0.1f);
            desc.setEase(LeanTweenType.easeInOutSine);
            desc.setOnComplete(() =>
            {
                _moving = false;
            });
            
            var windowProp = (double)APInitMgr.I.WindowSize.x / APInitMgr.I.WindowSize.y;
            if (windowProp >= _proportion)
            {
                // 找到合适的高
                var h = APInitMgr.I.WindowSize.y * 0.8f;
                LeanTween.size(_background,new Vector2((float)(h * _proportion), h), 0.1f)
                    .setEase(LeanTweenType.easeInOutSine);
            }
            else
            {
                // 找到合适的宽
                var w = APInitMgr.I.WindowSize.x * 0.8f;
                LeanTween.size(_background,new Vector2(w , (float)(w / _proportion)), 0.1f)
                    .setEase(LeanTweenType.easeInOutSine);
            }
        }
        private void Update()
        {
            if (!_inited) return;
            
            _brush?.SetColor(APSamplerMgr.I.CurColor);

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                LerpResetSize();
            }
        }
        
        //--------------------------------------------------------- 画布交互
        public void OnDrag(PointerEventData eventData)
        {
            if (!_inited) return;
            if (_moving) return;
            
            if (eventData.button == PointerEventData.InputButton.Middle)
            {
                _background.anchoredPosition += eventData.delta;
                return;
            }
            
            if (eventData.button == PointerEventData.InputButton.Left)
                Draw(eventData.position);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_inited) return;
            if (_moving) return;

            if (eventData.button == PointerEventData.InputButton.Left)
                Draw(eventData.position);
        }
        private void Draw(Vector2 pos)
        {
            if (!_inited) return;
            if (_moving) return;
            
            var posC = APBrush.Window2Canvas(_background, pos);
            if (posC.isInside)
            {
                _brush.DoWrite(posC.pos);
            }
        }
        public void OnScroll(PointerEventData eventData)
        {
            if (!_inited) return;
            if (_moving) return;
            _background.localScale += new Vector3(0.1f * eventData.scrollDelta.y, 0.1f * eventData.scrollDelta.y, 0);
        }
    }
}

