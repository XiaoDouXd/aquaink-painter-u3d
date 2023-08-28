using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Domain : byte
{
    None = 0,
    Canvas = 1,
    UI = 2
}

public class APAssetObjMgr : MonoBehaviour
{
    public Domain domain;
    public List<GameObject> objs;

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

    private readonly Dictionary<string, GameObject> _objs = new();

    #region Inst

    public static APAssetObjMgr UIObjs => _uI;
    public static APAssetObjMgr CanvasObjs => _cI;

    private void Awake()
    {
        var illegal = false;

        // 单例类
        switch (domain)
        {
            case Domain.None:
                Destroy(gameObject);
                break;
            case Domain.Canvas:
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
            default: throw new ArgumentOutOfRangeException();
        }

        if (illegal) return;

        // 收取游戏物体
        foreach (var obj in objs)
        {
            _objs[obj.name] = obj;
            obj.SetActive(false);
        }
    }

    private static APAssetObjMgr _cI;
    private static APAssetObjMgr _uI;

    #endregion
}
