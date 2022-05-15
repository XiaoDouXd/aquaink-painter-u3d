using MapEnumerable = AP.Canvas.MapBase.MapEnumerable;

namespace AP.Canvas
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

