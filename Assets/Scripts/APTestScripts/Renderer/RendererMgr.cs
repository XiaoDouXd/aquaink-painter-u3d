using UnityEngine;


namespace APRenderer
{
    [RequireComponent(typeof(Camera))]
    public class RendererMgr : MonoBehaviour
    {
        #region 一些全局设置
        public Vector2Int CanvasSize { get; private set; }
        public GameObject LayerObjs { get; private set; }
        #endregion

        #region 显示渲染循环
        private Camera m_show;
        #endregion

        #region Unity内建
        void Awake()
        {
            OnInit();
        }
        void Start()
        {
            m_show = GetComponent<Camera>();
            LayerObjs = new("layer");
        }
        void Update()
        {

        }
        #endregion

        #region 单例类
        public static RendererMgr I { get; private set; }

        private void OnInit()
        {
            if (I == null) I = this;
            else Destroy(this);
        }
        #endregion
    }
}
