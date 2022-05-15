using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AP.Canvas;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace AP.Canvas
{
    public class LayerWriteInfo
    {
        public RenderTexture f0;
        public RenderTexture f1234;
        public RenderTexture f5678;
        public RenderTexture color;
    }
    
    public class Layer : MapBase
    {
        public override Texture Tex => _tex;
        public int Id => _id;
        public string Name { get => _name; set => _name = value; }
        public LayerBlurType BlurType { get => _blurType; set => _blurType = value; }
        public APCanvasBlurMat Blur => _blurMat;

        private RenderTexture _tex;
        private LayerWriteInfo _writeInfo;
        private Material _showMat;
        private int _id;
        private string _name;
        private LayerBlurType _blurType;
        private APCanvasBlurMat _blurMat;
        
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Adv = Shader.PropertyToID("_Adv");
        private static readonly int Fix = Shader.PropertyToID("_Fix");
        
        private static readonly Shader ShowShader = Shader.Find("Layer/Show");

        public Layer(int width, int height, Texture paper, int id) : base((uint)width, (uint)height, MapRankTypes.LAYER)
        {
            var d2Q9Flow = new APFlow(width, height, paper);
            var color = new APColor(width, height, d2Q9Flow);
            d2Q9Flow.SetGlueAndFix(color);

            _id = id;
            _name = "新建图层";
            _blurType = LayerBlurType.NORMAL;

            _tex = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm);
            _tex.Create();

            _writeInfo = new LayerWriteInfo()
            {
                f0 = d2Q9Flow.F0,
                f1234 = d2Q9Flow.F1234,
                f5678 = d2Q9Flow.F5678,
                color = color.colAdv,
            };
            
            _showMat = new Material(ShowShader) { hideFlags = HideFlags.DontSave };
            
            _showMat.SetTexture(ColTable, APInitMgr.I.colorTable);
            _showMat.SetTexture(Adv, color.colAdv);
            _showMat.SetTexture(Fix, color.Tex);
            
            _blurMat = new APCanvasBlurMat(this);
        }
        public void DoWrite(Action<LayerWriteInfo> writeAct)
        {
            writeAct?.Invoke(_writeInfo);
        }
        public override void DoUpdate()
        {
            Graphics.Blit(null, _tex, _showMat);
        }
    }
}
