using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Work.Code.MatchSystem
{
    public class Node : MonoBehaviour, IDragHandler,  IEndDragHandler
    {
        [SerializeField] private NodeType nodeType;
        [SerializeField] private float deltaThreshold = 50f;
        [SerializeField] private float nodeMoveDuration = 0.25f;

        public NodeType NodeType => nodeType;
        public RectTransform Rect => transform as RectTransform;
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public float XPos { get; private set; } 
        public float YPos { get; private set; } 
        
        private MatchSystem _matchSystem;
        
        private Vector2 _onPressPos;
        private bool _dragged;
        
        public void Init(int x, int y, MatchSystem matchSystem)
        {
            _matchSystem = matchSystem;
            SetXY(x, y);
        }

        private void OnDestroy()
        {
            Rect.DOKill();
        }

        public async UniTask SetPos(float x, float y, bool isTween = true)
        {
            if(!isTween)
                Rect.anchoredPosition = new Vector2(x, y);
            try
            {
                await MoveTween(x, y).WithCancellation(destroyCancellationToken);
            }
            catch (Exception e)
            {
                Debug.Log("디버그 중 게임 종료");
            }
        }

        private IEnumerator MoveTween(float x, float y)
        {
            bool isEnd = false;
            
            Rect.DOAnchorPos(new Vector2(x, y), nodeMoveDuration)
                .OnComplete(() =>
                {
                    XPos = x;
                    YPos = y;
                    Rect.anchoredPosition = new Vector2(x, y);
                    isEnd = true;
                });
            
            yield return new WaitUntil(() => isEnd);
        }

        public void SetXY(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragged)
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
    }
}