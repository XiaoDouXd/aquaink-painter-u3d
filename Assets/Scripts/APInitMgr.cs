using UnityEngine;

public class APInitMgr : MonoBehaviour
{
    // 这里用于挂在初始化需要的资源
    [Tooltip("用于混色模型的颜色-颜料映射表")]
    public Texture2D colorTable;
    [Tooltip("默认纸张纹理")]
    public Texture2D defaultPaper1;
    [Tooltip("默认纸张纹理")]
    public Texture2D defaultPaper2;
    [Tooltip("默认纸张纹理")]
    public Texture2D defaultPaper3;
    [Tooltip("默认纸张纹理")]
    public Texture2D defaultPaper4;
    [Tooltip("默认纸张纹理")]
    public Texture2D defaultPaper5;

    #region 单例类
    public static APInitMgr I => _i;
    private static APInitMgr _i;
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            Application.targetFrameRate = 30;
            DontDestroyOnLoad(transform.parent);
        }
        else
            Destroy(gameObject);
    }
    #endregion
}
