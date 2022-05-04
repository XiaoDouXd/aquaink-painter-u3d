using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace APMaps
{
    /// <summary>
    /// 颜色更新及沉淀
    /// </summary>
    public class MapColor : MapBase
    {
        /// <summary>
        /// 颜色贴图
        /// </summary>
        public override Texture Tex => _colAdv;
        public Texture Glue => _colAdv;

        /// <summary>
        /// 颜色
        /// </summary>
        private RenderTexture _colAdv;
        private RenderTexture _colFix;
        private RenderTexture _rtTemp;

        private Material _matColAdv;
        private Material _matColFix;
        private Material _matWrite;

        private MapFlowD2Q9 _d2q9Map;
        
        private static readonly int LastTex0 = Shader.PropertyToID("_LastTex0");
        private static readonly int LastTex1234 = Shader.PropertyToID("_LastTex1234");
        private static readonly int LastTex5678 = Shader.PropertyToID("_LastTex5678");
        private static readonly int DestTex = Shader.PropertyToID("_DestTex");
        private static readonly int Delta = Shader.PropertyToID("_Delta");
        private static readonly int Last = Shader.PropertyToID("_Last");
        private static readonly int Rect1 = Shader.PropertyToID("_rect");
        private static readonly int ColTable = Shader.PropertyToID("_ColTable");
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public MapColor(int width, int height, MapFlowD2Q9 d2Q9) : base((uint)width, (uint)height, MapRankTypes.COLOR_FIX)
        {
            _d2q9Map = d2Q9;
            
            var colAdv = Shader.Find("COL_UPD/ColUpdate_Adv");
            var write = Shader.Find("AP/DoWrite_Col");
            Shader colFix = null;

            _colAdv = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _rtTemp = new RenderTexture(width, height, 0, GraphicsFormat.B8G8R8A8_UNorm);
            _colAdv.Create();
            _rtTemp.Create();

            _matColAdv = new Material(colAdv) { hideFlags = HideFlags.DontSave };
            _matWrite = new Material(write) { hideFlags = HideFlags.DontSave };
            
            var delta = new Vector2(1.0f / Width, 1.0f / Height);
            _matColAdv.SetVector(Delta, delta);
            _matColAdv.SetTexture(LastTex0, _d2q9Map.F0);
            _matColAdv.SetTexture(LastTex1234, _d2q9Map.F1234);
            _matColAdv.SetTexture(LastTex5678, _d2q9Map.F5678);
            _matColAdv.SetTexture(Last, _colAdv);
            _matColAdv.SetTexture(ColTable, APCanvasMgr.GetColTable());
            _matWrite.SetTexture(DestTex, _colAdv);
        }

        public override void DoUpdate()
        {
            base.DoUpdate();
            Graphics.Blit(null, _rtTemp, _matColAdv);
            Graphics.Blit(_rtTemp, _colAdv);
        }

        public override void DoWrite(Vector4 rect, Color col, params Texture[] texs)
        {
            base.DoWrite(rect, texs);
            
            _matWrite.SetColor(Color1, col);
            _matWrite.SetVector(Rect1, rect);
            if (texs.Length == 0)
            {
                Graphics.Blit(null, _rtTemp, _matWrite);
                Graphics.Blit(_rtTemp, _colAdv);
            }
            else
            {
                Graphics.Blit(texs[0], _rtTemp, _matWrite);
                Graphics.Blit(_rtTemp, _colAdv);
            }
        }
    }
}

