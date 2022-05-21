using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APWaterControl : MonoBehaviour
    {
        public Slider maxSlider;
        public Slider minSlider;
    
        public void OnSliderUpdate()
        {
            APSamplerMgr.I.SetPaperWet(minSlider.value, maxSlider.value);
        }
    
        private void Start()
        {
            maxSlider.value = APSamplerMgr.I.Evaporation;
            minSlider.value = APSamplerMgr.I.DefuseFactor;
        }
    }
}


