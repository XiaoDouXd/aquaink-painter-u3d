using APMaps;
using UnityEngine;
using MapEnumerable = APMaps.MapBase.MapEnumerable;

/// <summary>
/// 画布管理器
/// </summary>
[RequireComponent(typeof(Camera))]
public class APCanvasMgr : MonoBehaviour
{
    public Texture2D initTex;
    [Range(0, 1)]
    public float radius = 0.05f;

    public Texture2D paper;
    public Texture2D writeTex;
    public Material visualization;
    
    private static APCanvasMgr _i;
    private MapFlowD2Q9 _flow;
    private MapEnumerable _flowList;

    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            Application.targetFrameRate = 24;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _flow = new MapFlowD2Q9(Screen.width, Screen.height, paper, null);
        _flowList = new MapEnumerable(MapRankTypes.WATER_FLOW);
        
        // 初始化
        _flow.DoWrite(null, Vector4.zero, initTex, initTex, initTex);
        visualization.SetTexture("_LastTex0", _flow.F0);
        visualization.SetTexture("_LastTex1234", _flow.F1234);
        visualization.SetTexture("_LastTex5678", _flow.F5678);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float posx = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
            float posy = Mathf.Clamp01(Input.mousePosition.y / Screen.height);

            Vector4 rect = new Vector4(posx - radius, posy - radius, posx + radius, posy + radius);
            _flow.DoWrite(rect, writeTex, writeTex, writeTex);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        foreach (var flow in _flowList)
        {
            flow.DoUpdate();
        }
        Graphics.Blit(_flow.Tex, dest, visualization);
    }
}
