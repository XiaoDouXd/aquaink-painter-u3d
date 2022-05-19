using AP;
using UnityEngine;

public class APLeftPanelUI : MonoBehaviour, IAnimaLauncher
{
    public bool isOpen;
    public float animaFactor;
    public float maxPos;
    public float minPos;
    
    public float Factor => animaFactor;
    
    private APLeftPanelTabUI _tab;
    private APLeftPanelContentUI _content;
    private Animator _anima;
    private RectTransform _rectTrans;
    
    private static readonly int IsOpen = Animator.StringToHash("isOpen");

    private void Start()
    {
        _tab = GetComponentInChildren<APLeftPanelTabUI>();
        _content = GetComponentInChildren<APLeftPanelContentUI>();
        _anima = GetComponent<Animator>();
        _rectTrans = GetComponent<RectTransform>();
        _rectTrans.offsetMax = new Vector2(-minPos, _rectTrans.anchorMax.y);

        APAnimaMgr.I.Register(this, (f) =>
        {
            
            _rectTrans.offsetMax = new Vector2(-Mathf.Lerp(minPos, maxPos, f), _rectTrans.anchorMax.y);
        }, delegate(float f)
        {
            return LeanTween.easeOutQuad(0, 1, f);
        });
    }
    
    private void Update()
    {
        _anima.SetBool(IsOpen, isOpen);
    }
}
