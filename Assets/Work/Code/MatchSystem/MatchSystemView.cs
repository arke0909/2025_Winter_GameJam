using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Lib.ObjectPool.RunTime;
using Lib.Utiles;
using UnityEngine;
using Work.Code.Events;
using Work.Code.SoundSystem;

namespace Work.Code.MatchSystem
{
    // Owns board layout and presentation. MatchSystem decides rules and board changes.
    [Serializable]
    public class MatchSystemView
    {
        [SerializeField] private EventChannelSO particleEventChannel;
        [SerializeField] private PoolItemSO soundPlayer;
        [SerializeField] private SoundSO swapSound;
        [SerializeField] private SoundSO removeSound;
        [SerializeField] private SoundSO iceBreakSound;
        [SerializeField] private SoundSO unlockSound;
        [SerializeField] private PoolItemSO particleItem;
        [SerializeField] private PoolItemSO icedEffectItem;
        [SerializeField] private PoolItemSO lockedEffectItem;
        [SerializeField] private PoolItemSO lineEffectItem;
        [SerializeField] private PoolItemSO threeXthreeEffectItem;
        [SerializeField] private PoolItemSO nyanCatEffectItem;
        [SerializeField] private Node[] nodePrefabs;
        [SerializeField] private Node lockedNodePrefab;
        [SerializeField] private RectTransform nodeBoard;

        public Node[] NodePrefabs => nodePrefabs;
        public Node LockedNodePrefab => lockedNodePrefab;

        private PoolManagerMono _poolManager;
        private float _nodeWidth, _nodeHeight, _widthTerm, _heightTerm;

        public void Initialize(int width, int height, PoolManagerMono poolManager)
        {
            _poolManager = poolManager;
            RectTransform rt = nodePrefabs[0].transform as RectTransform;
            _nodeWidth = rt.rect.width;
            _nodeHeight = rt.rect.height;
            _widthTerm = (nodeBoard.rect.width - _nodeWidth * width) / width;
            _heightTerm = (nodeBoard.rect.height - _nodeHeight * height) / height;
        }

        public Node CreateNode(Node prefab, int x, int y, MatchSystem matchSystem, bool isIced)
        {
            Node node = UnityEngine.Object.Instantiate(prefab, nodeBoard);
            node.Init(x, y, matchSystem, isIced);
            return node;
        }

        public UniTask MoveNode(Node node, int x, int y, bool animate = true)
        {
            float posX = x * _nodeWidth + (x + 0.5f) * _widthTerm;
            float posY = y * -_nodeHeight - (y + 0.5f) * _heightTerm;
            return node.SetPos(posX, posY, animate);
        }

        public async UniTask ShowBoard(Node[,] nodes)
        {
            for (int y = nodes.GetLength(0) - 1; y >= 0; y--)
            {
                for (int x = 0; x < nodes.GetLength(1); x++)
                {
                    if (nodes[y, x] != null)
                        MoveNode(nodes[y, x], x, y).Forget();
                }

                await UniTask.Delay(100);
            }
        }

        public async UniTask SwapNodes(Node a, Node b)
        {
            _poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(swapSound);
            Vector2 aPos = a.Rect.anchoredPosition;
            Vector2 bPos = b.Rect.anchoredPosition;
            await UniTask.WhenAll(a.SetPos(bPos.x, bPos.y), b.SetPos(aPos.x, aPos.y));
        }

        public void RemoveNode(Node node)
        {
            _poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(removeSound);
            PoolItemSO effect = node.TryGetComponent<LockedNode>(out _) ? lockedEffectItem : particleItem;
            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(effect, node.CenterPos, Quaternion.identity));
            UnityEngine.Object.Destroy(node.gameObject);
        }

        public void SetTargeting(IEnumerable<Node> nodes, bool value)
        {
            foreach (Node node in nodes)
                node.OnTargeting(value);
        }

        public void PlayLineEffect(Node node, bool vertical)
        {
            Vector2 pos = vertical ? new Vector2(node.CenterPos.x, 0) : new Vector2(0, node.CenterPos.y);
            Quaternion rotation = vertical ? Quaternion.Euler(0, 0, 90f) : Quaternion.identity;
            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(lineEffectItem, pos, rotation));
        }

        public void PlayIceBreakEffect(Node node)
        {
            _poolManager.Pop<SoundPlayer>(soundPlayer).PlaySound(iceBreakSound);
            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(icedEffectItem, node.CenterPos, Quaternion.identity));
        }

        public void PlayAreaEffect(Node node)
        {
            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(threeXthreeEffectItem, node.CenterPos));
        }

        public void PlayClearBoardEffect()
        {
            particleEventChannel.InvokeEvent(
                ParticleEvents.PlayUIParticleEvent.Initializer(nyanCatEffectItem, new Vector2(15, 0)));
        }
    }
}
