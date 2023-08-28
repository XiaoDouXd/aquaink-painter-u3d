using System;
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

        public override void DoRelease()
        {
            base.DoRelease();
            f0 = null;
            f1234 = null;
            f5678 = null;
            fTemp = null;

            color = null;
            cTemp = null;
        }
    }
    public class APLayerPersistentInfo : MapInfoBase
    {
        public APFlowInfo flowInfo;
        public APColorInfo colorInfo;

        public override void DoRelease()
        {
            base.DoRelease();
            flowInfo.DoRelease();
            colorInfo.DoRelease();
        }
    }

    public class APLayer : MapBase
    {
        public override Texture Tex => _col.Tex;
        public override MapInfoBase Info => _info;
        public int Id => _id;
        public string Name { get => _name; set => _name = value; }
        public LayerBlurType BlurType { get => _blurType; set => _blurType = value; }
        public APCanvasBlurMat Blur => _blurMat;

        public APLayer(int width, int height, Texture paper, int id) : base((uint)width, (uint)height, MapRankTypes.Layer)
        {
            _flow = new APFlow(width, height, paper);
            _col = new APColor(width, height, _flow);
            _flow.SetGlueAndFix(_col);

            _id = id;
            _name = "新建图层";
            _blurType = LayerBlurType.Normal;

            var flowData = _flow.Info as APFlowInfo;
            var colData = _col.Info as APColorInfo;
            _info = new APLayerInfo() {
                map = this,

                f0 = flowData?.f0,
                f1234 = flowData?.f1234,
                f5678 = flowData?.f5678,
                fTemp = flowData?.fTemp,

                color = colData?.adv,
                cTemp = colData?.cTemp,
            };
            _blurMat = new APCanvasBlurMat(this);
        }

        public override void DoLoad(MapInfoBase info)
        {
            if (Released)
            {
                throw new ApplicationException("APLayer.DoLoad: 错误！死去的Layer类开始攻击我！");
            }

            base.DoLoad(info);
            var i = info as APLayerPersistentInfo;
            if (i == null) return;

            _flow.DoLoad(i.flowInfo);
            _col.DoLoad(i.colorInfo);
        }

        public override MapInfoBase DoSave(MapInfoBase container)
        {
            if (Released)
            {
                throw new ApplicationException("APLayer.DoSave: 错误！死去的Layer类开始攻击我！");
            }

            var i = container as APLayerPersistentInfo;
            if (i == null) return container;

            return new APLayerPersistentInfo()
            {
                map = this,
                colorInfo = _col.DoSave(i.colorInfo) as APColorInfo,
                flowInfo = _flow.DoSave(i.flowInfo) as APFlowInfo,
            };
        }

        public override MapInfoBase NewEmptyInfo()
        {
            if (Released)
            {
                throw new ApplicationException("APLayer.NewEmptyInfp: 错误！死去的Layer类开始攻击我！");
            }

            return new APLayerPersistentInfo()
            {
                map = this,
                colorInfo = _col.NewEmptyInfo() as APColorInfo,
                flowInfo = _flow.NewEmptyInfo() as APFlowInfo,
            };
        }

        public override void DoRelease()
        {
            if (Released) return;

            base.DoRelease();
            _col.DoRelease();
            _flow.DoRelease();
        }

        public void UpdateRenderData()
        {
            _col.UpdateDefuse();
            _flow.UpdateDefuse();
        }

        private APLayerInfo _info;
        private int _id;
        private string _name;
        private LayerBlurType _blurType;
        private APCanvasBlurMat _blurMat;
        private APColor _col;
        private APFlow _flow;
    }
}
