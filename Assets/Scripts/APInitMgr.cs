using AP.Canvas;
using AP.UI;
using UnityEngine;

public class APInitMgr : MonoBehaviour
{
    // ---------------------------------------------------------------------------
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
    [Tooltip("UI根节点")]
    public RectTransform surfaceRoot;
    
    // ---------------------------------------------------------------------------
    // 渲染初始化
    private void Start()
    {
        var canvas = APAssetObjMgr.CanvasObjs.Clone(
            "SurfaceObj",
            surfaceRoot.gameObject);
        canvas.GetComponent<APCanvasUI>().Init(
            new APCanvasInfo()
            {
                width = 1024,
                height = 1024,
                paper = defaultPaper1,
            });
        canvas.SetActive(true);
        StartCoroutine(MapRenderer.I.RenderCoroutine());
    }

    // ---------------------------------------------------------------------------
    #region 单例类
    public static APInitMgr I => _i;
    private static APInitMgr _i;
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            // 锁帧
            Application.targetFrameRate = 120;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    #endregion
    #region 工具函数
    public Vector2 WindowCenter => new Vector2(Screen.width/2.0f, Screen.height/2.0f);
    public Vector2 WindowSize => new Vector2(Screen.width, Screen.height);
    #endregion
}