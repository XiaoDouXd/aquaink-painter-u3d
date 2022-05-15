using System.Collections;
using UnityEngine;
using MapEnumerable = AP.Canvas.MapBase.MapEnumerable;

namespace AP.Canvas
{
    /// <summary>
    /// 贴图渲染器
    /// </summary>
    public class MapRenderer
    {
        private const float MaxRenderTimeSlice = 1.0f / 24.0f;
        
        public static MapRenderer I
        {
            get
            {
                if (_i == null)
                    _i = new MapRenderer();
                return _i;
            }
        }
        private static MapRenderer _i;
        
        private float _timeCount;

        public IEnumerator RenderCoroutine()
        {
            var flowMaps = new MapEnumerable(MapRankTypes.WATER_FLOW);
            var colMap = new MapEnumerable(MapRankTypes.COLOR_FIX);
            var layerMaps = new MapEnumerable(MapRankTypes.LAYER);
            var canvasMaps = new MapEnumerable(MapRankTypes.CANVAS);
            
            while (Application.isPlaying)
            {
                foreach (var flow in flowMaps)
                {
                    flow.DoUpdate();
                }

                if (TimeOut()) yield return null;

                foreach (var col in colMap)
                {
                    col.DoUpdate();
                }
                
                if (TimeOut()) yield return null;

                foreach (var layer in layerMaps)
                {
                    layer.DoUpdate();
                }
                
                if (TimeOut()) yield return null;

                foreach (var canvas in canvasMaps)
                {
                    canvas.DoUpdate();
                }
                
                yield return null;
            }
        }

        private bool TimeOut()
        {
            if (_timeCount >= MaxRenderTimeSlice)
            {
                _timeCount = 0;
                return true;
            }

            _timeCount += Time.deltaTime;
            return false;
        }
    }
}

