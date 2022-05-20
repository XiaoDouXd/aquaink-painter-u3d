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
        
        public Color CurColor { get; private set; }
        public Color LasColor { get; private set; }
        
        public float PenSizeMax { get; private set; }
        public float PenSizeMin { get; private set; }
        
        public float WetMax { get; private set; }
        public float WetMin { get; private set; }
    
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
                
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        #endregion
    }
}

