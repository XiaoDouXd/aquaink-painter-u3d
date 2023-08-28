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
        private const float MaxRenderTimeSlice = 1.0f / 60.0f;

        public static MapRenderer I
        {
            get
            {
                if (_i == null)
                    _i = new MapRenderer();
                return _i;
            }
        }

        public IEnumerator RenderCoroutine()
        {
            _refresh = false;
            var flowMaps = new MapEnumerable(MapRankTypes.WaterFlow);
            var colMap = new MapEnumerable(MapRankTypes.ColorFix);
            var layerMaps = new MapEnumerable(MapRankTypes.Layer);

            while (Application.isPlaying)
            {
                if (_refresh)
                {
                    flowMaps = new MapEnumerable(MapRankTypes.WaterFlow);
                    colMap = new MapEnumerable(MapRankTypes.ColorFix);
                    layerMaps = new MapEnumerable(MapRankTypes.Layer);
                    _refresh = false;
                }

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

                while (_pause)
                {
                    yield return null;
                }

                yield return null;
            }
        }

        public IEnumerator RenderCanvasCoroutine()
        {
            _refreshCanvas = false;
            var canvasMaps = new MapEnumerable(MapRankTypes.Canvas);
            while (Application.isPlaying)
            {
                if (_canvasWaitSomeframe)
                {
                    yield return null;
                    _canvasWaitSomeframe = false;
                }

                if (_refreshCanvas)
                {
                    canvasMaps = new MapEnumerable(MapRankTypes.Canvas);
                    _refreshCanvas = false;
                }

                foreach (var canvas in canvasMaps)
                {
                    canvas.DoUpdate();
                }

                yield return null;
            }
        }

        public void Refresh()
        {
            _refresh = true;
            _refreshCanvas = true;
        }

        public void SetPause(bool p = true)
        {
            _pause = p;
        }

        public void CanvasWaitSomeFrame()
        {
            _canvasWaitSomeframe = true;
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

        private static MapRenderer _i;

        private float _timeCount;
        private bool _refresh;
        private bool _refreshCanvas;
        private bool _pause;
        private bool _canvasWaitSomeframe;
    }
}

