using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

/// <summary>
/// 基于D2Q9结构的扩散模型测试
/// </summary>
[RequireComponent(typeof(Camera))]
public class D2Q9FlowTest : MonoBehaviour
{
    public Material d2Q9MatF0;
    public Material d2Q9MatF1234;
    public Material d2Q9MatF5678;
    public Material visualization;
    public Texture2D initVelocity;

    // ------------------------------- 获取的组件
    // 相机
    private Camera _camera;
    
    // ------------------------------- 内部参数
    private RenderTexture _rtF0;
    private RenderTexture _rtF1234;
    private RenderTexture _rtF5678;
    private Texture2D _texPaper;
    private bool _refreshState = true;

    private RenderTexture _rtTemp1;

    private void SetD2Q9Mat(Texture f0, Texture f1234, Texture f5678, Material mat)
    {
        mat.SetTexture("_LastTex0", f0);
        mat.SetTexture("_LastTex1234", f1234);
        mat.SetTexture("_LastTex5678", f5678);
    }

    #region Unity 内建函数
    private void Awake()
    {
        // 拿到相机组件
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        Application.targetFrameRate = 12;
        // 初始化渲染贴图
        _rtF0 = new RenderTexture(512, 512, 0, GraphicsFormat.R16_SFloat);
        _rtF1234 = new RenderTexture(512, 512, 0, GraphicsFormat.R16G16B16A16_SFloat);
        _rtF5678 = new RenderTexture(512, 512, 0, GraphicsFormat.R16G16B16A16_SFloat);
        _rtTemp1 = new RenderTexture(512, 512, 0, GraphicsFormat.R16G16B16A16_SFloat);

        SetD2Q9Mat(_rtF0, _rtF1234, _rtF5678, d2Q9MatF0);
        SetD2Q9Mat(_rtF0, _rtF1234, _rtF5678, d2Q9MatF1234);
        SetD2Q9Mat(_rtF0, _rtF1234, _rtF5678, d2Q9MatF5678);
        SetD2Q9Mat(_rtF0, _rtF1234, _rtF5678, visualization);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F5))
        {
            _refreshState = true;
            Debug.Log("按了F5: 刷新");
        }
    }
    
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_refreshState)
        {
            Graphics.Blit(initVelocity, _rtF0);
            Graphics.Blit(initVelocity, _rtF1234);
            Graphics.Blit(initVelocity, _rtF5678);
            _refreshState = false;
        }
        Graphics.Blit(null, _rtTemp1, d2Q9MatF0);
        Graphics.Blit(_rtTemp1, _rtF0);
        Graphics.Blit(null, _rtTemp1, d2Q9MatF1234);
        Graphics.Blit(_rtTemp1, _rtF1234);
        Graphics.Blit(null, _rtTemp1, d2Q9MatF5678);
        Graphics.Blit(_rtTemp1, _rtF5678);

        Graphics.Blit(src, dest, visualization);
    }

    #endregion
}
