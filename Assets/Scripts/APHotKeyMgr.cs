using UnityEngine;

public class APHotKeyMgr : MonoBehaviour
{
    #region 单例类
    public static APHotKeyMgr I => _i;
    private static APHotKeyMgr _i;
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    #endregion
}
