using System.Collections.Generic;
using AP.Canvas;
using UnityEngine;

namespace AP
{
    public enum APPersistentOp : uint
    {
        NONE = 0,
        DRAW = 1,
        // DELETE_LAYER = 10,
        // ADD_LAYER = 100,
    }
    
    public class APPersistentOperation
    {
        public APPersistentOp op;
        public APCanvas canvas;
        public APLayer layer;
    }
    
    public class APPersistentMgr : MonoBehaviour
    {
        private const uint MaxStep = 10;

        private class APPersistentOperationInfo
        {
            public int layerId;
            public APLayerPersistentInfo layer;
        }

        private class APPersistentLink
        {
            public LinkedList<APPersistentOperationInfo> link;
            public LinkedListNode<APPersistentOperationInfo> curNode;
        }
        private Dictionary<APCanvas, APPersistentLink> _saveData
            = new Dictionary<APCanvas, APPersistentLink>();

        public bool GoBack(APCanvas op)
        {
            if (_saveData.TryGetValue(op, out var list))
            {
                if (list.curNode.Previous != null)
                {
                    list.curNode = list.curNode.Previous;
                    var id = list.curNode.Value.layerId;
                    var layerInfo = list.curNode.Value.layer;
                    
                    op[id].DoLoad(layerInfo);
                    return true;
                }

                return false;
            }

            return false;
        }
        public bool GoForward(APCanvas op)
        {
            if (_saveData.TryGetValue(op, out var list))
            {
                if (list.curNode.Next != null && list.curNode.Next.Next != null)
                {
                    list.curNode = list.curNode.Next.Next;
                    var id = list.curNode.Value.layerId;
                    var layerInfo = list.curNode.Value.layer;
                    
                    op[id].DoLoad(layerInfo);
                    return true;
                }

                return false;
            }

            return false;
        }
        public void DoSave(APPersistentOperation op)
        {
            var can = op.canvas;
            if (_saveData.TryGetValue(can, out var opList))
            {
                if (opList.curNode == null)
                {
                    opList.link.Clear();
                    var layerInfo = op.layer.NewEmptyInfo();
                    var i = new APPersistentOperationInfo()
                    {
                        layerId = op.layer.Id,
                        layer =  layerInfo as APLayerPersistentInfo,
                    };
                    
                    op.layer.DoSave(i.layer);
                    opList.link.AddLast(i);
                    opList.curNode = opList.link.Last;
                }
                else if (opList.curNode == opList.link.Last && opList.link.Count >= MaxStep)
                {
                    var fir = opList.link.First.Value;
                    opList.link.RemoveFirst();
                    fir.layerId = op.layer.Id;
                    op.layer.DoSave(fir.layer);
                    opList.link.AddLast(fir);
                    opList.curNode = opList.link.Last;
                }
                else if (opList.curNode == opList.link.Last)
                {
                    var layerInfo = op.layer.NewEmptyInfo();
                    var i = new APPersistentOperationInfo()
                    {
                        layerId = op.layer.Id,
                        layer =  layerInfo as APLayerPersistentInfo,
                    };
                    op.layer.DoSave(i.layer);
                    opList.link.AddLast(i);
                    opList.curNode = opList.link.Last;
                }
                else if (opList.curNode != opList.link.Last)
                {
                    opList.curNode = opList.curNode.Next;
                    while (opList.curNode != null)
                    {
                        var tmp = opList.curNode;
                        opList.curNode = opList.curNode.Next;
                        tmp.Value.layer.DoRelease();
                        opList.link.Remove(tmp);
                    }
                    var layerInfo = op.layer.NewEmptyInfo();
                    var i = new APPersistentOperationInfo()
                    {
                        layerId = op.layer.Id,
                        layer =  layerInfo as APLayerPersistentInfo,
                    };
                    op.layer.DoSave(i.layer);
                    opList.link.AddLast(i);
                    opList.curNode = opList.link.Last;
                }
            }
            else
            {
                _saveData[can] = new APPersistentLink()
                {
                    link = new LinkedList<APPersistentOperationInfo>(),
                };
                
                var layerInfo = op.layer.NewEmptyInfo();
                var i = new APPersistentOperationInfo()
                {
                    layerId = op.layer.Id,
                    layer =  layerInfo as APLayerPersistentInfo,
                };
                op.layer.DoSave(i.layer);
                _saveData[can].link.AddLast(i);
                _saveData[can].curNode = _saveData[can].link.Last;
            }
        }
        public void DoDelete(APCanvas can)
        {
            if (_saveData.TryGetValue(can, out var list))
            {
                foreach (var info in list.link)
                {
                    info.layer.DoRelease();
                }
                
                list.link.Clear();
                list.curNode = null;
                _saveData.Remove(can);
            }
        }
        
        #region 单例类
        public static APPersistentMgr I => _i;
        private static APPersistentMgr _i;
    
        private void Awake()
        {
            if (_i == null)
            {
                _i = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        #endregion
    }
}


