using System.Collections;
using UnityEngine;

namespace BunchoWatch
{
    public class BunchoAction
    {
        public bool IsRunning { get; protected set; } = true;

        public virtual IEnumerator Action()
        {
            yield break;
        }
    }

    public class Stand : BunchoAction
    {
        public override IEnumerator Action()
        {
            yield break;
        }
    }

    public class Jump : BunchoAction
    {
        private readonly float _height = 400.0f;
        private readonly float _duration = 0.3f;
        private readonly Transform _target;
        private readonly Vector3 _toward;
        private readonly BunchoAnimator _animator;

        public Jump(BunchoAnimator controller, Transform target, Vector3 toward)
        {
            _animator = controller;
            _target = target;
            _toward = toward;
        }

        public override IEnumerator Action()
        {
            var direction = _toward - _target.position;
            yield return _animator.JumpStandby(direction.x > 0 ? MoveDirection.Right : MoveDirection.Left);
            yield return new WaitForSeconds(1.0f);
            var t = 0f;
            var z = _target.position.z;
            var start = _target.position;
            
            while (t < 1f)
            {
                t += Time.deltaTime / _duration;
                float clamped = Mathf.Clamp01(t);

                // 水平は直線（等速）
                Vector2 pos = Vector2.Lerp(start, _toward, clamped);
                float theta = Mathf.Lerp(0, Mathf.PI, clamped);

                // 縦だけ放物線オフセット：4h t(1-t) は t=0.5 で最大 h
                float sin = Mathf.Sin(theta);
                float parabola = sin * _height;
                pos.y += parabola;

                _target.position = new Vector3(pos.x, pos.y, z);
                yield return null;
            }
            
            yield return new WaitForSeconds(1.0f);
            yield return _animator.Mochi();
            
            _target.position = new Vector3(_toward.x, _toward.y, z);
            IsRunning = false;
        }
    }

    public class Walk : BunchoAction
    {
        private readonly Vector3 _toward;
        private readonly Transform _target;
        private readonly BunchoAnimator _animator;

        public Walk(BunchoAnimator animator, Transform target, Vector3 toward)
        {
            _target = target;
            _toward = toward;
            _animator = animator;
        }

        public override IEnumerator Action()
        {
            var distance = _toward - _target.position;
            while (distance.x > 0)
            {
                var direction = distance.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                int amount = (int)(distance.x - BunchoAnimator.WalkMoveAmount);
                if (amount < 0)
                {
                    amount = (int)distance.x;
                }

                yield return _animator.Walk(direction, amount);
                distance = _toward - _target.position;
            }

            _target.position = _toward;
            IsRunning = false;
        }
    }

    public class Idle : BunchoAction
    {
        public override IEnumerator Action()
        {
            float time = 0;
            while (time < 3.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            IsRunning = false;
        }
    }

    public class Sleep : BunchoAction
    {
        public override IEnumerator Action()
        {
            float time = 0;
            while (time < 3.0f)
            {
                time += Time.deltaTime;
                yield return null;
            }

            IsRunning = false;
        }
    }

    public class DeepSleep : BunchoAction
    {
        public override IEnumerator Action()
        {
            yield break;
        }
    }
}