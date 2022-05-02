using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APMaps;
using MapEnumerable = APMaps.MapBase.MapEnumerable;

namespace APRenderer
{
    /// <summary>
    /// 贴图渲染器
    /// </summary>
    public class MapRenderer
    {
        public void Render()
        {
            // 更新所有的流体层
            var flowMaps = new MapEnumerable(MapRankTypes.WATER_FLOW);
            foreach (var flow in flowMaps)
            {
                flow.DoUpdate();
            }
        }
    }
}

