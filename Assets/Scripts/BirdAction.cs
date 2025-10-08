using System.Collections;
using UnityEngine;

namespace BunchoWatch
{
    public class BirdAction
    {
        public bool IsRunning { get; protected set; } = true;

        public virtual IEnumerator Action()
        {
            yield break;
        }
    }

    public class Stand : BirdAction
    {
        public override IEnumerator Action()
        {
            yield break;
        }
    }

    public class Jump : BirdAction
    {
        private readonly float _height = 400.0f;
        private readonly float _duration = 0.3f;
        private readonly Transform _target;
        private readonly Vector3 _toward;
        private readonly BirdController _controller;

        public Jump(BirdController controller, Transform target, Vector3 toward)
        {
            _controller = controller;
            _target = target;
            _toward = toward;
        }

        public override IEnumerator Action()
        {
            _controller.Jump(0);
            yield return new WaitForSeconds(1.0f);
            float t = 0f;
            // Zはそのまま
            float z = _target.position.z;
            var start = _target.position;

            _controller.Jump(1);
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
            
            _controller.Jump(0);
            yield return new WaitForSeconds(1.0f);
            _controller.Idle(0);
            
            _target.position = new Vector3(_toward.x, _toward.y, z);
            IsRunning = false;
        }
    }

    public class Walk : BirdAction
    {
        private readonly float _duration = 0.5f;
        private readonly Vector3 _start;
        private readonly Vector3 _toward;
        private readonly Transform _target;
        private readonly BirdController _controller;

        public Walk(BirdController controller, Transform target, Vector3 toward)
        {
            _target = target;
            _start = _target.position;
            _toward = toward;
            _controller = controller;
        }

        public override IEnumerator Action()
        {
            float time = 0;
            while (time < _duration)
            {
                float t = time / _duration;
                _target.position = Vector3.Lerp(_start, _toward, t);
                time += Time.deltaTime;
                _controller.Walk();
                yield return null;
            }

            _target.position = _toward;
            IsRunning = false;
        }
    }

    public class Idle : BirdAction
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

    public class Sleep : BirdAction
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

    public class DeepSleep : BirdAction
    {
        public override IEnumerator Action()
        {
            yield break;
        }
    }
}