using System;
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

        public override void DoRelease()
        {
            base.DoRelease();
            if (f0 != null) f0.Release();
            if (f1234 != null) f1234.Release();
            if (f5678 != null) f5678.Release();
            if (fTemp != null) fTemp.Release();

            f0 = null;
            f1234 = null;
            f5678 = null;
            fTemp = null;
        }
    }

    /// <summary>
    /// 基于D2Q9的流体更新
    /// </summary>
    public class APFlow : MapBase
    {
        public override Texture Tex => _tex;
        public override MapInfoBase Info => new APFlowInfo() {
            map = this,
            f0 = _rtF0,
            f1234 = _rtF1234,
            f5678 = _rtF5678,
            fTemp = _rtTemp
        };

        public APFlow(int width, int height, Texture paper) : base((uint)width, (uint)height, MapRankTypes.WaterFlow)
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
            _d2Q9MatF0.SetFloat(E, APSamplerMgr.I.Evaporation);
            _d2Q9MatF1234.SetVector(Delta, delta);
            _d2Q9MatF1234.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF1234.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF1234.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF1234.SetFloat(E, APSamplerMgr.I.Evaporation);
            _d2Q9MatF5678.SetVector(Delta, delta);
            _d2Q9MatF5678.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF5678.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF5678.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF5678.SetFloat(E, APSamplerMgr.I.Evaporation);

            _showMat.SetTexture(LastTex0, _rtF0);
            _showMat.SetTexture(LastTex1234, _rtF1234);
            _showMat.SetTexture(LastTex5678, _rtF5678);

            SetPaper(paper);
        }

        public override void DoUpdate()
        {
            if (Released)
            {
                throw new ApplicationException("APFlow.Update: 错误！死去的Flow类开始攻击我！");
            }

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
            if (Released)
            {
                throw new ApplicationException("APFlow.SetGlueAndFix: 错误！死去的Flow类开始攻击我！");
            }

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
            if (Released)
            {
                throw new ApplicationException("APFlow.SetPaper: 错误！死去的Flow类开始攻击我！");
            }

            _paper = paper;

            _d2Q9MatF0.SetTexture(Paper, _paper);
            _d2Q9MatF1234.SetTexture(Paper, _paper);
            _d2Q9MatF5678.SetTexture(Paper, _paper);
        }

        public void UpdateDefuse()
        {
            _d2Q9MatF0.SetFloat(E, APSamplerMgr.I.Evaporation);
            _d2Q9MatF1234.SetFloat(E, APSamplerMgr.I.Evaporation);
            _d2Q9MatF5678.SetFloat(E, APSamplerMgr.I.Evaporation);
        }

        public override void DoLoad(MapInfoBase info)
        {
            if (Released)
            {
                throw new ApplicationException("APFlow.DoLoad: 错误！死去的Flow类开始攻击我！");
            }

            base.DoLoad(info);
            var i = info as APFlowInfo;
            if (i == null) return;

            Graphics.Blit(i.f0, _rtF0);
            Graphics.Blit(i.f1234, _rtF1234);
            Graphics.Blit(i.f5678, _rtF5678);
        }

        public override MapInfoBase DoSave(MapInfoBase container)
        {
            if (Released)
            {
                throw new ApplicationException("APFlow.DoSave: 错误！死去的Flow类开始攻击我！");
            }

            var i = container as APFlowInfo;
            if (i == null) return container;

            Graphics.Blit(_rtF0, i.f0);
            Graphics.Blit(_rtF1234, i.f1234);
            Graphics.Blit(_rtF5678, i.f5678);

            return i;
        }

        public override MapInfoBase NewEmptyInfo()
        {
            if (Released)
            {
                throw new ApplicationException("APFlow.NewEmptyInfo: 错误！死去的Flow类开始攻击我！");
            }

            var i = new APFlowInfo()
            {
                map = this,
                f0 = new RenderTexture(Width, Height, 0, GraphicsFormat.R16G16_SFloat) { filterMode = FilterMode.Point },
                f1234 = new RenderTexture(Width, Height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point },
                f5678 = new RenderTexture(Width, Height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point },
                fTemp = null,
            };

            i.f0.Create();
            i.f1234.Create();
            i.f5678.Create();

            return i;
        }

        public override void DoRelease()
        {
            if (Released) return;

            base.DoRelease();

            _colFix = null;
            _glue = null;
            _paper = null;

            if (_tex != null) _tex.Release();
            if (_rtF0 != null) _rtF0.Release();
            if (_rtF1234 != null) _rtF1234.Release();
            if (_rtF5678 != null) _rtF5678.Release();
            if (_rtTemp != null) _rtTemp.Release();

            _tex = null;
            _rtF0 = null;
            _rtF1234 = null;
            _rtF5678 = null;
            _rtTemp = null;
        }

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
        private static readonly int E = Shader.PropertyToID("E");
    }
}
