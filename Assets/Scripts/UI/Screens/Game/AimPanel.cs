using System;
using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Game
{
    public class AimPanel : Panel
    {
        [SerializeField] private Image _aimFillImg;

        private DateTime _targetFoundDateTime;
        
        private TargetController _target;
        private float _ticksCurrent;
        private float _ticksRequired;

        public override void Init()
        {
            PlayerManager.instance.targetFoundEvent += OnTargetFound;
            PlayerManager.instance.targetLostEvent += OnTargetLost;

            TargetsManager.instance.targetDestroyingEvent += OnTargetDestroying;
            
            base.Init();
        }

        private void OnTargetDestroying(TargetController target)
        {
            if (_target != target) return;
                
            _target = null;
            _ticksCurrent = 0.0f;
            _ticksRequired = float.MaxValue;
            SetFill(0.0f);
        }

        private void OnTargetFound(TargetController target)
        {
            _target = target;
            
            _targetFoundDateTime = DateTime.Now;

            var difference = _targetFoundDateTime - target.firedDateTime;

            if (difference.TotalSeconds <= 1.0f)
            {
                _ticksRequired = 0.25f;
            }
            else if (difference.TotalSeconds <= 2.0f)
            {
                _ticksRequired = 0.5f;
            }
            else if (difference.TotalSeconds <= 3.0f)
            {
                _ticksRequired = 0.75f;
            }
            else if (difference.TotalSeconds <= 4.0f)
            {
                _ticksRequired = 1.0f;
            }
            else
            {
                _ticksRequired = 1.0f;
            }
        }

        private void OnTargetLost(TargetController target)
        {
            _ticksCurrent = 0.0f;
            _target = null;
            SetFill(0.0f);
        }

        private void Update()
        {
            if (_target == null) return;

            if (_ticksCurrent < _ticksRequired)
            {
                _ticksCurrent += Time.deltaTime;
                SetFill(_ticksCurrent / _ticksRequired);
            }
        }

        public override void Dispose()
        {
            PlayerManager.instance.targetFoundEvent -= OnTargetFound;
            PlayerManager.instance.targetLostEvent -= OnTargetLost;
            
            TargetsManager.instance.targetDestroyingEvent -= OnTargetDestroying;
            
            base.Dispose();
        }

        private void SetFill(float amount)
        {
            amount = Mathf.Clamp01(amount);
            _aimFillImg.fillAmount = amount;
        }
    }
}
