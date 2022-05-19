using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UITabInfo
{
    public string editorName;
    public string name;
    public bool isChosen;
    public APLeftPanelContentUI content;

    [HideInInspector] public APLeftPanelTabItemUI item;
    [HideInInspector] public APLeftPanelTabUI tabMgr;
    [HideInInspector] public int id;
    public Action select;
    public Action deselect;
}

public class APLeftPanelTabUI : MonoBehaviour
{
    public int defaultTab;
    public int curTab;
    public List<UITabInfo> tabs;
    public APLeftPanelTabItemUI sourceObj;

    public void UniqueSelect(UITabInfo tab)
    {
        foreach (var vaTab in tabs)
        {
            if (vaTab == tab)
                vaTab.select?.Invoke();
            else
                vaTab.deselect?.Invoke();
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
                tabs[idx].select?.Invoke();
            }
            else
            {
                tabs[idx].deselect?.Invoke();
            }

            var idx1 = idx;
            tabs[idx].item.startComplete = (item) =>
            {
                item.Init(tabs[idx1]);
            };
            tabs[idx].item.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
