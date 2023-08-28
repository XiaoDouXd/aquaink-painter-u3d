using UnityEngine;
using AP.Canvas;

namespace AP.Brush
{
    public class APBrushRoundSetInfo : APBrushSetInfoBase
    {
        public float radius = 0.08f;
        public float wet;
        public Color color;
        public float interval = 0.01f;
        public float soft;
        public float alphaAdd;
        public float shape;
        public float angle;
        public float pressure;

        public override void GetValue(APBrushSetInfoBase info)
        {
            if (info is not APBrushRoundSetInfo i) return;

            radius = i.radius;
            wet = i.wet;
            color = i.color;
            interval = i.interval;
            shape = i.shape;
            soft = i.soft;
            alphaAdd = i.alphaAdd;
            angle = i.angle;
            pressure = i.pressure;
        }
    }

    public class APBrushRound : APBrushBase
    {
        protected override APBrushSetInfoBase Info => _info;

        public APBrushRound(APCanvas canvas) : base(canvas)
        {
            _matWrite0 = new Material(WriteF0) { hideFlags = HideFlags.DontSave };
            _matWrite1234 = new Material(WriteF1234) { hideFlags = HideFlags.DontSave };
            _matWrite5678 = new Material(WriteF5678) { hideFlags = HideFlags.DontSave };
            _matColor = new Material(ColorShader) { hideFlags = HideFlags.DontSave };

            _matColor.SetTexture(ColTable, APInitMgr.I.colorTable);
            _matColor.SetBrushPreColorToMat(preWrite);
            _matWrite1234.SetBrushPreAlphaToMat(preWrite);
            _matWrite5678.SetBrushPreAlphaToMat(preWrite);
            _matWrite0.SetBrushPreAlphaToMat(preWrite);
            _preWriteMaterial.SetBrushPreAlphaToMat(preWrite);
            _preWriteAlphaMaterial.SetTexture(DoWriteTransPaperSub, APInitMgr.I.defaultPaperSub);
            _preWriteAlphaMaterial.SetVector(DoWriteTransPaperSubTrans,
                new Vector2(1, 1));

            _info = new APBrushRoundSetInfo();
        }

        public override void DoCreate(APCanvas canvasIn, int currLayer = -1)
        {
            base.DoCreate(canvasIn, currLayer);
            _matColor.SetBrushPreColorToMat(preWrite);
            _matWrite1234.SetBrushPreAlphaToMat(preWrite);
            _matWrite5678.SetBrushPreAlphaToMat(preWrite);
            _matWrite0.SetBrushPreAlphaToMat(preWrite);
        }

        public override void SetTex(params Texture[] tex)
        {
            if (tex.Length == 0) return;

            _preWriteAlphaMaterial.SetTexture(DoWriteTransBrush, tex[0]);
        }

        public void SetColor(Color color)
        {
            _info.color = color;
        }

        public void SetWet(float wet)
        {
            _info.wet = wet;
        }

        public void SetRadius(float radius)
        {
            _info.radius = radius;
        }

        public void SetBrushInterval(float interval)
        {
            _info.interval = interval;
        }

        public void SetSoftAndAlphaAdd(float soft, float alphaAdd)
        {
            _info.soft = soft;
            _info.alphaAdd = alphaAdd;
        }

        public void SetShapeFactor(float shape)
        {
            _info.shape = shape;
        }

        public void SetRotation(float angle)
        {
            _info.angle = angle;
        }

        public void SetPressure(float press)
        {
            _info.pressure = press;
        }

        public override void DoWriteDown(Vector2 pos)
        {
            base.DoWriteDown(pos);
            if (!_drawStart)
            {
                _posLast = pos;
                DoPreWrite(pos);
                _drawStart = true;
            }
            else
            {
                var vec = pos - _posLast;
                var dis = vec.magnitude;
                // 有得画的
                if (dis >= _info.interval)
                {
                    var times = Mathf.Floor(dis / _info.interval);
                    var norVec = vec.normalized;
                    for (var i = 0; i < times; i++)
                    {
                        _posLast += norVec * _info.interval;
                        DoPreWrite(_posLast);
                    }
                }
            }
        }

        public override void DoWriteUp(Vector2 pos)
        {
            if (!_drawStart) return;

            DoWriteDown(pos);
            DoWrite();
            _drawStart = false;
            base.DoWriteUp(pos);
        }

        protected override void SetPreWriteMat(Vector2 pos)
        {
            Vector4 rect = new Vector4(pos.x - _info.radius, pos.y - _info.radius, pos.x + _info.radius, pos.y + _info.radius);
            Vector2 rota = new Vector2(Mathf.Cos(_info.angle), Mathf.Sin(_info.angle));
            Vector2 wh = new Vector2(canvas.Width, canvas.Height);

            PreWriteAlphaMat.SetVector(DoWriteTransRect, rect);
            PreWriteAlphaMat.SetVector(DoWriteTransRotaWh, new Vector4(rota.x, rota.y, wh.x, wh.y));
            PreWriteAlphaMat.SetFloat(DoWriteTransAlphaAdd, _info.alphaAdd);
            PreWriteAlphaMat.SetFloat(DoWriteTransSoft, _info.soft);
            PreWriteAlphaMat.SetFloat(DoWriteTransPress, _info.pressure);
            PreWriteMat.SetColor(BrushColor, _info.color);
        }

        protected override Material PreWriteMat => _preWriteMaterial;
        protected override Material PreWriteAlphaMat => _preWriteAlphaMaterial;

        protected override void DoWrite()
        {
            // 绑定资源
            _matWrite0.SetFloat(Wet, _info.wet);
            _matWrite1234.SetFloat(Wet, _info.wet);
            _matWrite5678.SetFloat(Wet, _info.wet);
            _matColor.SetColor(BrushColor, _info.color);
            _preWriteMaterial.SetFloat(AlphaThreshold, _info.shape);

            canvas[curLayer]?.DoWrite((info) =>
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

            base.DoWrite();
        }

        // 等间距笔画实现
        private bool _drawStart;
        private Vector2 _posLast;
        private static readonly int AlphaThreshold = Shader.PropertyToID("_AlphaThreshold");
        private static readonly int DoWriteTransPaperSub = Shader.PropertyToID("_DoWriteTrans_paperSub");
        private static readonly int DoWriteTransPaperSubTrans = Shader.PropertyToID("_DoWriteTrans_paperSubTrans");
        private static readonly int DoWriteTransPress = Shader.PropertyToID("_DoWriteTrans_press");

        private APBrushRoundSetInfo _info;

        private static readonly Shader PreAlphaShader = Shader.Find("AP/PreWrite/Color_Round_Alpha");
        private static readonly Shader PreShader = Shader.Find("AP/PreWrite/Color_Round");
        private static readonly Shader WriteF0 = Shader.Find("DoWrite/WaterF0_Round");
        private static readonly Shader WriteF1234 = Shader.Find("DoWrite/WaterF1234_Round");
        private static readonly Shader WriteF5678 = Shader.Find("DoWrite/WaterF5678_Round");
        private static readonly Shader ColorShader = Shader.Find("DoWrite/Color_Round");

        private Material _matWrite0;
        private Material _matWrite1234;
        private Material _matWrite5678;
        private Material _matColor;

        private readonly Material _preWriteAlphaMaterial = new (PreAlphaShader) {hideFlags = HideFlags.DontSave};
        private readonly Material _preWriteMaterial = new (PreShader) {hideFlags = HideFlags.DontSave};
        private static readonly int BrushColor = Shader.PropertyToID("_color");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Wet = Shader.PropertyToID("_wet");
        private static readonly int DoWriteTransRect = Shader.PropertyToID("_DoWriteTrans_rect");
        private static readonly int DoWriteTransRotaWh = Shader.PropertyToID("_DoWriteTrans_rotaWH");
        private static readonly int DoWriteTransAlphaAdd = Shader.PropertyToID("_DoWriteTrans_alphaAdd");
        private static readonly int DoWriteTransBrush = Shader.PropertyToID("_DoWriteTrans_brush");
        private static readonly int DoWriteTransSoft = Shader.PropertyToID("_DoWriteTrans_soft");
    }
}


