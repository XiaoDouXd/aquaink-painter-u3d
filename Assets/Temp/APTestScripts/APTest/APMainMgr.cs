using AP.Canvas;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using MapEnumerable = AP.Canvas.MapBase.MapEnumerable;

/// <summary>
/// 画布管理器
/// </summary>
[RequireComponent(typeof(Camera))]
public class APMainMgr : MonoBehaviour
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
    public RawImage uiSurface;
    
    private static APMainMgr _i;
    private APFlow _flow;
    private APColor _col;
    private MapEnumerable _flowList;
    private RenderTexture _target;

    public static Texture2D GetColTable()
    {
        return _i.colTable;
    }
    
    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            Application.targetFrameRate = 30;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        _flow = new APFlow(Screen.width, Screen.height, paper);
        _flowList = new MapEnumerable();
        _col = new APColor(Screen.width, Screen.height, _flow);
        _target = new RenderTexture(Screen.width, Screen.height, 0, GraphicsFormat.R8G8B8A8_UNorm);
        _target.Create();
        uiSurface.texture = _target;
        
        // 初始化
        //_flow.DoWrite(null, Vector4.zero, initTex, initTex, initTex);
        visualization.SetTexture("_LastTex0", _flow.F0);
        visualization.SetTexture("_LastTex1234", _flow.F1234);
        visualization.SetTexture("_LastTex5678", _flow.F5678);
        visualization.SetTexture("_Adv", _col.colAdv);
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
            //_flow.DoWrite(rect, writeTex, writeTex, writeTex);
            //_col.DoWrite(rect, color, writeTex);
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            radius += radius >= 1.0 ? 0 : 0.005f * Input.mouseScrollDelta.y;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            radius += radius <= 0.005 ? 0 : 0.005f * Input.mouseScrollDelta.y;
        }
        
        _flow.DoUpdate();
        _col.DoUpdate();
        Graphics.Blit(_col.Tex, _target, visualization);
    }

    // private void OnRenderImage(RenderTexture src, RenderTexture dest)
    // {
    //
    //     Graphics.Blit(_col.Tex, dest, visualization);
    // }
}
