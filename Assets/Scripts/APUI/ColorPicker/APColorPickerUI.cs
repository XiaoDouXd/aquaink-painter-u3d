using System.Net.Mime;
using System.Numerics;
using AP.UI;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace AP.UI
{
    public interface IColorPickerComponent
    {
        public void SetPicker(APColorPickerUI picker);
        public void UpdateColor();
    }
    
    public class APColorPickerUI : MonoBehaviour
    {
        public Color colorPicked;
        public Vector3 HSV { get; private set; }

        public APColorPickerRectUI rect;
        public APColorPickerRingUI ring;
        public APColorPickerSliderUI sliderR;
        public APColorPickerSliderUI sliderG;
        public APColorPickerSliderUI sliderB;
        public APColorPickerSliderUI sliderA;
        public UnityEngine.UI.RawImage colorShow;
        
        public void UpdateAllBySlider(Color color)
        {
            colorPicked = color;
            Color.RGBToHSV(colorPicked, out var h, out var s, out var v);
            HSV = new Vector3(h, s, v);

            if (HSV.y != 0)
            {
                ring.UpdateColor();
                rect.UpdateColor();
            }
            
            sliderR.UpdateColor();
            sliderG.UpdateColor();
            sliderB.UpdateColor();
            sliderA.UpdateColor();
            colorShow.color = colorPicked;
            
            APSamplerMgr.I.SetColor(this);
        }
        public void UpdateAllByRing(Vector3 hsv)
        {
            var col = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
            HSV = hsv;
            colorPicked = new Color(col.r, col.g, col.b, colorPicked.a);
            
            if (HSV.y != 0)
            {
                ring.UpdateColor();
            }
            
            rect.UpdateColor();
            sliderR.UpdateColor();
            sliderG.UpdateColor();
            sliderB.UpdateColor();
            sliderA.UpdateColor();
            
            colorShow.color = colorPicked;
            APSamplerMgr.I.SetColor(this);
        }
        public void UpdateAllByRect(Vector3 hsv)
        {
            var col = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
            HSV = hsv;
            colorPicked = new Color(col.r, col.g, col.b, colorPicked.a);
            
            if (HSV.y != 0)
            {
                rect.UpdateColor();
                ring.UpdateColor();
            }
            
            sliderR.UpdateColor();
            sliderG.UpdateColor();
            sliderB.UpdateColor();
            sliderA.UpdateColor();
            
            colorShow.color = colorPicked;
            APSamplerMgr.I.SetColor(this);
        }
        private void Start()
        {
            colorPicked = APSamplerMgr.I.CurColor;
            
            rect.SetPicker(this);
            ring.SetPicker(this);
            sliderA.SetPicker(this);
            sliderB.SetPicker(this);
            sliderG.SetPicker(this);
            sliderR.SetPicker(this);
            
            rect.UpdateColor();
            ring.UpdateColor();
            sliderR.UpdateColor();
            sliderG.UpdateColor();
            sliderB.UpdateColor();
            sliderA.UpdateColor();
            
            colorShow.color = colorPicked;
            APSamplerMgr.I.SetColor(this);
        }
    }
}

