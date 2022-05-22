using AP.Canvas;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APFileDataUI : MonoBehaviour
    {
        public int maxWidth;
        public int maxHeight;
        
        public InputField widthInput;
        public InputField heightInput;
        public Button button;

        public GameObject mask;

        private int _wid;
        private int _hei;

        public void InputData()
        {
            if (string.IsNullOrWhiteSpace(widthInput.text))
                widthInput.text = "1024";
            if (string.IsNullOrWhiteSpace(heightInput.text))
                heightInput.text = "1024";
            
            if (int.TryParse(widthInput.text, out var width))
            {
                if (width > maxWidth)
                {
                    widthInput.text = $"{maxWidth}";
                    width = maxWidth;
                }
                else if (width < 1)
                {
                    widthInput.text = "1";
                    width = 1;
                }

                _wid = width;
            }
            if (int.TryParse(heightInput.text, out var height))
            {
                if (height > maxHeight)
                {
                    heightInput.text = $"{maxHeight}";
                    height = maxHeight;
                }
                else if (height < 1)
                {
                    heightInput.text = "1";
                    height = 1;
                }

                _hei = height;
            }
        }
        
        public void NewCanvas()
        {
            if (_wid == 0)
            {
                widthInput.text = "1024";
                _wid = 1024;
            }
            if (_hei == 0)
            {
                heightInput.text = "1024";
                _hei = 1024;
            }
            
            var canvas = APAssetObjMgr.CanvasObjs.Clone(
                "SurfaceObj",
                APInitMgr.I.surfaceRoot.gameObject);
            canvas.GetComponent<APCanvasUI>().Init(
                new APCanvasInfo()
                {
                    width = _wid,
                    height = _hei,
                    paper = APInitMgr.I.defaultPaper1,
                });
            canvas.SetActive(true);
            mask.SetActive(true);
            
            MapRenderer.I.Refresh();
        }
    }
}

