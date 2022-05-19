using UnityEngine;
using UnityEngine.InputSystem;

public class APUIMgr : MonoBehaviour
{
    public bool EnableUIEvent { get; set; }
    public GameObject UIRoot => gameObject;

    private void Start()
    {
        EnableUIEvent = true;
    }
    
    #region 单例类
    public static APUIMgr I => _i;
    private static APUIMgr _i;
    
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
    #region 工具函数
    public GameObject Clone(GameObject obj, GameObject parent = null)
    {
        if (!obj) return null;
        if (parent)
            return Instantiate(obj, parent.transform);
        else
            return Instantiate(obj, obj.transform.parent);
    }
    public (Vector2 pos, bool isInside) MousePos2Rect(RectTransform rt)
    {
        if (!rt)
            return (Input.mousePosition, false);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, 
                Mouse.current.position.ReadValue(), 
                Camera.main, 
                out var pos);

        var rect = rt.rect;
        var x = (pos.x + rect.width * 0.5f) / rect.width;
        var y = (pos.y + rect.height * 0.5f) / rect.height;
        
        return (new Vector2(x, y), x <= 1 && x >= 0 && y <= 1 && y >= 0);
    }
    public Vector2 Clamp01(Vector2 i)
    {
        return new Vector2(Mathf.Clamp01(i.x), Mathf.Clamp01(i.y));
    }

    public float SignedAngle(Vector2 from, Vector2 to)
    {
        return Vector2.SignedAngle(from, to) + 180;
    }
    #endregion
}
