using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MixboxBlurTest : MonoBehaviour
{
    public Material mixboxBlurMat;
    public Texture2D ctableTex;

    public Color colorLeft;
    public Color colorRight;

    #region Unity 内建函数
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if ( mixboxBlurMat)
        {
            mixboxBlurMat.SetColor("_Color1", colorLeft);
            mixboxBlurMat.SetColor("_Color2", colorRight);
            Graphics.Blit(ctableTex, dest, mixboxBlurMat);
        }
    }
    #endregion
}
