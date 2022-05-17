using UnityEngine;

public class APUIMgr : MonoBehaviour
{
    public GameObject UIRoot => gameObject;
    
    #region 单例类
    public static APUIMgr I => _i;
    private static APUIMgr _i;
    
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
