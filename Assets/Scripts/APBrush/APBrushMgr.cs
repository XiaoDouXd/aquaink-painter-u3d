using AP.Canvas;
using UnityEngine;

namespace AP.Brush
{
    public class APBrushMgr
    {
        public static APBrushMgr I
        {
            get
            {
                if (_i == null)
                    _i = new APBrushMgr();
                return _i;
            }
        }
        private static APBrushMgr _i;
    }

    public class APBrush
    {
        private APCanvas _canvas;
        private int _curLayer;
        private float _radius = 0.08f;
        private float _wet;
        private Color _color;

        private static readonly Shader WriteF0 = Shader.Find("DoWrite/WaterF0_Round");
        private static readonly Shader WriteF1234 = Shader.Find("DoWrite/WaterF1234_Round");
        private static readonly Shader WriteF5678 = Shader.Find("DoWrite/WaterF5678_Round");
        private static readonly Shader ColorShader = Shader.Find("DoWrite/Color_Round");

        private Material _matWrite0;
        private Material _matWrite1234;
        private Material _matWrite5678;
        private Material _matColor;
        
        private static readonly int BrushColor = Shader.PropertyToID("_color");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Wet = Shader.PropertyToID("_wet");
        private static readonly int DoWriteTransRect = Shader.PropertyToID("_DoWriteTrans_rect");
        private static readonly int DoWriteTransRotaWh = Shader.PropertyToID("_DoWriteTrans_rotaWH");
        private static readonly int DoWriteTransAlphaAdd = Shader.PropertyToID("_DoWriteTrans_alphaAdd");
        private static readonly int DoWriteTransBrush = Shader.PropertyToID("_DoWriteTrans_brush");
        private static readonly int DoWriteTransSoft = Shader.PropertyToID("_DoWriteTrans_soft");

        public APBrush(APCanvas canvas)
        {
            _canvas = canvas;
            _curLayer = _canvas.FirstLayer;

            _matWrite0 = new Material(WriteF0) { hideFlags = HideFlags.DontSave };
            _matWrite1234 = new Material(WriteF1234) { hideFlags = HideFlags.DontSave };
            _matWrite5678 = new Material(WriteF5678) { hideFlags = HideFlags.DontSave };
            _matColor = new Material(ColorShader) { hideFlags = HideFlags.DontSave };
            
            _matColor.SetTexture(ColTable, APInitMgr.I.colorTable);
        }

        public void SetColor(Color color)
        {
            _color = color;
        }
        public void SetWet(float wet)
        {
            _wet = wet;
        }
        public void SetRadius(float radius)
        {
            _radius = radius;
        }
        public void SetTex(Texture tex)
        {
            _matColor.SetTexture(DoWriteTransBrush, tex);
            _matWrite1234.SetTexture(DoWriteTransBrush, tex);
            _matWrite5678.SetTexture(DoWriteTransBrush, tex);
            _matWrite0.SetTexture(DoWriteTransBrush, tex);
        }

        public void SetBrushInterval(float interval)
        {
            _interval = interval;
        }
        
        // 等间距笔画实现
        private float _interval = 0.01f;
        private bool _drawStart;
        private Vector2 _posLast;
        public void WriteDown(Vector2 pos)
        {
            if (!_drawStart)
            {
                _posLast = pos;
                DoWrite(_curLayer, pos);
                _drawStart = true;
            }
            else
            {
                var vec = pos - _posLast;
                var dis = vec.magnitude;
                // 有得画的
                if (dis >= _interval)
                {
                    var times = Mathf.Floor(dis / _interval);
                    var norVec = vec.normalized;
                    for (var i = 0; i < times; i++)
                    {
                        _posLast += norVec * _interval;
                        DoWrite(_curLayer, _posLast);
                    }
                }
            }
        }

        public void WriteUp(Vector2 pos)
        {
            if (!_drawStart) return;
            
            WriteDown(pos);
            _drawStart = false;
        }

        private void DoWrite(int layerId, Vector2 pos)
        {
            Vector4 rect = new Vector4(pos.x - _radius, pos.y - _radius, pos.x + _radius, pos.y + _radius);
            Vector2 rota = new Vector2(Mathf.Cos(Time.time), Mathf.Sin(Time.time));
            Vector2 wh = new Vector2(_canvas.Width, _canvas.Height);

            // 绑定资源
            _matWrite0.SetVector(DoWriteTransRect, rect);
            _matWrite0.SetVector(DoWriteTransRotaWh, new Vector4(rota.x, rota.y, wh.x, wh.y));
            _matWrite0.SetFloat(DoWriteTransAlphaAdd, -0);
            _matWrite0.SetFloat(Wet, _wet);
            _matWrite0.SetFloat(DoWriteTransSoft, 0);
            _matWrite1234.SetVector(DoWriteTransRect, rect);
            _matWrite1234.SetVector(DoWriteTransRotaWh, new Vector4(rota.x, rota.y, wh.x, wh.y));
            _matWrite1234.SetFloat(DoWriteTransAlphaAdd, -0);
            _matWrite1234.SetFloat(Wet, _wet);
            _matWrite1234.SetFloat(DoWriteTransSoft, 0);
            _matWrite5678.SetVector(DoWriteTransRect, rect);
            _matWrite5678.SetVector(DoWriteTransRotaWh, new Vector4(rota.x, rota.y, wh.x, wh.y));
            _matWrite5678.SetFloat(DoWriteTransAlphaAdd, -0);
            _matWrite5678.SetFloat(Wet, _wet);
            _matWrite5678.SetFloat(DoWriteTransSoft, 0);
            _matColor.SetVector(DoWriteTransRect, rect);
            _matColor.SetVector(DoWriteTransRotaWh, new Vector4(rota.x, rota.y, wh.x, wh.y));
            _matColor.SetFloat(DoWriteTransAlphaAdd, -0);
            _matColor.SetColor(BrushColor, _color);
            _matColor.SetFloat(DoWriteTransSoft, 0);
            
            _canvas[layerId]?.DoWrite((info) =>
            {
                var data = info as APLayerInfo;
                if (data == null) return;

                // 写入
                Graphics.Blit(data.f0, data.fTemp, _matWrite0);
                Graphics.Blit(data.fTemp, data.f0);
                Graphics.Blit(data.f1234, data.fTemp, _matWrite1234);
                Graphics.Blit(data.fTemp, data.f1234);
                Graphics.Blit(data.f5678, data.fTemp, _matWrite5678);
                Graphics.Blit(data.fTemp, data.f5678);
                Graphics.Blit(data.color, data.cTemp, _matColor);
                Graphics.Blit(data.cTemp, data.color);
            });
        }
        public static (Vector2 pos, bool isInside) Window2Canvas(RectTransform canvasTrans, Vector2 windowPos)
        {
            windowPos -= APInitMgr.I.WindowCenter + canvasTrans.anchoredPosition;
            
            if (!canvasTrans)
                return (windowPos, false);

            var rect = canvasTrans.rect;
            var localScale = canvasTrans.localScale;
            var x = (windowPos.x - rect.xMin * localScale.x) / (localScale.x * rect.width);
            var y = (windowPos.y - rect.yMin * localScale.y) / (localScale.y * rect.height);

            return (new Vector2(x, y), x <= 1 && x >= 0 && y <= 1 && y >= 0);
        }
    }
}


