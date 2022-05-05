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

    public Color color;
    public Texture2D gule;
    public Texture2D paper;
    public Texture2D writeTex;
    public Material visualization;
    public Texture2D colTable;
    
    private static APCanvasMgr _i;
    private MapFlowD2Q9 _flow;
    private MapColor _col;
    private MapEnumerable _flowList;

    public static Texture2D GetColTable()
    {
        return _i.colTable;
    }
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            // Application.targetFrameRate = 30;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _flow = new MapFlowD2Q9(Screen.width, Screen.height, paper, gule);
        _flowList = new MapEnumerable();
        _col = new MapColor(Screen.width, Screen.height, _flow);
        
        // 初始化
        _flow.DoWrite(null, Vector4.zero, initTex, initTex, initTex);
        visualization.SetTexture("_LastTex0", _flow.F0);
        visualization.SetTexture("_LastTex1234", _flow.F1234);
        visualization.SetTexture("_LastTex5678", _flow.F5678);
        visualization.SetTexture("_Adv", _col.Glue);
        visualization.SetTexture("_Fix", _col.Tex);
        visualization.SetTexture("_ColTable", colTable);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float posx = Mathf.Clamp01(Input.mousePosition.x / Screen.width);
            float posy = Mathf.Clamp01(Input.mousePosition.y / Screen.height);

            Vector4 rect = new Vector4(posx - radius, posy - radius, posx + radius, posy + radius);
            _flow.DoWrite(rect, writeTex, writeTex, writeTex);
            _col.DoWrite(rect, color, writeTex);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            radius += radius >= 1.0 ? 0 : 0.005f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            radius -= radius <= 0.005 ? 0 : 0.005f;
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        _flow.DoUpdate();
        _col.DoUpdate();
        
        Graphics.Blit(_col.Tex, dest, visualization);
    }
}
