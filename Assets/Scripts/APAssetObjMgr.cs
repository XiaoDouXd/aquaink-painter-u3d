using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Domain : byte
{
    NONE = 0,
    CANVAS = 1,
    UI = 2,
}

public class APAssetObjMgr : MonoBehaviour
{
    public Domain domain;
    public List<GameObject> objs;

    private Dictionary<string, GameObject> _objs = 
        new Dictionary<string, GameObject>();

    public GameObject Clone(string objName, GameObject parent)
    {
        var obj = _objs[objName];
        if (!obj) return null;
        if (parent)
        {
            var o = Instantiate(obj, parent.transform);
            o.SetActive(true);
            return o;
        }
        else
        {
            var o = Instantiate(obj);
            o.SetActive(true);
            return o;
        }
            
    }
    
    #region 多例类
    public static APAssetObjMgr CanvasObjs => _cI;
    public static APAssetObjMgr UIObjs => _uI;
    
    private static APAssetObjMgr _cI;
    private static APAssetObjMgr _uI;
    
    private void Awake()
    {
        var illegal = false;
        
        // 单例类
        switch (domain)
        {
            case Domain.NONE:
                Destroy(gameObject);
                break;
            case Domain.CANVAS:
                if (_cI == null)
                {
                    _cI = this;
                    gameObject.SetActive(false);
                    break;
                }
                Destroy(gameObject);
                illegal = true;
                break;
            case Domain.UI:
                if (_uI == null)
                {
                    _uI = this;
                    gameObject.SetActive(false);
                    break;
                }
                Destroy(gameObject);
                illegal = true;
                break;
        }

        if (illegal) return;
        
        // 收取游戏物体
        foreach (var obj in objs)
        {
            _objs[obj.name] = obj;
            obj.SetActive(false);
        }
    }
    #endregion
}
