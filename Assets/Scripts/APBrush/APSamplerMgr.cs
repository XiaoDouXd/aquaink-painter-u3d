using System.Collections;
using System.Collections.Generic;
using AP.UI;
using UnityEngine;

namespace AP
{
    public class APSamplerMgr : MonoBehaviour
    {
        [SerializeField]
        private Color defaultColor;
        [SerializeField]
        [Range(0, 1)]
        private float penSizeMax;
        [SerializeField]
        [Range(0, 1)]
        private float penSizeMin;
        [SerializeField]
        [Range(0, 1)]
        private float defaultWet;
        [SerializeField]
        [Range(0.0001f, 1)]
        private float defaultBrushInterval;

        [Space(10)] 
        [SerializeField] 
        [Range(0.0002f, 1.0f)]
        private float defuseFactor;
        [SerializeField]
        [Range(0.00001f, 0.1f)]
        private float evaporation;

        [Space(10)]
        [SerializeField]
        [Range(0, 1000000)]
        private float defaultSoft = 0;
        [SerializeField]
        [Range(-1, 1)]
        private float defaultAlphaAdd = 0;
        
        
        public float DefuseFactor { get; private set; }
        public float Evaporation { get; private set; }

        public Color CurColor { get; private set; }
        public Color LasColor { get; private set; }
        
        public float PenSizeMax { get; private set; }
        public float PenSizeMin { get; private set; }
        
        public float WetMax { get; private set; }
        public float WetMin { get; private set; }

        public float Soft { get; private set; }
        public float AlphaAdd { get; private set; }
        public float AlphaAddMin { get; private set; }

        public float BrushInterval { get; private set; }
        
        public void SetColor(APColorPickerUI picker)
        {
            LasColor = CurColor;
            CurColor = picker.colorPicked;
        }
        public void SetPenSize(float min, float max)
        {
            PenSizeMin = min;
            PenSizeMax = max;
        }
        public void SetWet(float min, float max)
        {
            WetMax = max;
            WetMin = min;
        }
        public void SetPaperWet(float def, float eva)
        {
            DefuseFactor = def;
            Evaporation = eva;
        }
        public void SetBrushInterval(float interval)
        {
            BrushInterval = interval;
        }
        public void SetSoftAndAlphaAdd(float soft, float add, float addMin)
        {
            Soft = soft;
            AlphaAdd = add;
            AlphaAddMin = addMin;
        }

        #region 单例类
        public static APSamplerMgr I => _i;
        private static APSamplerMgr _i;
        
        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
                
                // 初始化数据
                CurColor = defaultColor;
                PenSizeMax = penSizeMax;
                penSizeMin = penSizeMin <= penSizeMax ? penSizeMin : penSizeMax;
                WetMax = defaultWet;
                WetMin = defaultWet;
                DefuseFactor = defuseFactor;
                Evaporation = evaporation;
                BrushInterval = defaultBrushInterval;
                Soft = defaultSoft;
                AlphaAdd = defaultAlphaAdd;
                AlphaAddMin = defaultAlphaAdd;
                
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        #endregion
    }
}

