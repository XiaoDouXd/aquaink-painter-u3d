using UnityEngine;

public class APCanvasMgr : MonoBehaviour
{
    #region 单例类
    public static APCanvasMgr I => _i;

    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private static APCanvasMgr _i;

    #endregion
}
