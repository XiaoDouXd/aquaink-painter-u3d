
using UnityEngine;

namespace APRenderer
{
    /// <summary>
    /// 一个图层
    /// </summary>
    public class Layer
    {
        #region Method
        public Layer()
        {
            if (RendererMgr.I == null)
            {
                throw new System.ApplicationException(
                    "Layer" +
                    m_layerId.ToString() +
                    ": 找不到渲染管理器!"
                    );
            }
            m_layerId = preId;
            preId++;

            // 初始化根节点
            m_root = new("Layer_" + m_layerId.ToString());
            m_root.transform.parent = RendererMgr.I.LayerObjs.transform;

            CamInit_WaterUpdate();
            CamInit_ColorUpdate();
            CamInit_Fixation();
        }
        #endregion

        #region Field
        private RenderTexture m_water;
        private RenderTexture m_fixation;
        private RenderTexture m_colorMovable;

        private uint m_layerId = 0;
        private readonly GameObject m_root = null;

        private static uint preId = 0;
        #endregion

        #region 内部渲染循环
        private Camera m_waterUpdateCam;
        private Camera m_colorUpdateCam;
        private Camera m_fixationCam;

        private void CamInit_WaterUpdate()
        {
            GameObject waterCam = new("waterCam", typeof(Camera));
            waterCam.transform.parent = m_root ? m_root.transform : null;
            m_waterUpdateCam = waterCam.GetComponent<Camera>();

            m_waterUpdateCam.clearFlags = CameraClearFlags.Color;
            m_waterUpdateCam.backgroundColor = new(0, 0, 0, 0);
            m_waterUpdateCam.allowHDR = false;
            m_waterUpdateCam.allowMSAA = false;
            m_waterUpdateCam.allowDynamicResolution = false;

            RenderTextureDescriptor rt_desc = new()
            {
                width = RendererMgr.I.CanvasSize.x,
                height = RendererMgr.I.CanvasSize.y,
                colorFormat = RenderTextureFormat.R8,
            };
            m_water = new(rt_desc); m_water.Create();

            m_waterUpdateCam.targetTexture = m_water;
        }
        private void CamInit_ColorUpdate()
        {
            GameObject colorCam = new("colorCam", typeof(Camera));
            colorCam.transform.parent = m_root ? m_root.transform : null;
            m_colorUpdateCam = colorCam.GetComponent<Camera>();

            m_colorUpdateCam.clearFlags = CameraClearFlags.Color;
            m_colorUpdateCam.backgroundColor = new(0, 0, 0, 0);
            m_colorUpdateCam.allowHDR = false;
            m_colorUpdateCam.allowMSAA = false;
            m_colorUpdateCam.allowDynamicResolution = false;

            RenderTextureDescriptor rt_desc = new()
            {
                width = RendererMgr.I.CanvasSize.x,
                height = RendererMgr.I.CanvasSize.y,
                colorFormat = RenderTextureFormat.ARGB4444,
            };
            m_colorMovable = new(rt_desc); m_colorMovable.Create();

            m_colorUpdateCam.targetTexture = m_colorMovable;
        }
        private void CamInit_Fixation()
        {
            GameObject colorCam = new("fixationCam", typeof(Camera));
            colorCam.transform.parent = m_root ? m_root.transform : null;
            m_fixationCam = colorCam.GetComponent<Camera>();

            m_fixationCam.clearFlags = CameraClearFlags.Color;
            m_fixationCam.backgroundColor = new(0, 0, 0, 0);
            m_fixationCam.allowHDR = false;
            m_fixationCam.allowMSAA = false;
            m_fixationCam.allowDynamicResolution = false;

            RenderTextureDescriptor rt_desc = new()
            {
                width = RendererMgr.I.CanvasSize.x,
                height = RendererMgr.I.CanvasSize.y,
                colorFormat = RenderTextureFormat.ARGB4444,
            };
            m_fixation = new(rt_desc); m_fixation.Create();

            m_fixationCam.targetTexture = m_fixation;
        }
        #endregion
    }
}
