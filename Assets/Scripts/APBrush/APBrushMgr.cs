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
        private Color _color;

        private static readonly Shader WriteF0 = Shader.Find("DoWrite/WaterF0_Round");
        private static readonly Shader WriteF1234 = Shader.Find("DoWrite/WaterF1234_Round");
        private static readonly Shader WriteF5678 = Shader.Find("DoWrite/WaterF5678_Round");
        private static readonly Shader ColorShader = Shader.Find("DoWrite/Color_Round");

        private Material _matWrite0;
        private Material _matWrite1234;
        private Material _matWrite5678;
        private Material _matColor;
        
        private static readonly int Rect1 = Shader.PropertyToID("_rect");
        private static readonly int Color1 = Shader.PropertyToID("_color");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");

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
        public void DoWrite(Vector2 pos)
        {
            DoWrite(_curLayer, pos);
        }
        public void DoWrite(int layerId, Vector2 pos)
        {
            Vector4 rect = new Vector4(pos.x - _radius, pos.y - _radius, pos.x + _radius, pos.y + _radius);
            _matColor.SetColor(Color1, _color);
            
            _canvas[layerId]?.DoWrite((info) =>
            {
                var data = info as APLayerInfo;
                if (data == null) return;
                
                // 绑定资源
                _matWrite0.SetVector(Rect1, rect);
                _matWrite1234.SetVector(Rect1, rect);
                _matWrite5678.SetVector(Rect1, rect);
                _matColor.SetVector(Rect1, rect);
                
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


