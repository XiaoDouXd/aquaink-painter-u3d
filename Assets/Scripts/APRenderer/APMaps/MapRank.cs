using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace APMaps
{
    /// <summary>
    /// 给贴图集排序
    /// </summary>
    public abstract partial class MapBase
    {
        // 一级表
        private static readonly LinkedList<MapBaseList> InstList = new LinkedList<MapBaseList>();
        // 本体在二级表的索引
        private LinkedListNode<MapBase> _mapListNode;
        private MapBaseList _mapBaseList;
        
        
        /// <summary>
        /// 插队
        /// </summary>
        private void CutInLine(uint code)
        {
            var inst = InstList.First;

            if (inst == null)
            {
                var mapList = new MapBaseList(code);
                mapList.Add(this);
                InstList.AddLast(mapList);
                return;
            }
            
            while (inst.Next != null)
            {
                if (inst.Value.Code == code)
                {
                    inst.Value.Add(this);
                    return;
                }
                else if (inst.Value.Code > code)
                {
                    var mapList = new MapBaseList(code);
                    mapList.Add(this);
                    InstList.AddBefore(inst, mapList);
                }

                inst = inst.Next;
            }
        }

        /// <summary>
        /// 贴图集集
        /// </summary>
        private class MapBaseList : IEnumerable<MapBase>
        {
            public uint Code { get; }
            public bool Empty => _mapList == null || _mapList.Count == 0;

            private LinkedList<MapBase> _mapList;
            private LinkedListNode<MapBaseList> _thisInList;

            public MapBaseList(uint code)
            {
                Code = code;
                _mapList = new LinkedList<MapBase>();
            }

            public void Add(MapBase map)
            {
                if (map._mapListNode != null)
                    Remove(map);

                map._mapListNode = _mapList.AddLast(map);
                map._mapBaseList = this;
            }

            public void Remove(MapBase map)
            {
                _mapList.Remove(map._mapListNode);
                if (_mapList.Count != 0) return;
                
                InstList.Remove(_thisInList);
                _thisInList = null;
                _mapList = null;
            }

            public IEnumerator<MapBase> GetEnumerator()
            {
                if (_mapList == null)
                    yield break;

                foreach (var map in _mapList)
                {
                    yield return map;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
        /// <summary>
        /// 全局贴图遍历器
        /// </summary>
        public class MapEnumerable : IEnumerable<MapBase>
        {
            private readonly MapBaseList _list;
            private readonly bool _setCode;

            public MapEnumerable(MapRankTypes? code = null)
            {
                if (code == null) return;
                _setCode = true;

                foreach (var inst in InstList.Where(inst => inst.Code == (uint) code))
                {
                    _list = inst;
                    return;
                }
            }
            
            public IEnumerator<MapBase> GetEnumerator()
            {
                if ((_list == null || _list.Empty) && _setCode) 
                    yield break;
                
                if (_list != null)
                {
                    foreach (var map in _list)
                    {
                        yield return map;
                    }
                    yield break;
                }
                
                foreach (var map in InstList.SelectMany(inst => inst))
                {
                    yield return map;
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}

