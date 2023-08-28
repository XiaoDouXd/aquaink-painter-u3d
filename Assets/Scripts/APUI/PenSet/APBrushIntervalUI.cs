using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APBrushIntervalUI : MonoBehaviour
    {
        public Slider maxSlider;

        public void OnSliderUpdate()
        {
            APSamplerMgr.I.SetBrushInterval(maxSlider.value);
        }

        private void Start()
        {
            maxSlider.value = APSamplerMgr.I.BrushInterval;
        }
    }
}


