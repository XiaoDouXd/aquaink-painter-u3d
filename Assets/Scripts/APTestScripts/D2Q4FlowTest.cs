using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

/// <summary>
/// 基于D2Q4结构的扩散模型测试
/// </summary>
[RequireComponent(typeof(Camera))]
//[ExecuteAlways]
public class D2Q4FlowTest : MonoBehaviour
{
    public Material d2Q4Mat;
    public Material visualization;
    public Texture2D initVelocity;

    // ------------------------------- 获取的组件
    // 相机
    private Camera _camera;
    
    // ------------------------------- 内部参数
    private RenderTexture _rtVelocity;
    private Texture2D _texPaper;
    private bool _refreshState = true;

    private RenderTexture _rtTemp1;

    #region Unity 内建函数
    private void Awake()
    {
        // 拿到相机组件
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        Application.targetFrameRate = 10;
        // 初始化渲染贴图
        _rtVelocity = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBHalf);
        _rtTemp1 = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBHalf);

        d2Q4Mat.SetTexture("_LastTex", _rtVelocity);
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
            Graphics.Blit(initVelocity, _rtVelocity);
            _refreshState = false;
        }
        Graphics.Blit(src, _rtTemp1, d2Q4Mat);
        Graphics.Blit(_rtTemp1, _rtVelocity);
        
        Graphics.Blit(_rtVelocity, dest, visualization);
    }

    #endregion
}
