using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dragText;
    [SerializeField] private Image _handImage;
    private RectTransform _handRect;
    [SerializeField] private float _radiusCos;
    [SerializeField] private float _radiusSin;
    [SerializeField] private float _rateCos;
    [SerializeField] private float _rateSin;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Image _infinityImage;
    private void Awake()
    {
        _handRect = _handImage.rectTransform;
        _dragText.DOFade(0.5f, 1).From(1).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        _handRect.anchoredPosition = new Vector2(_radiusCos * Mathf.Cos(_rateCos * Time.time), _radiusSin * Mathf.Sin(_rateSin * Time.time)) + _offset;
    }
}