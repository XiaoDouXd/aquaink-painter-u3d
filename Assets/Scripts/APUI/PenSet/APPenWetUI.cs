using System.Collections;
using System.Collections.Generic;
using AP;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APPenWetUI : MonoBehaviour
    {
        public Slider maxSlider;
        public Slider minSlider;
    
        public void OnSliderUpdate()
        {
            if (minSlider.value > maxSlider.value)
            {
                maxSlider.value = minSlider.value;
            }
            APSamplerMgr.I.SetWet(minSlider.value, maxSlider.value);
        }
    
        private void Start()
        {
            maxSlider.value = APSamplerMgr.I.WetMax;
            minSlider.value = APSamplerMgr.I.WetMin;
        }
    }
}

