using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace APLayerFramework
{
    /// <summary>
    /// 基于D2Q9的流体更新
    /// </summary>
    public class MapFlowD2Q9 : MapFlowBase
    {
        private RenderTexture _rtF0;
        private RenderTexture _rtF1234;
        private RenderTexture _rtF5678;
        private RenderTexture _rtTemp;

        private static bool _matInited;
        private static Material _d2Q9MatF0;
        private static Material _d2Q9MatF1234;
        private static Material _d2Q9MatF5678;
        private static Material _d2Q9MatVelocity;
        private static Material _d2Q9DoWrite;
        
        private static readonly int LastTex0 = Shader.PropertyToID("_LastTex0");
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex1234");
        private static readonly int LastTex5678 = Shader.PropertyToID("_LastTex5678");
        private static readonly int DestTex = Shader.PropertyToID("_DestTex");
        private static readonly int Rect1 = Shader.PropertyToID("_rect");

        public MapFlowD2Q9(int width, int height) : base(width, height)
        {
            InitMats();
            
            // 初始化贴图
            _rtF0 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat) { filterMode = FilterMode.Point };
            _rtF1234 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat) { filterMode = FilterMode.Point };
            _rtF5678 = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat) { filterMode = FilterMode.Point };
            _rtTemp.Create();
            _rtF0.Create();
            _rtF1234.Create();
            _rtF5678.Create();
        
            // 设置贴图到材质
            _d2Q9MatF0.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF0.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF0.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF1234.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF1234.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF1234.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF5678.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF5678.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF5678.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatVelocity.SetTexture(LastTex0, _rtF0);
            _d2Q9MatVelocity.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatVelocity.SetTexture(LastTex5678, _rtF5678);
        }

        public override void DoRecreate(int width, int height, bool clear = true)
        {
            base.DoRecreate(width, height, clear);
            
            // 初始化贴图
            _rtF0 = new RenderTexture(width, height, 0, GraphicsFormat.R16_SFloat) { filterMode = FilterMode.Point };
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtF5678 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
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
        
            ApplyVelocity(null, _d2Q9MatVelocity);
        }

        /// <summary>
        /// 写入新信息到速度场
        /// </summary>
        public override void DoWrite(Material material, Vector4 rect, params Texture[] texs)
        {
            base.DoWrite(material, rect, texs);

            if (texs.Length < 3)
                throw new ApplicationException("MapFlowD2Q9.DoWrite: 传入贴图数量错误");

            if (material == null)
            {
                Graphics.Blit(texs[0], _rtF0);
                Graphics.Blit(texs[1], _rtF1234);
                Graphics.Blit(texs[2], _rtF5678);
            }
            else
            {
                Graphics.Blit(texs[0], _rtF0, material);
                Graphics.Blit(texs[1], _rtF1234, material);
                Graphics.Blit(texs[2], _rtF5678, material);
            }
        }

        public override void DoWrite(Vector4 rect, params Texture[] texs)
        {
            base.DoWrite(rect, texs);

            if (texs.Length < 3)
                throw new ApplicationException("MapFlowD2Q9.DoWrite: 传入贴图数量错误");
            
            _d2Q9DoWrite.SetVector(Rect1, rect);
            _d2Q9DoWrite.SetTexture(DestTex, _rtF0);
            Graphics.Blit(texs[0], _rtTemp, _d2Q9DoWrite);
            Graphics.Blit(_rtTemp, _rtF0);
            
            _d2Q9DoWrite.SetTexture(DestTex, _rtF1234);
            Graphics.Blit(texs[1], _rtTemp, _d2Q9DoWrite);
            Graphics.Blit(_rtTemp, _rtF1234);
            
            _d2Q9DoWrite.SetTexture(DestTex, _rtF5678);
            Graphics.Blit(texs[2], _rtTemp, _d2Q9DoWrite);
            Graphics.Blit(_rtTemp, _rtF5678);
        }

        private static void InitMats()
        {
            if (_matInited) return;
            
            var f0 = Shader.Find("LBM_SRT/D2Q9Update_F0");
            var f1234 = Shader.Find("LBM_SRT/D2Q9Update_F1234");
            var f5678 = Shader.Find("LBM_SRT/D2Q9Update_F5678");
            var velocity = Shader.Find("LBM_SRT/D2Q9Calc_Velocity");
            var doWrite = Shader.Find("AP/DoWrite_Add");

            // 初始化材质
            _d2Q9MatF0 = new Material(f0) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF1234 = new Material(f1234) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF5678 = new Material(f5678) { hideFlags = HideFlags.DontSave };
            _d2Q9MatVelocity = new Material(velocity) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWrite = new Material(doWrite) { hideFlags = HideFlags.DontSave };

            _matInited = true;
        }
    }
}
