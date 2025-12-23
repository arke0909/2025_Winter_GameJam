using UnityEngine;
using Work.Code.MatchSystem;

namespace Work.Code.Items
{
    [CreateAssetMenu(fileName = "Remove3CrossNode", menuName = "SO/Item/Node/Remove 3CrossNode")]
    public class Remove3CrossNode : EffectNodeSO
    {
        public override void Execute(MatchSystem.MatchSystem ms, NodeData data)
        {
            int x = data.Pos.x;
            int y = data.Pos.y;
            
            for (int xn = -1; xn < 2; xn++)
            {
                int nx = x + xn;

                ms.RemoveVertical(nx);
            }
            for (int yn = -1; yn < 2; yn++)
            {
                int ny = y + yn;

                ms.RemoveHorizontal(ny);
            }
        }
    }
}