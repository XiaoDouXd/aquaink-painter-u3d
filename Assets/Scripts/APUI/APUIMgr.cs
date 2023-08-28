using UnityEngine;
using UnityEngine.InputSystem;

public class APUIMgr : MonoBehaviour
{
    public float canvasScalerWid;

    public bool EnableUIEvent { get; set; }
    public GameObject UIRoot => gameObject;

    private void Start()
    {
        EnableUIEvent = true;
    }

    #region 单例类

    public static APUIMgr I => _i;

    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private static APUIMgr _i;

    #endregion

    #region 工具函数

    public GameObject Clone(GameObject obj, GameObject parent = null)
    {
        return !obj ? null : Instantiate(obj, parent ? parent.transform : obj.transform.parent);
    }
    public (Vector2 pos, bool isInside) MousePos2Rect(RectTransform rt)
    {
        if (!rt) return (Input.mousePosition, false);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt,
                Mouse.current.position.ReadValue(),
                Camera.main,
                out var pos);

        var rect = rt.rect;
        var x = (pos.x + rect.width * 0.5f) / rect.width;
        var y = (pos.y + rect.height * 0.5f) / rect.height;
        return (new Vector2(x, y), x is <= 1 and >= 0 && y is <= 1 and >= 0);
    }

    public Vector2 Clamp01(Vector2 i) => new(Mathf.Clamp01(i.x), Mathf.Clamp01(i.y));
    public float SignedAngle(Vector2 from, Vector2 to) => Vector2.SignedAngle(from, to) + 180;
    public float GetCanvasScaleHeight() => canvasScalerWid / APInitMgr.I.WindowAspect;

    #endregion
}
