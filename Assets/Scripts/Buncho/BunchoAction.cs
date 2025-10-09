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

    public class Mochi : BunchoAction
    {
        private readonly BunchoAnimator _animator;
        
        public Mochi(BunchoAnimator animator)
        {
            _animator = animator;
        }

        public override IEnumerator Action()
        {
            yield return _animator.Mochi();
            yield return new WaitForSeconds(3.0f);

            yield return _animator.CloseEye();
            yield return new WaitForSeconds(5.0f);

            yield return _animator.OpenEye();
            yield return new WaitForSeconds(5.0f);

            yield return _animator.MochiKubiKashige();
            yield return new WaitForSeconds(10.0f);
            
            yield return _animator.Mochi();
            yield return new WaitForSeconds(10.0f);
            IsRunning = false;
        }
    }

    public class Jump : BunchoAction
    {
        private readonly float _height = 400.0f;
        private readonly float _duration = 0.3f;
        private readonly Transform _target;
        private readonly Vector3 _toward;
        private readonly BunchoAnimator _animator;

        public Jump(BunchoAnimator animator, Transform target, Vector3 toward)
        {
            _animator = animator;
            _target = target;
            _toward = toward;
        }

        public override IEnumerator Action()
        {
            var direction = _toward - _target.position;
            yield return _animator.JumpStandby(direction.x > 0 ? MoveDirection.Right : MoveDirection.Left);
            yield return new WaitForSeconds(3.0f);
            var t = 0f;
            var z = _target.position.z;
            var start = _target.position;

            _animator.Reset();
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

            yield return _animator.JumpStandby(MoveDirection.Center);
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
            var direction = (_toward - _target.position).x > 0 ? MoveDirection.Right : MoveDirection.Left;
            var distance = Mathf.Abs((int)(_toward - _target.position).x);
            while (distance > 0)
            {
                var amount = Mathf.Clamp(distance - BunchoAnimator.WalkMoveAmount, 0, BunchoAnimator.WalkMoveAmount);
                if (amount <= 0)
                {
                    amount = distance;
                }

                yield return _animator.Walk(direction, amount);
                distance = Mathf.Abs((int)(_toward - _target.position).x);
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