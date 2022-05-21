using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

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
        
        private static readonly int TexFix = Shader.PropertyToID("_TexCur");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        
        private APLayer _layer;
        private Material _mat;        
        
        public APCanvasBlurMat(APLayer layer, LayerBlurType type = LayerBlurType.NORMAL)
        {
            _layer = layer;
            _mat = new Material(Normal);
            
            SetType(type);
        }
        
        public void SetType(LayerBlurType type)
        {
            switch (type)
            {
                case LayerBlurType.NORMAL:
                    _mat.shader = Normal;
                    break;
                case LayerBlurType.REALITY:
                    _mat.shader = Reality;
                    _mat.SetTexture(ColTable, APInitMgr.I.colorTable);
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
            _mat.SetTexture(TexFix, _layer.Tex);
        }
        public void Blur(Texture layerLast, RenderTexture dest)
        {
            Graphics.Blit(layerLast, dest, _mat);
        }
    }
    
    public class APCanvas : MapBase, IEnumerable<APLayer>
    {
        private const int MaxLayerCount = 100000;        
        public override Texture Tex => _blurTex;
        public override MapInfoBase Info => null;
        public RawImage Surface { get; private set; }

        public APLayer this[int layerId]
        {
            get
            {
                if (Released)
                    return null;
                return _layers[layerId];
            }
        } 
        public int FirstLayer => _layerRank.First.Value;

        private static readonly Shader ClearShader = Shader.Find("Canvas/Clear0000");
        
        private RenderTexture _blurTex;
        private RenderTexture _rtTemp;
        private Material _clear;
        
        private readonly Dictionary<int, APLayer> _layers = new Dictionary<int, APLayer>();
        private readonly LinkedList<int> _layerRank = new LinkedList<int>();
        private int _curIdxCount;
        private int _newLayerCount;

        public APCanvas(int width, int height, RawImage surface, Texture paper = null) : 
            base((uint)width, (uint)height, MapRankTypes.CANVAS)
        {
            _newLayerCount = 1;
            _clear = new Material(ClearShader) { hideFlags = HideFlags.DontSave };
            _blurTex = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm)
            {
                filterMode = FilterMode.Point,
            };
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.R8G8B8A8_UNorm);
            _blurTex.Create();
            _rtTemp.Create();
            
            Surface = surface;
            Surface.texture = _blurTex;
            
            if (paper != null)
                Add(paper);
            else
                Add();
        }

        public void UpdateRenderData()
        {
            foreach (var la in _layers)
            {
                la.Value.UpdateRenderData();
            }
        }

        public void ReRank(LinkedListNode<int> node, int rank)
        {
            if (Released)
            {
                throw new ApplicationException("APCanvas.ReRank: 错误！死去的Canvas类开始攻击我！");
            }
            
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
            if (Released)
            {
                throw new ApplicationException("APCanvas.Remove: 错误！死去的Canvas类开始攻击我！");
            }
            
            if (_layers.Count == 1)  return;

            _layers[id]?.DoRelease();
            _layers.Remove(id);
        }
        public void Add()
        {
            if (Released)
            {
                throw new ApplicationException("APCanvas.Add: 错误！死去的Canvas类开始攻击我！");
            }
            
            var layer = new APLayer(Width, Height, APInitMgr.I.defaultPaper1, NewIdx());
            layer.Name = $"新建图层{_newLayerCount}";

            _layers.Add(layer.Id, layer);
            _layerRank.AddLast(layer.Id);
        }
        public void Add(Texture paper)
        {
            if (Released)
            {
                throw new ApplicationException("APCanvas.Add: 错误！死去的Canvas类开始攻击我！");
            }
            
            var layer = new APLayer(Width, Height, paper, NewIdx());
            layer.Name = $"新建图层{_newLayerCount}";
            
            _layers.Add(layer.Id, layer);
            _layerRank.AddLast(layer.Id);
        }
        public override void DoUpdate()
        {
            if (Released)
            {
                throw new ApplicationException("APCanvas.DoUpdate: 错误！死去的Canvas类开始攻击我！");
            }
            
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
        public override void DoRelease()
        {
            if (Released) return;
            base.DoRelease();

            foreach (var l in _layers)
            {
                l.Value.DoRelease();
            }
            _layers.Clear();
            _layerRank.Clear();
            APPersistentMgr.I.DoDelete(this);
        }
        public IEnumerator<APLayer> GetEnumerator()
        {
            if (_layerRank == null || Released)
                yield break;

            foreach (var layer in _layerRank)
            {
                yield return _layers[layer];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private int NewIdx()
        {
            if (Released) return -1;
            
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
    }
}