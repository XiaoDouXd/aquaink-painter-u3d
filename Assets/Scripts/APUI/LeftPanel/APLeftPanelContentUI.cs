using System;
using System.Collections.Generic;
using AP.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AP.UI
{
    public class APLeftPanelContentUI : MonoBehaviour
    {
        public RectMask2D mask;
        public APLeftPanelTabItemUI CurShow { get; private set; }
        public APLeftPanelTabItemUI NexShow { get; private set; }
    
        private Dictionary<APLeftPanelTabItemUI, APLeftPanelContentItemUI> _content =
            new Dictionary<APLeftPanelTabItemUI, APLeftPanelContentItemUI>();
        private bool _showing;
        private bool _exiting;
    
        public void Init(APLeftPanelTabUI tabs)
        {
            foreach (var tab in tabs.tabs)
            {
                _content[tab.item] = tab.content;
                tab.content.SetContent(this, tab.item);
            }
        }

        public void Show(APLeftPanelTabItemUI tab)
        {
            if (_showing || _exiting)
            {
                NexShow = tab;
                
                mask.enabled = _showing || _exiting;
                return;
            }
            if (tab == null && CurShow != null)
            {
                _content[CurShow].Exit();
                _exiting = true;
                CurShow = null;
                
                mask.enabled = _showing || _exiting;
                return;
            }
            else if (tab == null)
            {
                mask.enabled = _showing || _exiting;
                return;
            }

            if (CurShow != null)
            {
                _content[CurShow].Exit();
                _exiting = true;
                NexShow = tab;
                mask.enabled = _showing || _exiting;
                return;
            }
            
            CurShow = tab;
            _content[tab].Show();
            _showing = true;
            mask.enabled = _showing || _exiting;
        }
        public void OnItemShowComplete(APLeftPanelTabItemUI tab, APLeftPanelContentItemUI item)
        {
            if (NexShow != null)
            {
                _content[tab].Exit();
                _exiting = true;
            }

            _showing = false;

            mask.enabled = _showing || _exiting;
            APInitMgr.I.RenderReset();
        }
        public void OnItemExitComplete(APLeftPanelTabItemUI tab, APLeftPanelContentItemUI item)
        {
            if (NexShow != null)
            {
                CurShow = NexShow;
                NexShow = null;
                _content[CurShow].Show();

                _showing = true;
            }
            
            _exiting = false;
            
            mask.enabled = _showing || _exiting;
            APInitMgr.I.RenderReset();
        }
    }
}

