using System;
using UnityEngine;

namespace AP.Canvas
{
    public class MapInfoBase
    {
        public MapBase map;

        public virtual void DoRelease()
        {
            map = null;
        }
    }
    
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
        public bool Released => _isRelease;
        /// <summary>
        /// 贴图输出
        /// </summary>
        public abstract Texture Tex { get; }
        /// <summary>
        /// 数据输出
        /// </summary>
        public abstract MapInfoBase Info { get; }
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
        /// 释放贴图集
        /// </summary>
        public virtual void DoRelease()
        {
            if (Released) return;
            
            _mapBaseList.Remove(this);
            _isRelease = true;
        }
        /// <summary>
        /// 写入贴图
        /// </summary>
        /// <param name="act"> 写入内容的方法 </param>
        public virtual void DoWrite(Action<MapInfoBase> act)
        {
            act?.Invoke(Info);
        }
        /// <summary>
        /// 载入贴图
        /// </summary>
        /// <param name="info"></param>
        public virtual void DoLoad(MapInfoBase info) { }
        /// <summary>
        /// 保存贴图
        /// </summary>
        /// <returns></returns>
        public virtual MapInfoBase DoSave(MapInfoBase container)
        {
            return container;
        }
        /// <summary>
        /// 制作贴图容器
        /// </summary>
        /// <returns></returns>
        public virtual MapInfoBase NewEmptyInfo()
        {
            return new MapInfoBase();
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
            _isRelease = false;
            CutInLine((uint)typ);
        }

        // -----------------------------------------------------
        /// <summary>
        /// 大小
        /// </summary>
        private (uint, uint) _size;
        private bool _isRelease;
    }
}