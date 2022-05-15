namespace AP.Canvas
{
    /// <summary>
    /// 贴图排序类型
    /// </summary>
    public enum MapRankTypes : uint
    {
        WATER_FLOW = 100,
        COLOR_FIX = 200,
        LAYER = 300,
        CANVAS = 400,
    }

    /// <summary>
    /// 混合模式
    /// </summary>
    public enum LayerBlurType : uint
    {
        NORMAL = 0,
        REALITY = 1,
        MULTIPLY = 2,
        DARKEN = 3,
        ADD = 4,
        OVERLAY = 5,
    }
}
