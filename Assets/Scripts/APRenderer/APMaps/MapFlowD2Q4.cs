using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = System.Object;

namespace APMaps
{
    /// <summary>
    /// 基于D2Q9的流体更新
    /// </summary>
    public class MapFlowD2Q4 : MapBase
    {
        public override Texture Tex => _rtF1234;

        private RenderTexture _rtF1234;
        private RenderTexture _rtTemp;

        private static bool _matInited;
        private static Material _d2Q9MatF1234;
        private static Material _d2Q9MatVelocity;
        
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex");

        public MapFlowD2Q4(int width, int height) : base((uint)width, (uint)height, MapRankTypes.WATER_FLOW)
        {
            InitMats();
            
            // 初始化贴图
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtTemp.Create();
            _rtF1234.Create();

            // 设置贴图到材质
            _d2Q9MatF1234.SetTexture(LastTex1234, _rtF1234);
            _d2Q9MatVelocity.SetTexture(LastTex1234, _rtF1234);
        }

        public override void DoRecreate(int width, int height, bool clear = true)
        {
            base.DoRecreate(width, height, clear);
            
            // 初始化贴图
            _rtF1234 = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R16G16B16A16_SFloat) { filterMode = FilterMode.Point };
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            
            Graphics.Blit(null, _rtTemp, _d2Q9MatF1234);
            Graphics.Blit(_rtTemp, _rtF1234);
        }

        /// <summary>
        /// 写入新信息到速度场
        /// </summary>
        public override void DoWrite(Material material, Vector4 rect, params Texture[] texs)
        {
            base.DoWrite(material, rect, texs);

            if (texs.Length < 1)
                throw new ApplicationException("MapFlowD2Q4.DoWrite: 传入贴图数量错误");

            if (material == null)
            {
                Graphics.Blit(texs[0], _rtF1234);
            }
            else
            {
                Graphics.Blit(texs[0], _rtF1234, material);
            }
        }

        private static void InitMats()
        {
            if (_matInited) return;
            
            var f1234 = Shader.Find("LBM_SRT/D2Q4Update");
            var velocity = Shader.Find("LBM_SRT/D2Q4Calc");

            // 初始化材质
            _d2Q9MatF1234 = new Material(f1234) { hideFlags = HideFlags.DontSave };
            _d2Q9MatVelocity = new Material(velocity) { hideFlags = HideFlags.DontSave };

            _matInited = true;
        }
    }
}
