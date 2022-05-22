using System;
using System.Numerics;
using AP.Canvas;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

namespace AP.Brush
{
    public class APBrushMgr
    {
        public static APBrushMgr I
        {
            get
            {
                if (_i == null)
                    _i = new APBrushMgr();
                return _i;
            }
        }
        private static APBrushMgr _i;
        public static (Vector2 pos, bool isInside) Window2Canvas(RectTransform canvasTrans, Vector2 windowPos)
        {
            windowPos -= APInitMgr.I.WindowCenter + canvasTrans.anchoredPosition;
            
            if (!canvasTrans)
                return (windowPos, false);

            var rect = canvasTrans.rect;
            var localScale = canvasTrans.localScale;
            var x = (windowPos.x - rect.xMin * localScale.x) / (localScale.x * rect.width);
            var y = (windowPos.y - rect.yMin * localScale.y) / (localScale.y * rect.height);

            return (new Vector2(x, y), x <= 1 && x >= 0 && y <= 1 && y >= 0);
        }
    }

    public abstract class APBrushSetInfoBase
    {
        public abstract void GetValue(APBrushSetInfoBase info);
    }
    
    public static class APBrushPreWriteExt
    {
        private static readonly int PreTexAlpha = Shader.PropertyToID("_PreTexAlpha");
        private static readonly int PreTexShaderName = Shader.PropertyToID("_PreTex");
        public static void SetBrushPreAlphaToMat(this Material mat, APBrushPreWrite preWrite)
        {
            if (preWrite.Released) return;
            mat.SetTexture(PreTexAlpha, preWrite.AlphaTex);
        }
        public static void SetBrushPreColorToMat(this Material mat, APBrushPreWrite preWrite)
        {
            if (preWrite.Released) return;
            mat.SetTexture(PreTexShaderName, preWrite.Tex);
        }
    }

    public class APBrushPreWrite : MapBase
    {
        public override Texture Tex => _preWriteTexColor;
        public Texture AlphaTex => _preWriteTexAlpha;
        public override MapInfoBase Info => null;

        /// <summary>
        /// 预写入缓存
        /// </summary>
        private RenderTexture _preWriteTexColor;
        private RenderTexture _preWriteTexAlpha;
        private RenderTexture _pTemp;
        
        private static readonly Shader PreClearShader = Shader.Find("Canvas/Clear0000");
        private static readonly Material PreClearMat = new Material(PreClearShader) { hideFlags = HideFlags.DontSave };

        public APBrushPreWrite(APCanvas canvas) : base((uint)canvas.Width, (uint)canvas.Height, MapRankTypes.NONE)
        {
            _preWriteTexColor = new RenderTexture(canvas.Width, canvas.Height, 0, GraphicsFormat.R8G8B8A8_UNorm);
            _pTemp = new RenderTexture(canvas.Width, canvas.Height, 0, GraphicsFormat.R8G8B8A8_UNorm);
            _preWriteTexAlpha = new RenderTexture(canvas.Width, canvas.Height, 0, GraphicsFormat.R8_UNorm);
            _preWriteTexColor.Create();
            _pTemp.Create();
            _preWriteTexAlpha.Create();

            DoClear();
        }

        public void DoClear()
        {
            Graphics.Blit(null, _preWriteTexColor, PreClearMat);
            Graphics.Blit(null, _preWriteTexAlpha, PreClearMat);
        }
        public void DoWriteAlpha(Material mat, Texture tex = null)
        {
            Graphics.Blit(_preWriteTexAlpha, _pTemp, mat);
            Graphics.Blit(_pTemp, _preWriteTexAlpha);
        }
        public void DoWriteColor(Material mat)
        {
            Graphics.Blit(_preWriteTexColor, _pTemp, mat);
            Graphics.Blit(_pTemp, _preWriteTexColor);
        }
        public override void DoRelease()
        {
            base.DoRelease();
            _preWriteTexColor.Release();
            _pTemp.Release();
            _preWriteTexAlpha.Release();
            
            _preWriteTexColor = null;
            _pTemp = null;
            _preWriteTexAlpha = null;
        }
    }
    
    public abstract class APBrushBase
    {
        public Texture PreTex => preWrite.Tex;
        protected abstract APBrushSetInfoBase Info { get; }
        protected abstract Material PreWriteMat { get; }
        protected abstract Material PreWriteAlphaMat { get; }

        protected APCanvas canvas;
        protected int curLayer;
        private Action<APBrushSetInfoBase> _infoUpdater;

        protected APBrushPreWrite preWrite;
        public APBrushBase(APCanvas canvas, int currLayer = -1)
        {
            this.canvas = canvas;
            if (currLayer == -1)
                curLayer = canvas.FirstLayer;
            else
            {
                curLayer = currLayer;
            }

            preWrite = canvas.PreWrite;
        }

        /// <summary>
        /// 开始写入
        /// 在 override 时请在开头调用基类中的该函数
        /// </summary>
        /// <param name="pos"> 写入位置 </param>
        public virtual void DoWriteDown(Vector2 pos)
        {
            MapRenderer.I.SetPause();
        }
        /// <summary>
        /// 结束写入
        /// 在 override 时请在结尾处调用基类中的该函数
        /// </summary>
        /// <param name="pos"> 写入位置 </param>
        public virtual void DoWriteUp(Vector2 pos)
        {
            MapRenderer.I.SetPause(false);
        }
        public virtual void DoCreate(APCanvas canvasIn, int currLayer = -1)
        {
            if (canvas != null)
                DoRelease();
            
            canvas = canvasIn;
            if (currLayer == -1)
                curLayer = canvas.FirstLayer;
            else
            {
                curLayer = currLayer;
            }

            preWrite = canvasIn.PreWrite;
        }
        public virtual void DoRelease()
        {
            preWrite = null;
            canvas = null;
            curLayer = -1;
        }
        public virtual void SetTex(params Texture[] texs) { }
        protected virtual void SetPreWriteMat(Vector2 pos) { }
        /// <summary>
        /// 从预写入贴图拷贝到指定层级
        /// override 时请在末尾调用基类中的函数
        /// 以清除预写入贴图的信息
        /// </summary>
        protected virtual void DoWrite()
        {
            MapRenderer.I.CanvasWaitSomeFrame();
            preWrite.DoClear();
        }
        protected void DoPreWrite(Vector2 pos)
        {
            SetPreWriteMat(pos);
            preWrite.DoWriteAlpha(PreWriteAlphaMat);
            preWrite.DoWriteColor(PreWriteMat);
        }
        public void SetInfoUpdater(Action<APBrushSetInfoBase> updater)
        {
            _infoUpdater = updater;
        }
        public void SetInfo()
        {
            _infoUpdater?.Invoke(Info);
        }
        public void SetInfo(APBrushSetInfoBase info)
        {
            Info.GetValue(info);
        }

    }
}


