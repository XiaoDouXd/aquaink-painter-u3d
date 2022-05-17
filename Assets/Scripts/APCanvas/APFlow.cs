using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace AP.Canvas
{
    public class APFlowInfo : MapInfoBase
    {
        public RenderTexture f0;
        public RenderTexture f1234;
        public RenderTexture f5678;
        public RenderTexture fTemp;
    }
    
    /// <summary>
    /// 基于D2Q9的流体更新
    /// </summary>
    public class APFlow : MapBase
    {
        public override Texture Tex => _tex;
        public override MapInfoBase Info => new APFlowInfo() {
            f0 = _rtF0,
            f1234 = _rtF1234,
            f5678 = _rtF5678,
            fTemp = _rtTemp
        };

        private Texture _colFix;
        private Texture _glue;
        private Texture _paper;
        private RenderTexture _tex;
        private RenderTexture _rtF0;
        private RenderTexture _rtF1234;
        private RenderTexture _rtF5678;
        private RenderTexture _rtTemp;
        
        private readonly Material _d2Q9MatF0;
        private readonly Material _d2Q9MatF1234;
        private readonly Material _d2Q9MatF5678;
        private readonly Material _showMat;

        private static readonly int LastTex0 = Shader.PropertyToID("_LastTex0");
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex1234");
        private static readonly int LastTex5678 = Shader.PropertyToID("_LastTex5678");
        private static readonly int Delta = Shader.PropertyToID("_Delta");
        private static readonly int Paper = Shader.PropertyToID("_Paper");
        private static readonly int Glue = Shader.PropertyToID("_Glue");
        private static readonly int ColFix = Shader.PropertyToID("_Fix");

        private static readonly Shader ShowShader = Shader.Find("LBM_SRT/D2Q9Update_Show");
        private static readonly Shader F0Shader = Shader.Find("LBM_SRT/D2Q9Update_F0");
        private static readonly Shader F1234Shader = Shader.Find("LBM_SRT/D2Q9Update_F1234");
        private static readonly Shader F5678Shader = Shader.Find("LBM_SRT/D2Q9Update_F5678");

        public APFlow(int width, int height, Texture paper) : base((uint)width, (uint)height, MapRankTypes.WATER_FLOW)
        {
            _paper = paper;

            // 初始化材质
            _d2Q9MatF0 = new Material(F0Shader) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF1234 = new Material(F1234Shader) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF5678 = new Material(F5678Shader) { hideFlags = HideFlags.DontSave };
            _showMat = new Material(ShowShader) { hideFlags = HideFlags.DontSave };

            // 初始化贴图
            _tex = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtF0 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16_SFloat) { filterMode = FilterMode.Point };
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtF5678 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _tex.Create();
            _rtTemp.Create();
            _rtF0.Create();
            _rtF1234.Create();
            _rtF5678.Create();
        
            // 设置贴图到材质
            var delta = new Vector2(1.0f / Width, 1.0f / Height);
            _d2Q9MatF0.SetVector(Delta, delta);
            _d2Q9MatF0.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF0.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF0.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF1234.SetVector(Delta, delta);
            _d2Q9MatF1234.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF1234.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF1234.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF5678.SetVector(Delta, delta);
            _d2Q9MatF5678.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF5678.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF5678.SetTexture(LastTex5678, _rtF5678);
            
            _showMat.SetTexture(LastTex0, _rtF0);
            _showMat.SetTexture(LastTex1234, _rtF1234);
            _showMat.SetTexture(LastTex5678, _rtF5678);

            SetPaper(paper);
        }
        public override void DoUpdate()
        {
            base.DoUpdate();
        
            Graphics.Blit(null, _rtTemp, _d2Q9MatF0);
            Graphics.Blit(_rtTemp, _rtF0);
            Graphics.Blit(null, _rtTemp, _d2Q9MatF1234);
            Graphics.Blit(_rtTemp, _rtF1234);
            Graphics.Blit(null, _rtTemp, _d2Q9MatF5678);
            Graphics.Blit(_rtTemp, _rtF5678);
            Graphics.Blit(null, _tex, _showMat);
        }
        public void SetGlueAndFix(APColor col)
        {
            var colInfo = col.Info as APColorInfo;
            _glue = colInfo?.glue;
            _colFix = colInfo?.fix;
            
            _d2Q9MatF0.SetTexture(Glue, _glue);
            _d2Q9MatF0.SetTexture(ColFix, _colFix);
            _d2Q9MatF1234.SetTexture(Glue, _glue);
            _d2Q9MatF1234.SetTexture(ColFix, _colFix);
            _d2Q9MatF5678.SetTexture(Glue, _glue);
            _d2Q9MatF5678.SetTexture(ColFix, _colFix);
        }
        public void SetPaper(Texture paper)
        {
            _paper = paper;
            
            _d2Q9MatF0.SetTexture(Paper, _paper);
            _d2Q9MatF1234.SetTexture(Paper, _paper);
            _d2Q9MatF5678.SetTexture(Paper, _paper);
        }
    }
}
