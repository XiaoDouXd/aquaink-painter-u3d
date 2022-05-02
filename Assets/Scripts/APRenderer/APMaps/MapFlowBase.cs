using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace APMaps
{
    /// <summary>
    /// 流体的Map
    /// </summary>
    public abstract class MapFlowBase : MapBase
    {
        // -------------------------------------------------------------
        /// <summary>
        /// 获取速度场
        /// </summary>
        /// <returns></returns>
        public override Texture Tex => _rtVelocity;
        /// <summary>
        /// 重构速度场大小
        /// </summary>
        /// <param name="width"> 宽 </param>
        /// <param name="height"> 高 </param>
        /// <param name="clear"> 是否清空 </param>
        public override void DoRecreate(int width, int height, bool clear = true)
        {
            base.DoRecreate(width, height, clear);
        
            // 不复制
            if (clear)
            {
                _rtVelocity.Release();
                _rtVelocity = new RenderTexture(Width, Height, _depth, GraphicsFormat.R16G16_SFloat);
            }

            // 复制原到新速度场
            var newVelocityRT = new RenderTexture(_rtVelocity.descriptor);
            newVelocityRT.Create();
        
            if (_recreateMaterial)
                Graphics.Blit(_rtVelocity, newVelocityRT, _recreateMaterial);
            else
                Graphics.Blit(_rtVelocity, newVelocityRT);
        
            _rtVelocity.Release();
            _rtVelocity = newVelocityRT;
        }
        /// <summary>
        /// 释放贴图
        /// </summary>
        public override void DoRelease()
        {
            base.DoRelease();
            _rtVelocity.Release();
        }
    
        // -------------------------------------------------------------
        /// <summary>
        /// 流体场
        /// </summary>
        /// <param name="width"> 宽 </param>
        /// <param name="height"> 高 </param>
        /// <param name="recreateShaderName"> 重构的Shader索引 </param>
        /// <param name="depth"> 深度 </param>
        protected MapFlowBase(int width, int height, string recreateShaderName = null, int depth = 0) 
            : base((uint)width, (uint)height, MapRankTypes.WATER_FLOW)
        {
            _depth = depth;
            _rtVelocity = new RenderTexture(Width, Height, _depth, GraphicsFormat.R16G16_SFloat);
            _rtVelocity.Create();
        
            // 载入复制用的 shader
            if (string.IsNullOrWhiteSpace(recreateShaderName)) return;
            var shader = Shader.Find(recreateShaderName);
            if (shader == null || !shader.isSupported) return;
            _recreateMaterial = new Material(shader) { hideFlags = HideFlags.DontSave };
        }
        /// <summary>
        /// 应用计算出来的流体场
        /// </summary>
        protected void ApplyVelocity(Texture velocity)
        {
            Graphics.Blit(velocity, _rtVelocity);
        }
        /// <summary>
        /// 应用计算出来的流体场
        /// </summary>
        /// <param name="velocity"> 计算结果 </param>
        /// <param name="mat"> 复制用的材质 </param>
        protected void ApplyVelocity(Texture velocity, Material mat)
        {
            Graphics.Blit(velocity, _rtVelocity, mat);
        }
    
        // -------------------------------------------------------------
        /// <summary>
        /// 在宏观纸面内流体的速度场
        /// 格式为 R16G16_SF (Half2类型)
        /// </summary>
        private RenderTexture _rtVelocity;
        /// <summary>
        /// 重构用的材质
        /// </summary>
        private readonly Material _recreateMaterial;
        /// <summary>
        /// 0
        /// </summary>
        private readonly int _depth;
    }
}
