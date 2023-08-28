using UnityEngine;

public class APHotKeyMgr : MonoBehaviour
{
    #region Inst

    public static APHotKeyMgr I => _i;

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

    private static APHotKeyMgr _i;

    #endregion
}
