namespace AP.Canvas
{
    /// <summary>
    /// 贴图排序类型
    /// </summary>
    public enum MapRankTypes : uint
    {
        None = 0,
        WaterFlow = 100,
        ColorFix = 200,
        Layer = 300,
        Canvas = 400,
    }

    /// <summary>
    /// 混合模式
    /// </summary>
    public enum LayerBlurType : uint
    {
        Normal = 0,
        Reality = 1,
        Multiply = 2,
        Darken = 3,
        Add = 4,
        Overlay = 5,
    }
}
