using UnityEngine;

namespace AP.Canvas
{
    public class APLayerInfo : MapInfoBase
    {
        public RenderTexture f0;
        public RenderTexture f1234;
        public RenderTexture f5678;
        public RenderTexture fTemp;
        
        public RenderTexture color;
        public RenderTexture cTemp;
    }
    
    public class APLayer : MapBase
    {
        public override Texture Tex => _col.Tex;
        public override MapInfoBase Info => _info;
        public int Id => _id;
        public string Name { get => _name; set => _name = value; }
        public LayerBlurType BlurType { get => _blurType; set => _blurType = value; }
        public APCanvasBlurMat Blur => _blurMat;

        private Texture _tex;
        private APLayerInfo _info;
        private int _id;
        private string _name;
        private LayerBlurType _blurType;
        private APCanvasBlurMat _blurMat;
        private APColor _col;

        public APLayer(int width, int height, Texture paper, int id) : base((uint)width, (uint)height, MapRankTypes.LAYER)
        {
            var d2Q9Flow = new APFlow(width, height, paper);
            _col = new APColor(width, height, d2Q9Flow);
            d2Q9Flow.SetGlueAndFix(_col);

            _id = id;
            _name = "新建图层";
            _blurType = LayerBlurType.NORMAL;

            var flowData = d2Q9Flow.Info as APFlowInfo;
            var colData = _col.Info as APColorInfo;
            _info = new APLayerInfo() {
                f0 = flowData?.f0,
                f1234 = flowData?.f1234,
                f5678 = flowData?.f5678,
                fTemp = flowData?.fTemp,
                
                color = colData?.adv,
                cTemp = colData?.cTemp,
            };
            _blurMat = new APCanvasBlurMat(this);
        }
    }
}
