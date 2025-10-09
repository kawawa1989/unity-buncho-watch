using System;
using UnityEngine;

namespace BunchoWatch
{
    public class FieldController : MonoBehaviour
    { 
        [SerializeField]
        private Perch[] perches;

        [SerializeField] 
        private BunchoController buncho;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            buncho.Initialize(this);
        }

        public Perch GetPerchAt(int index)
        {
            return perches[index];
        }

        /// <summary>
        /// スクリーン座標から最も近いPerchのPositionを取得するメソッド
        /// </summary>
        /// <param name="screenPosition">タッチされたスクリーン座標</param>
        /// <returns>最も近いPerchのPosition</returns>
        public (Vector3 position, int perchIndex, int positionIndex) GetNearestPerchPosition(Vector2 screenPosition)
        {
            if (perches == null || perches.Length == 0)
            {
                Debug.LogError("Perchesが設定されていません");
                return (Vector3.zero, 0, 0);
            }

            // スクリーン座標をワールド座標に変換
            var nearestPosition = Vector3.zero;
            var nearestDistance = float.MaxValue;
            var resultPerchIndex = 0;
            var resultPositionIndex = 0;

            // 全てのPerchの全てのPositionをチェック
            for (var perchIndex = 0; perchIndex < perches.Length; perchIndex++)
            {
                // Perchの全てのポジションをチェック
                for (var position = 0; position < perches[perchIndex].GetPositionCount(); position++)
                {
                    var perchPosition = perches[perchIndex].GetPosition(position);
                    var distance = Vector2.Distance(screenPosition, new Vector2(perchPosition.x, perchPosition.y));
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPosition = perchPosition;
                        resultPerchIndex = perchIndex;
                        resultPositionIndex = position;
                    }
                }
            }

            return (nearestPosition, resultPerchIndex, resultPositionIndex);
        }
    }
}