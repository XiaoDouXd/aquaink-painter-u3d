using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APPenSizeUI : MonoBehaviour
    {
        public Slider maxSlider;
        public Slider minSlider;

        public void OnSliderUpdate()
        {
            if (minSlider.value > maxSlider.value)
            {
                maxSlider.value = minSlider.value;
            }
            APSamplerMgr.I.SetPenSize(minSlider.value, maxSlider.value);
        }

        private void Start()
        {
            maxSlider.value = APSamplerMgr.I.PenSizeMax;
            minSlider.value = APSamplerMgr.I.PenSizeMin;
        }
    }
}

