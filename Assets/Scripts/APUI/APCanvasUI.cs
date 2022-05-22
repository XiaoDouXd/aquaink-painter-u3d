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

    public class APCanvasUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler, IScrollHandler
    {
        public bool Inited => _inited;
        
        private APCanvas _canvas;
        private APBrushRound _brush;
        private RectTransform _background;
        private RawImage _surface;
        private double _proportion;
        private bool _moving;
        private bool _inited;
        private APLayerPersistentInfo _clearSave;

        public void Init(APCanvasInfo data)
        {
            _surface = transform.Find("DrawCanvas").GetComponent<RawImage>();
            _background = GetComponent<RectTransform>();

            _canvas = new APCanvas(data.width, data.height, _surface, data.paper);
            _brush = new APBrushRound(_canvas);
            
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

            _clearSave = _canvas[_canvas.FirstLayer].NewEmptyInfo() as APLayerPersistentInfo;
            APPersistentMgr.I.DoSave(new APPersistentOperation()
            {
                canvas = _canvas,
                layer = _canvas[_canvas.FirstLayer],
                op = APPersistentOp.DRAW
            });
            
            _brush?.SetTex(APInitMgr.I.brushTex1);
            _brush?.SetRadius(APSamplerMgr.I.PenSizeMin);
        }
        //--------------------------------------------------------- UI刷新
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
            _brush?.SetBrushInterval(APSamplerMgr.I.BrushInterval);
            
            if (Pen.current.pressure.IsActuated())
            {
                _brush?.SetRadius(Mathf.Lerp(
                        APSamplerMgr.I.PenSizeMin,
                        APSamplerMgr.I.PenSizeMax,
                        Pen.current.pressure.ReadValue()));
                    _brush?.SetWet(Mathf.Lerp(
                        APSamplerMgr.I.WetMin,
                        APSamplerMgr.I.WetMax,
                        Pen.current.pressure.ReadValue()
                    ));
                _brush?.SetSoftAndAlphaAdd(APSamplerMgr.I.Soft, 
                    Mathf.Lerp(
                        APSamplerMgr.I.AlphaAddMin,
                        APSamplerMgr.I.AlphaAdd,
                        Pen.current.pressure.ReadValue()));
            }
            else if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _brush?.SetRadius(APSamplerMgr.I.PenSizeMax);
                _brush?.SetWet(APSamplerMgr.I.WetMax);
                _brush?.SetSoftAndAlphaAdd(APSamplerMgr.I.Soft, APSamplerMgr.I.AlphaAddMin);
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                LerpResetSize();
            }

#if !UNITY_EDITOR
            if (Keyboard.current.ctrlKey.isPressed &&
                Keyboard.current.zKey.wasPressedThisFrame)
#else
            if (Keyboard.current.ctrlKey.isPressed &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
#endif
            {
                APPersistentMgr.I.GoBack(_canvas);
            }
#if !UNITY_EDITOR
            if (Keyboard.current.ctrlKey.isPressed &&
                Keyboard.current.zKey.wasPressedThisFrame &&
                Keyboard.current.shiftKey.isPressed)
#else
            if (Keyboard.current.ctrlKey.isPressed &&
                Keyboard.current.spaceKey.wasPressedThisFrame
                && Keyboard.current.altKey.isPressed)
#endif
            {
                APPersistentMgr.I.GoForward(_canvas);
            }
            
            if (Keyboard.current.ctrlKey.isPressed &&
                Keyboard.current.spaceKey.wasPressedThisFrame
                && Keyboard.current.shiftKey.isPressed)
            {
                _canvas[_canvas.FirstLayer].DoLoad(_clearSave);
            }
        }
        private void FixedUpdate()
        {
            _canvas.UpdateRenderData();
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
            
            if (
                eventData.button == PointerEventData.InputButton.Left 
            )
                Draw(eventData.position);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_inited) return;
            if (_moving) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Draw(eventData.position);
            }
                
        }
        private void Draw(Vector2 pos, bool up = false)
        {
            if (!_inited) return;
            if (_moving) return;
            var posC = APBrushMgr.Window2Canvas(_background, pos);
            if (posC.isInside)
            {
                if (up)
                {
                    _brush.DoWriteUp(posC.pos);
                }
                else
                {
                    _brush.DoWriteDown(posC.pos);
                }
            }
            else
            {
                _brush.DoWriteUp(posC.pos);
            }
        }
        public void OnScroll(PointerEventData eventData)
        {
            if (!_inited) return;
            if (_moving) return;
            _background.localScale += new Vector3(0.1f * eventData.scrollDelta.y, 0.1f * eventData.scrollDelta.y, 0);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _brush?.SetRadius(APSamplerMgr.I.PenSizeMin);
            Draw(eventData.position, true);
            APPersistentMgr.I.DoSave(new APPersistentOperation()
            {
                canvas = _canvas,
                layer = _canvas[_canvas.FirstLayer],
                op = APPersistentOp.DRAW
            });
        }
    }
}

