using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP.Canvas
{
    public class APCanvasBlurMat
    {
        private static readonly Shader Normal = Shader.Find("LayerBlur/Normal");
        private static readonly Shader Reality = Shader.Find("LayerBlur/Reality");
        private static readonly Shader Multiply = Shader.Find("LayerBlur/Multiply");
        private static readonly Shader Darken = Shader.Find("LayerBlur/Darken");
        private static readonly Shader Add = Shader.Find("LayerBlur/Add");
        private static readonly Shader Overlay = Shader.Find("LayerBlur/Overlay");
        
        private static readonly int TexCur = Shader.PropertyToID("_TexCur");
        
        public APCanvasBlurMat(Layer layer, LayerBlurType type = LayerBlurType.NORMAL)
        {
            _layer = layer;
            _mat = new Material(Normal);
            
            SetType(type);
        }

        private Layer _layer;
        private Material _mat;
        public void SetType(LayerBlurType type)
        {
            switch (type)
            {
                case LayerBlurType.NORMAL:
                    _mat.shader = Normal;
                    break;
                case LayerBlurType.REALITY:
                    _mat.shader = Reality;
                    break;
                case LayerBlurType.MULTIPLY:
                    _mat.shader = Multiply;
                    break;
                case LayerBlurType.DARKEN:
                    _mat.shader = Darken;
                    break;
                case LayerBlurType.ADD:
                    _mat.shader = Add;
                    break;
                case LayerBlurType.OVERLAY:
                    _mat.shader = Overlay;
                    break;
            }
            _mat.SetTexture(TexCur, _layer.Tex);
        }
        public void Blur(Texture layerLast, RenderTexture dest)
        {
            Graphics.Blit(layerLast, dest, _mat);
        }
    }
    
    public class APCanvas : MapBase
    {
        private const int MaxLayerCount = 100000;        
        public override Texture Tex => _blurTex;

        private static readonly Shader ClearShader = Shader.Find("Canvas/Clear0000");
        
        private RenderTexture _blurTex;
        private RenderTexture _rtTemp;
        private Material _clear;
        
        private readonly Dictionary<int, Layer> _layers = new Dictionary<int, Layer>();
        private readonly LinkedList<int> _layerRank = new LinkedList<int>();
        private int _curIdxCount;
        private int _newLayerCount;

        public APCanvas(int width, int height, Texture paper) : 
            base((uint)width, (uint)height, MapRankTypes.CANVAS)
        {
            _newLayerCount = 1;
            _clear = new Material(ClearShader) { hideFlags = HideFlags.DontSave };
            
            if (paper != null)
                Add(paper);
            else
                Add();
        }

        public void ReRank(LinkedListNode<int> node, int rank)
        {
            _layerRank.Remove(node);
            if (rank >= _layerRank.Count)
            {
                _layerRank.AddLast(node);
            }
            else if (rank <= 1)
            {
                _layerRank.AddFirst(node);
            }
            else
            {
                var pointer = _layerRank.First;
                var count = 1;
                while (pointer.Next != null)
                {
                    if (count == rank)
                    {
                        _layerRank.AddBefore(pointer, node);
                        return;
                    }

                    pointer = pointer.Next;
                    count++;
                }
            }
        }
        public void Remove(int id)
        {
            if (_layers.Count == 1)  return;
            
            _layers.Remove(id);
        }
        public void Add()
        {
            var layer = new Layer(Width, Height, APInitMgr.I.defaultPaper1, NewIdx());
            layer.Name = $"新建图层{_newLayerCount}";

            _layers.Add(layer.Id, layer);
            _layerRank.AddLast(layer.Id);
        }
        public void Add(Texture paper)
        {
            var layer = new Layer(Width, Height, paper, NewIdx());
            layer.Name = $"新建图层{_newLayerCount}";
            
            _layers.Add(layer.Id, layer);
            _layerRank.AddLast(layer.Id);
        }
        private int NewIdx()
        {
            if (_layers.Count == MaxLayerCount)
            {
                throw new ApplicationException("APCanvas.NewIdx: 错误！创建了过多的图层！");
            }
            
            while (_layers.ContainsKey(_curIdxCount))
            {
                if (_curIdxCount == int.MaxValue)
                {
                    _curIdxCount = 0;
                }
                _curIdxCount++;
            }

            return _curIdxCount;
        }
        public override void DoUpdate()
        {
            base.DoUpdate();

            Graphics.Blit(null, _blurTex, _clear);
            foreach (var layerId in _layerRank)
            {
                if (!_layers.ContainsKey(layerId))
                    throw new ApplicationException("APCanvas.DoUpdate: 错误！引用了不存在的图层！");
                
                _layers[layerId].Blur.Blur(_blurTex, _rtTemp);
                Graphics.Blit(_rtTemp, _blurTex);
            }
        }
    }
}