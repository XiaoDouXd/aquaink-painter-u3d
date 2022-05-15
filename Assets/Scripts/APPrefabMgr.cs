using UnityEngine;

public class APPrefabMgr : MonoBehaviour
{
    public GameObject surfaceObj;

    #region 单例类
    public static APPrefabMgr I => _i;
    private static APPrefabMgr _i;
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            gameObject.SetActive(false);
        }
        else
            Destroy(gameObject);
    }
    #endregion
}
