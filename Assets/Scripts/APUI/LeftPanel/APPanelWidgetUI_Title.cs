using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APPanelWidgetUI_Title : APPanelWidgetUI
    {
        public Text titleText;
        
        public override void Init(APPanelWidgetInfo info)
        {
            base.Init(info);
            if (titleText)
            {
                if (info.stringParams != null && info.stringParams.Count != 0)
                    titleText.text = info.stringParams[0];
            }
        }
    }
}


