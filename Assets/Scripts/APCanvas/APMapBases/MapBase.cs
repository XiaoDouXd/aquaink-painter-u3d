using UnityEngine;

namespace AP.Canvas
{
    /// <summary>
    /// 贴图集类
    /// 用于定义贴图集: 如水贴图, 颜料贴图等
    /// </summary>
    public abstract partial class MapBase
    {
        // -----------------------------------------------------
        /// <summary>
        /// 宽
        /// </summary>
        public int Width => (int) _size.Item1;
        /// <summary>
        /// 高
        /// </summary>
        public int Height => (int) _size.Item2;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; set; } = true;
        /// <summary>
        /// 贴图输出
        /// </summary>
        public abstract Texture Tex { get; }
        /// <summary>
        /// 刷新贴图
        /// </summary>
        public virtual void DoUpdate() { }
        /// <summary>
        /// 刷新贴图
        /// <param name="material"> 材质 </param>
        /// <param name="texs"> 贴图 </param>
        /// </summary>
        public virtual void DoUpdate(Material material, params Texture[] texs) { }
        /// <summary>
        /// 重置贴图
        /// </summary>
        public virtual void DoRecreate(int width, int height, bool clear = true)
        {
            _size = ((uint)width, (uint)height);
        }
        /// <summary>
        /// 释放贴图集
        /// </summary>
        public virtual void DoRelease()
        {
            _mapBaseList.Remove(this);
        }

        // -----------------------------------------------------
        /// <summary>
        /// 贴图集
        /// </summary>
        /// <param name="width"> 宽 </param>
        /// <param name="height"> 高 </param>
        /// <param name="typ"> 贴图集类型(决定了在什么时候被渲染) </param>
        protected MapBase(uint width, uint height, MapRankTypes typ)
        {
            _size = (width, height);
            CutInLine((uint)typ);
        }

        // -----------------------------------------------------
        /// <summary>
        /// 大小
        /// </summary>
        private (uint, uint) _size;
    }
}