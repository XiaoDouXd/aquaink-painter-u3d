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
        [Space(10)]

        public Slider softSlider;
        public Slider alphaAddSlider;
        public Slider alphaAddMinSlider;
    
        public void OnSliderUpdate()
        {
            if (minSlider.value > maxSlider.value)
            {
                 minSlider.value = maxSlider.value;
            }

            if (alphaAddMinSlider.value > alphaAddSlider.value)
            {
                alphaAddMinSlider.value = alphaAddSlider.value;
            }
            APSamplerMgr.I.SetWet(minSlider.value, maxSlider.value);
            APSamplerMgr.I.SetSoftAndAlphaAdd(softSlider.value, alphaAddSlider.value, alphaAddMinSlider.value);
        }
    
        private void Start()
        {
            var a = APSamplerMgr.I.WetMax;
            var b = APSamplerMgr.I.WetMin;
            var c = APSamplerMgr.I.Soft;
            var d = APSamplerMgr.I.AlphaAdd;
            
            maxSlider.value = a;
            minSlider.value = b;
            softSlider.value = c;
            alphaAddSlider.value = d;
            alphaAddMinSlider.value = d;
        }
    }
}

