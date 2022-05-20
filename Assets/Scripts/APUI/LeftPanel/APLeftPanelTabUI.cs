using System;
using System.Collections.Generic;
using UnityEngine;

namespace AP.UI
{
    [Serializable]
    public class UITabInfo
    {
        public string editorName;
        public string name;
        public bool isChosen;
        public APLeftPanelContentItemUI content;
    
        [HideInInspector] public APLeftPanelTabItemUI item;
        [HideInInspector] public APLeftPanelTabUI tabMgr;
        [HideInInspector] public int id;
        public Action<UITabInfo> select;
        public Action<UITabInfo> deselect;
    }
    
    public class APLeftPanelTabUI : MonoBehaviour
    {
        public int defaultTab;
        [Space(10)]
        public APLeftPanelUI leftPanel;
        public APLeftPanelContentUI content;
        public List<UITabInfo> tabs;
        public APLeftPanelTabItemUI sourceObj;

        private APLeftPanelTabItemUI _curItem;

        public void OnButton(APLeftPanelTabItemUI item)
        {
            var closePanel = false;
            if (item == _curItem)
            {
                closePanel = true;
                _curItem = null;
            }
            else
            {
                _curItem = item;
            }
            
            
            foreach (var vaTab in tabs)
            {
                if (vaTab.item == item)
                {
                    vaTab.select?.Invoke(closePanel ? null : vaTab);
                }
                else
                    vaTab.deselect?.Invoke(vaTab);
            }
        }
        
        private void Start()
        {
            if (!sourceObj)
                throw new ApplicationException("APLeftPanelTabUI.Start: 错误！UI菜单Tab丢失！");
            
            sourceObj.gameObject.SetActive(false);
            
            // 根据 Tab 列表初始化 Tab 标签
            if (tabs == null || tabs.Count == 0)
                throw new ApplicationException("APLeftPanelTabUI.Start: 错误！UI菜单Tab丢失！");
    
            for (var idx = 0; idx < tabs.Count; idx++)
            {
                tabs[idx].item = APUIMgr.I.Clone(sourceObj.gameObject).GetComponent<APLeftPanelTabItemUI>();
                tabs[idx].item.gameObject.transform.name = $"GenTab_{tabs[idx].editorName}";
                tabs[idx].tabMgr = this;
                if (idx == defaultTab)
                {
                    tabs[idx].select?.Invoke(tabs[idx]);
                }
                else
                {
                    tabs[idx].deselect?.Invoke(tabs[idx]);
                }
    
                var idx1 = idx;
                tabs[idx].item.startComplete = (item) =>
                {
                    item.Init(tabs[idx1]);
                };
                tabs[idx].item.gameObject.SetActive(true);
                tabs[idx].select += (info) =>
                {
                    leftPanel.SelectTab(info == null ? null : info.item);
                };
            }
            
            content.Init(this);
        }
    }
}


