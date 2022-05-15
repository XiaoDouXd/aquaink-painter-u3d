using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class APHotKeyMgr : MonoBehaviour
{
    public bool dragging;

    private void Update()
    {
        // 重置
        dragging = false;
        
        // 检测
        if (Input.GetKey(KeyCode.Space))
            dragging = true;
    }
    
    #region 单例类
    public static APHotKeyMgr I => _i;
    private static APHotKeyMgr _i;
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            DontDestroyOnLoad(transform.parent);
        }
        else
            Destroy(gameObject);
    }
    #endregion
}
