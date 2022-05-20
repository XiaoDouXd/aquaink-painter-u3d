using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AP.Canvas
{
    public class APColorInfo : MapInfoBase
    {
        public Texture fix;
        public RenderTexture adv;
        public RenderTexture glue;
        public RenderTexture cTemp;
    }
    
    public class APColor : MapBase
    {
        public override Texture Tex => _tex;
        public override MapInfoBase Info => new APColorInfo() {
            fix = _colFix,
            adv = _colAdv,
            glue = _glue,
            cTemp = _rtTemp,
        };

        private RenderTexture _tex;
        private RenderTexture _glue;
        private RenderTexture _colAdv;
        private RenderTexture _colFix;
        private RenderTexture _rtTemp;

        private readonly Material _matShow;
        private readonly Material _matGlue;
        private readonly Material _matColAdv;
        private readonly Material _matColFix;

        private static readonly int Flow = Shader.PropertyToID("_Flow");
        private static readonly int Delta = Shader.PropertyToID("_Delta");
        private static readonly int Last = Shader.PropertyToID("_Last");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Adv = Shader.PropertyToID("_Adv");
        private static readonly int Fix = Shader.PropertyToID("_Fix");
        
        private static readonly Shader ColClearShader = Shader.Find("Canvas/Clear1110");
        private static readonly Shader ColAdvShader = Shader.Find("COL_UPD/ColUpdate_Adv");
        private static readonly Shader ColFixShader = Shader.Find("COL_UPD/ColUpdate_Fix");
        private static readonly Shader ColShowShader = Shader.Find("COL_UPD/ColUpdate_Show");
        private static readonly Shader GlueShader = Shader.Find("COL_UPD/ColUpdate_Glue");
        
        public APColor(int width, int height, APFlow d2Q9) : 
            base((uint)width, (uint)height, MapRankTypes.COLOR_FIX)
        {
            var d2Q9Map = d2Q9;

            _tex = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _colAdv = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _colFix = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _glue = new RenderTexture(width, height, 0, GraphicsFormat.R8_UNorm);
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _glue.Create();
            _colAdv.Create();
            _rtTemp.Create();
            _colFix.Create();
            _tex.Create();

            _matColAdv = new Material(ColAdvShader) { hideFlags = HideFlags.DontSave };
            _matColFix = new Material(ColFixShader) { hideFlags = HideFlags.DontSave };
            _matGlue = new Material(GlueShader) { hideFlags = HideFlags.DontSave };
            _matShow = new Material(ColShowShader) { hideFlags = HideFlags.DontSave };
            var matClear1110 = new Material(ColClearShader) { hideFlags = HideFlags.DontSave };
            Graphics.Blit(null, _colFix, matClear1110);
            Graphics.Blit(null, _colAdv, matClear1110);

            var delta = new Vector2(1.0f * width / 60000, 1.0f * height / 60000);
            _matColAdv.SetVector(Delta, delta);
            _matColAdv.SetTexture(Flow, d2Q9Map.Tex);
            _matColAdv.SetTexture(Last, _colAdv);
            _matColAdv.SetTexture(ColTable, APInitMgr.I.colorTable);
            _matColFix.SetTexture(Flow, d2Q9Map.Tex);
            _matColFix.SetTexture(Adv, _colAdv);
            _matColFix.SetTexture(Fix, _colFix);
            _matColFix.SetTexture(ColTable, APInitMgr.I.colorTable);
            _matGlue.SetVector(Delta, delta);
            _matGlue.SetTexture(Last, _glue);
            _matGlue.SetTexture(Flow, d2Q9Map.Tex);
            _matShow.SetTexture(Adv, _colAdv);
            _matShow.SetTexture(Fix, _colFix);
            _matShow.SetTexture(ColTable, APInitMgr.I.colorTable);
        }
        public override void DoUpdate()
        {
            base.DoUpdate();
            Graphics.Blit(null, _rtTemp, _matColAdv);
            Graphics.Blit(_rtTemp, _colAdv);
            Graphics.Blit(null, _rtTemp, _matColFix);
            Graphics.Blit(_rtTemp, _colFix);
            Graphics.Blit(null, _rtTemp, _matGlue);
            Graphics.Blit(_rtTemp, _glue);
            Graphics.Blit(null, _tex, _matShow);
        }
    }
}

