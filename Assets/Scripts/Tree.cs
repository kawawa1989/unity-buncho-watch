using System.Collections.Generic;
using UnityEngine;

namespace BunchoWatch
{
    public class Tree : MonoBehaviour
    {
        [SerializeField]
        private Transform[] positions;


        public Vector3 GetPosition(int index)
        {
            return positions[index].position;
        }

        public int PickNearest(int index)
        {
            var min = Mathf.Max(index - 3, 0);
            var max = Mathf.Min(index + 3, positions.Length);
            return Random.Range(min, max);
        }
        
        // 左側にいる場合、右に向かうようにする
        // 右側にいる場合は左ぬ向かうようにする
        public int Pick(int index)
        {
            // index が positions.Length / 2 以下である場合、左向き
            // それ以外は右向きと扱う
            int dir = index <= positions.Length / 2 ? 1 : -1;

            // 右向き(左へ向かう)
            if (dir < 0)
            {
                var min = Mathf.Max(index - 3, 0);
                var max = index;
                return Random.Range(min, max);
            }
            else
            {
                var min = index;
                var max = Mathf.Min(index + 3, positions.Length);
                return Random.Range(min, max);
            }
        }
    }
}