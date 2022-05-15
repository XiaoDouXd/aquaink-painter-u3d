using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AP.Canvas
{
    public class APColor : MapBase
    {
        public override Texture Tex => _colFix;
        public RenderTexture colAdv => _colAdv;
        public RenderTexture colGlue => _glue;

        private RenderTexture _glue;
        private RenderTexture _colAdv;
        private RenderTexture _colFix;
        private RenderTexture _rtTemp;

        private readonly Material _matGlue;
        private readonly Material _matColAdv;
        private readonly Material _matColFix;

        private static readonly int LastTex0 = Shader.PropertyToID("_LastTex0");
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex1234");
        private static readonly int LastTex5678 = Shader.PropertyToID("_LastTex5678");
        private static readonly int Delta = Shader.PropertyToID("_Delta");
        private static readonly int Last = Shader.PropertyToID("_Last");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Adv = Shader.PropertyToID("_Adv");
        private static readonly int Fix = Shader.PropertyToID("_Fix");
        
        private static readonly Shader ColAdvShader = Shader.Find("COL_UPD/ColUpdate_Adv");
        private static readonly Shader ColFixShader = Shader.Find("COL_UPD/ColUpdate_Fix");
        private static readonly Shader GlueShader = Shader.Find("COL_UPD/ColUpdate_Glue");

        public APColor(int width, int height, APFlow d2Q9) : base((uint)width, (uint)height, MapRankTypes.COLOR_FIX)
        {
            var d2Q9Map = d2Q9;

            _colAdv = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _colFix = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _glue = new RenderTexture(width, height, 0, GraphicsFormat.R8_UNorm);
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _glue.Create();
            _colAdv.Create();
            _rtTemp.Create();
            _colFix.Create();

            _matColAdv = new Material(ColAdvShader) { hideFlags = HideFlags.DontSave };
            _matColFix = new Material(ColFixShader) { hideFlags = HideFlags.DontSave };
            _matGlue = new Material(GlueShader) { hideFlags = HideFlags.DontSave };

            var delta = new Vector2(1.0f / Width, 1.0f / Height);
            _matColAdv.SetVector(Delta, delta);
            _matColAdv.SetTexture(LastTex0, d2Q9Map.F0);
            _matColAdv.SetTexture(LastTex1234, d2Q9Map.F1234);
            _matColAdv.SetTexture(LastTex5678, d2Q9Map.F5678);
            _matColAdv.SetTexture(Last, _colAdv);
            _matColAdv.SetTexture(ColTable, APInitMgr.I.colorTable);
            _matColFix.SetTexture(LastTex0, d2Q9Map.F0);
            _matColFix.SetTexture(LastTex1234, d2Q9Map.F1234);
            _matColFix.SetTexture(LastTex5678, d2Q9Map.F5678);
            _matColFix.SetTexture(Adv, _colAdv);
            _matColFix.SetTexture(Fix, _colFix);
            _matColFix.SetTexture(ColTable, APInitMgr.I.colorTable);
            _matGlue.SetVector(Delta, delta);
            _matGlue.SetTexture(Last, _glue);
            _matGlue.SetTexture(LastTex0, d2Q9Map.F0);
            _matGlue.SetTexture(LastTex1234, d2Q9Map.F1234);
            _matGlue.SetTexture(LastTex5678, d2Q9Map.F5678);
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
        }
    }
}

