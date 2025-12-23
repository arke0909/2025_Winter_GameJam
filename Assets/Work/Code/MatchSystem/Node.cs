using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Work.Code.Manager;

namespace Work.Code.MatchSystem
{
    public class Node : MonoBehaviour, IDragHandler,  IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private NodeType nodeType;
        [SerializeField] private Image icedImage;
        [SerializeField] private Image targetingImage;
        [SerializeField] private float deltaThreshold = 50f;
        [SerializeField] private float nodeMoveSpeed = 800;
        [SerializeField] private float sizeMultiply = 0.8f;

        public NodeType NodeType => nodeType;
        public RectTransform Rect => transform as RectTransform;
        public bool IsIced => _isIced;
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public float XPos { get; private set; } 
        public float YPos { get; private set; } 
        
        public Vector2 CenterPos { get; private set; }
        
        private MatchSystem _matchSystem;
        
        private Vector2 _onPressPos;

        private bool _dragged;
        private bool _isIced;
        
        public void Init(int x, int y, MatchSystem matchSystem, bool isIced)
        {
            _matchSystem = matchSystem;
            _isIced = isIced;
            icedImage.enabled = isIced;
            OnTargeting(false);
            CenterPos = Rect.TransformPoint(Rect.rect.center);
            
            SetXY(x, y);
        }

        private void OnDestroy()
        {
            Rect.DOKill();
            targetingImage.DOKill();
        }

        public async UniTask SetPos(float x, float y, bool isTween = true)
        {
            Rect.DOKill(true);

            Vector2 target = new Vector2(x, y);
            Vector2 start = Rect.anchoredPosition;

            XPos = x;
            YPos = y;


            if (!isTween)
            {
                Rect.anchoredPosition = target;
                CenterPos = Rect.TransformPoint(Rect.rect.center);

                return;
            }

            float distance = Vector2.Distance(start, target);
            float duration = distance / nodeMoveSpeed;

            try
            {
                await Rect
                    .DOAnchorPos(target, duration)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => CenterPos = Rect.TransformPoint(Rect.rect.center))
                    .AsyncWaitForCompletion();
            }
            catch
            {
                // 파괴 중 취소
            }
        }

        public void OnTargeting(bool value)
        {
            targetingImage.rectTransform.DOKill(); 
            targetingImage.enabled = value;

            if (value)
            {
                targetingImage.rectTransform.localScale = Vector3.one;

                float targetScaleValue = (sizeMultiply <= 0) ? 0.8f : sizeMultiply;
                Vector3 targetScale = new Vector3(targetScaleValue, targetScaleValue, 1f);

                targetingImage.rectTransform.DOScale(targetScale, 1.25f)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(gameObject);
            }
            else
            {
                targetingImage.rectTransform.localScale = Vector3.one;
            }
        }
        
        public void SetXY(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public void Unfreeze()
        {
            if (!_isIced) return;

            _isIced = false;
            icedImage.enabled = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (_dragged || GameManager.Instance.LeftTurnCount <= 0)
                return; 
            {
                _onPressPos = eventData.pressPosition;
                Vector2 delta = eventData.position - _onPressPos;

                if (delta.magnitude > deltaThreshold)
                {
                    Vector2Int dir = GetDragDir(delta);
                    if (dir == Vector2Int.zero)
                        return;

                    _matchSystem.TrySwapByDir(this, dir);
                    _dragged = true;
                }
            }
        }
        
        
        
        private Vector2Int GetDragDir(Vector2 delta)
        {
            return Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? new Vector2Int(Math.Sign(delta.x), 0) : new Vector2Int(0, Math.Sign(-delta.y));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragged = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _matchSystem.OnNodeClicked(this);
        }
    }
}