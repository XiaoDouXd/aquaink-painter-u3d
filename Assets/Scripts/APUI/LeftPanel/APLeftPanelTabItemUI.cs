using System;
using System.ComponentModel;
using AP.UI;
using UnityEngine;
using UnityEngine.UI;

public class APLeftPanelTabItemUI : MonoBehaviour
{
    public bool IsDebut => _debuting;
    
    public Action<APLeftPanelTabItemUI> debutComplete;
    public Action<APLeftPanelTabItemUI> startComplete;

    private UITabInfo _info;
    private Text _text;
    private RectTransform _rectTransform;
    private Vector2 _defaultSize;
    private Button _button;
    private Animator _buttonAnimator;
    private bool _debuting;
    private bool _started;
    private bool _inited;

    public void Debut()
    {
        if (!_inited) return;
        
        _debuting = true;
        _buttonAnimator.enabled = false;
        var desc = LeanTween.size(_rectTransform, _defaultSize, 1f);
        desc.setEase(LeanTweenType.easeOutQuart);
        desc.setOnComplete(() =>
        {
            _debuting = false;
            _buttonAnimator.enabled = true;
        });
    }

    public void Init(UITabInfo info)
    {
        if (!_started)
            throw new WarningException("APLeftPanelTabItemUI.Init: 警告！尝试在Start前初始化！");

        _info = info;
        var tabName = "";
        foreach (var c in _info.name)
        {
            tabName += $"{c}\n";
        }
        _text.text = tabName.Substring(0, Mathf.Max(tabName.Length-1, 0) );
        _defaultSize.y = _text.preferredHeight + 50;
        _rectTransform.sizeDelta = new Vector2(0, _defaultSize.y);
        _info.select += Select;
        _info.deselect += Deselect;

        _inited = true;
        
        Debut();
    }
    
    private void Select(UITabInfo info)
    {
        _button.Select();
        _info.isChosen = true;
    }
    private void Deselect(UITabInfo info)
    {
        _button.OnDeselect(null);
        _info.isChosen = true;
    }
    
    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _buttonAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _defaultSize = _rectTransform.sizeDelta;
        _rectTransform.sizeDelta = new Vector2(0, _defaultSize.y);
        _buttonAnimator.enabled = false;
        _started = true;

        startComplete?.Invoke(this);
    }
}
