using System;
using System.Data;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace APMaps
{
    /// <summary>
    /// 基于D2Q9的流体更新
    /// </summary>
    public class MapFlowD2Q9 : MapBase
    {
        public override Texture Tex => _rtF5678;
        public Texture F0 => _rtF0;
        public Texture F1234 => _rtF1234;
        public Texture F5678 => _rtF5678;

        private Texture _glue;
        private Texture _paper;
        private RenderTexture _rtF0;
        private RenderTexture _rtF1234;
        private RenderTexture _rtF5678;
        private RenderTexture _rtTemp;
        
        private readonly Material _d2Q9MatF0;
        private readonly Material _d2Q9MatF1234;
        private readonly Material _d2Q9MatF5678;
        private readonly Material _d2Q9DoWriteF0;
        private readonly Material _d2Q9DoWriteF1234;
        private readonly Material _d2Q9DoWriteF5678;
        
        private static readonly int LastTex0 = Shader.PropertyToID("_LastTex0");
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex1234");
        private static readonly int LastTex5678 = Shader.PropertyToID("_LastTex5678");
        private static readonly int DestTex = Shader.PropertyToID("_DestTex");
        private static readonly int Rect1 = Shader.PropertyToID("_rect");
        private static readonly int Delta = Shader.PropertyToID("_Delta");
        private static readonly int Paper = Shader.PropertyToID("_Paper");
        private static readonly int Glue = Shader.PropertyToID("_Glue");

        public MapFlowD2Q9(int width, int height) : base((uint)width, (uint)height, MapRankTypes.WATER_FLOW)
        {
            var f0 = Shader.Find("LBM_SRT/D2Q9Update_F0");
            var f1234 = Shader.Find("LBM_SRT/D2Q9Update_F1234");
            var f5678 = Shader.Find("LBM_SRT/D2Q9Update_F5678");
            var doWriteF0 = Shader.Find("AP/DoWrite_WaterF0");
            var doWriteF1234 = Shader.Find("AP/DoWrite_WaterF1234");
            var doWriteF5678 = Shader.Find("AP/DoWrite_WaterF5678");

            // 初始化材质
            _d2Q9MatF0 = new Material(f0) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF1234 = new Material(f1234) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF5678 = new Material(f5678) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF0 = new Material(doWriteF0) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF1234 = new Material(doWriteF1234) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF5678 = new Material(doWriteF5678) { hideFlags = HideFlags.DontSave };

            // 初始化贴图
            _rtF0 = new RenderTexture(width, height, 0, GraphicsFormat.R8G8_UNorm) { filterMode = FilterMode.Point };
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm) { filterMode = FilterMode.Point };
            _rtF5678 = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm) { filterMode = FilterMode.Point };
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
        }
        
        public MapFlowD2Q9(int width, int height, Texture paper, Texture glue) : base((uint)width, (uint)height, MapRankTypes.WATER_FLOW)
        {
            _paper = paper;
            _glue = glue;
            
            var f0 = Shader.Find("LBM_SRT/D2Q9Update_F0");
            var f1234 = Shader.Find("LBM_SRT/D2Q9Update_F1234");
            var f5678 = Shader.Find("LBM_SRT/D2Q9Update_F5678");
            var doWriteF0 = Shader.Find("AP/DoWrite_WaterF0");
            var doWriteF1234 = Shader.Find("AP/DoWrite_WaterF1234");
            var doWriteF5678 = Shader.Find("AP/DoWrite_WaterF5678");

            // 初始化材质
            _d2Q9MatF0 = new Material(f0) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF1234 = new Material(f1234) { hideFlags = HideFlags.DontSave };
            _d2Q9MatF5678 = new Material(f5678) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF0 = new Material(doWriteF0) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF1234 = new Material(doWriteF1234) { hideFlags = HideFlags.DontSave };
            _d2Q9DoWriteF5678 = new Material(doWriteF5678) { hideFlags = HideFlags.DontSave };

            // 初始化贴图
            _rtF0 = new RenderTexture(width, height, 0, GraphicsFormat.R32_SFloat) { filterMode = FilterMode.Point };
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R32G32B32A32_SFloat) { filterMode = FilterMode.Point };
            _rtF5678 = new RenderTexture(width, height, 0, GraphicsFormat.R32G32B32A32_SFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R32G32B32A32_SFloat) { filterMode = FilterMode.Point };
            _rtTemp.Create();
            _rtF0.Create();
            _rtF1234.Create();
            _rtF5678.Create();
        
            // 设置贴图到材质
            var delta = new Vector2(1.0f / Width, 1.0f / Height);
            _d2Q9MatF0.SetVector(Delta, delta);
            _d2Q9MatF0.SetTexture(Paper, _paper);
            _d2Q9MatF0.SetTexture(Glue, _glue);
            _d2Q9MatF0.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF0.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF0.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF1234.SetVector(Delta, delta);
            _d2Q9MatF1234.SetTexture(Paper, _paper);
            _d2Q9MatF1234.SetTexture(Glue, _glue);
            _d2Q9MatF1234.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF1234.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF1234.SetTexture(LastTex5678, _rtF5678);
            _d2Q9MatF5678.SetVector(Delta, delta);
            _d2Q9MatF5678.SetTexture(Paper, _paper);
            _d2Q9MatF5678.SetTexture(Glue, _glue);
            _d2Q9MatF5678.SetTexture(LastTex0, _rtF0);
            _d2Q9MatF5678.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatF5678.SetTexture(LastTex5678, _rtF5678);
            
            _d2Q9DoWriteF0.SetTexture(DestTex, _rtF0);
            _d2Q9DoWriteF5678.SetTexture(DestTex, _rtF5678);
            _d2Q9DoWriteF1234.SetTexture(DestTex, _rtF1234);
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
            
            _d2Q9DoWriteF0.SetVector(Rect1, rect);
            _d2Q9DoWriteF1234.SetVector(Rect1, rect);
            _d2Q9DoWriteF5678.SetVector(Rect1, rect);
            
            Graphics.Blit(texs[0], _rtTemp, _d2Q9DoWriteF0);
            Graphics.Blit(_rtTemp, _rtF0);
            
            Graphics.Blit(texs[1], _rtTemp, _d2Q9DoWriteF1234);
            Graphics.Blit(_rtTemp, _rtF1234);
            
            Graphics.Blit(texs[2], _rtTemp, _d2Q9DoWriteF5678);
            Graphics.Blit(_rtTemp, _rtF5678);
        }
    }
}
