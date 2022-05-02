using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace APMaps
{
    /// <summary>
    /// 颜色更新及沉淀
    /// </summary>
    public class MapColor : MapBase
    {
        /// <summary>
        /// 颜色贴图
        /// </summary>
        public override Texture Tex => _col;
        public Texture Glue => _colConcentration;

        /// <summary>
        /// 颜色浓度
        /// 格式为 RGB_SF16
        /// RGB 分别代表表面、渗透、沉淀的浓度
        /// </summary>
        private RenderTexture _colConcentration;
        /// <summary>
        /// 每个像素的颜色值
        /// 格式为 RGBA_UNORM
        /// </summary>
        private RenderTexture _col;

        private Material _matConcentrationUpdate;
        private Material _matColUpdate;

        public MapColor(int width, int height) : base((uint)width, (uint)height, MapRankTypes.COLOR_FIX)
        {
            
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            
        }
    }
}

