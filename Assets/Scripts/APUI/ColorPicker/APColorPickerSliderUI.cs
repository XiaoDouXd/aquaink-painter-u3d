using System;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    [Serializable]
    public enum ColorChannel : byte
    {
        R = 0,
        G = 1,
        B = 2,
        A = 3
    }
    
    public class APColorPickerSliderUI : MonoBehaviour, IColorPickerComponent
    {
        public ColorChannel channel;
        
        public Color _color;

        private APColorPickerUI _picker;
        public Slider slider;
        public InputField texInput;
        public RawImage bg;
        public Text tagTex;
        
        private static readonly int TargetColor = Shader.PropertyToID("_TargetColor");

        public void SetColor(Color color)
        {
            _color = color;
        }
        public void SliderValueChange()
        {
            texInput.text = $"{(int)slider.value}";
            SetVal((int)slider.value);
        }
        public void TexValueChange()
        {
            if (string.IsNullOrWhiteSpace(texInput.text))
                slider.value = 0;
            
            if (int.TryParse(texInput.text, out var c))
            {
                slider.value = Mathf.Clamp(c, 0, 255);
                            SetVal((int)slider.value);
            }
            texInput.text = $"{slider.value}";
        }

        public void SetPicker(APColorPickerUI picker)
        {
            _picker = picker;
        }

        public void UpdateColor()
        {
            _color = _picker.colorPicked;
            switch (channel)
            {
                case ColorChannel.R:
                    slider.value = _color.r * 255.0f;
                    break;
                case ColorChannel.G:
                    slider.value = _color.g * 255.0f;
                    break;
                case ColorChannel.B:
                    slider.value = _color.b * 255.0f;
                    break;
                case ColorChannel.A:
                    slider.value = _color.a * 255.0f;
                    break;
            }
            
            bg.material.SetColor(TargetColor, _color);
        }

        private void SetVal(int val)
        {
            switch (channel)
            {
                case ColorChannel.R:
                    _color = new Color(val / 255.0f, _color.g, _color.b, _color.a);
                    break;
                case ColorChannel.G:
                    _color = new Color(_color.r,val / 255.0f , _color.b, _color.a);
                    break;
                case ColorChannel.B:
                    _color = new Color(_color.r, _color.g, val / 255.0f, _color.a);
                    break;
                case ColorChannel.A:
                    _color = new Color(_color.r, _color.g, _color.b, val / 255.0f);
                    break;
            }

            bg.material.SetColor(TargetColor, _color);
            _picker.UpdateAllBySlider(_color);
        }
        private void Awake()
        {
            // slider = transform.Find("/Slider").GetComponent<Slider>();
            // tex = transform.Find("/Slider/Background/InputField").GetComponent<InputField>();
            // bg = transform.Find("/Slider/Background/Mask/BG").GetComponent<RawImage>();

            switch (channel)
            {
                case ColorChannel.R:
                    tagTex.text = "R  ";
                    break;
                case ColorChannel.G:
                    tagTex.text = "G  ";
                    break;
                case ColorChannel.B:
                    tagTex.text = "B  ";
                    break;
                case ColorChannel.A:
                    tagTex.text = "A  ";
                    break;
            }
        }
    }
}

