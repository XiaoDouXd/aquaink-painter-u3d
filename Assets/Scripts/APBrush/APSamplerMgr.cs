using System.Collections;
using System.Collections.Generic;
using AP.UI;
using UnityEngine;

namespace AP
{
    public class APSamplerMgr : MonoBehaviour
    {
        public Color CurColor { get; private set; }
        public Color LasColor { get; private set; }
    
        public void SetColor(APColorPickerUI picker)
        {
            LasColor = CurColor;
            CurColor = picker.colorPicked;
        }
    
        #region 单例类
        public static APSamplerMgr I => _i;
        private static APSamplerMgr _i;
        
        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        #endregion
    }
}

