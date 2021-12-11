using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class TargetController : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _flyingCurve;
        [Space]
        [SerializeField] private GameObject _explosion;

        public DateTime firedDateTime { get; private set; }
        
        private float _flyingTimeLimit = 5.0f;
        private float _flyingTimeCurrent = 0.0f;

        private Vector3 _startPoint;
        private Vector3 _endPoint;
        private float _maxHeight;
        
        private bool _isFlying;

        public event Action<TargetController> groundReachedEvent;
        public event Action<TargetController> groundAlmostReachedEvent;

        private bool _isAlmostReachedSent;
        
        public void Fire(Vector3 endPoint, float maxHeight)
        {
            firedDateTime = DateTime.Now;

            _startPoint = transform.position;
            _endPoint = endPoint;
            _maxHeight = maxHeight;
            
            transform.position = Vector3.zero;
            _flyingTimeCurrent = 0.0f;
            _isFlying = true;
        }
        
        private void Update()
        {
            if (!_isFlying) return;
            
            _flyingTimeCurrent += Time.deltaTime / _flyingTimeLimit;
            _flyingTimeCurrent = Mathf.Clamp01(_flyingTimeCurrent);

            transform.position = Utils.Parabola(_startPoint, _endPoint, _maxHeight,
                _flyingCurve.Evaluate(_flyingTimeCurrent));

            if (_flyingTimeCurrent >= 1.0f)
            {
                _isFlying = false;
                groundReachedEvent?.Invoke(this);
            }
            else if (_flyingTimeCurrent >= 0.99f)
            {
                if (!_isAlmostReachedSent)
                {
                    groundAlmostReachedEvent?.Invoke(this);
                    _isAlmostReachedSent = true;
                }
            }
        }

        public void Explode()
        {
            var go = Instantiate(_explosion);
            go.transform.position = transform.position;
        }
        
        public void Dispose()
        {
            //Free all resources here
            _isAlmostReachedSent = false;
            _isFlying = false;
        }
    }
}