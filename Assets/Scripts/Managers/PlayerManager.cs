using System;
using DefaultNamespace;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : Manager<PlayerManager>
    {
        [SerializeField] private Transform _camera;
        [SerializeField] private GunController _gun;
        [Space]
        [SerializeField] private float _minVerticalAngle = -90f;
        [SerializeField] private float _maxVerticalAngle = 90f;
        [Space]
        [SerializeField] private float _minHorizontalAngle = -30.0f;
        [SerializeField] private float _maxHorizontalAngle = 30.0f;

        public event Action<TargetController> targetFoundEvent;
        public event Action<TargetController> targetLostEvent;
        public event Action<TargetController> targetHitEvent;
        public event Action<TargetController> targetMissEvent;
        
        private float _rotX;
        private float _rotY;

        private TargetController _target;
        private TargetController _aimedTarget;

        private bool _isTargetFound;
        private bool _isInitialized;

        public override void Init()
        {
            base.Init();
            
            if (_isInitialized)
            {
                Debug.LogError("[PlayerController -> Init] Already initialized!");
                return;
            }
            
            TargetsManager.instance.targetSpawnedEvent += OnTargetSpawned;
            TargetsManager.instance.targetDestroyingEvent += OnTargetDestroying;
            TargetsManager.instance.targetAlmostReachedGroundEvent += OnTargetAlmostReachedGround;
            
            _rotX = _camera.localEulerAngles.x;
            _rotY = _camera.localEulerAngles.y;
            _isInitialized = true;
        }

        public override void Dispose()
        {
            TargetsManager.instance.targetSpawnedEvent -= OnTargetSpawned;
            TargetsManager.instance.targetDestroyingEvent -= OnTargetDestroying;
            TargetsManager.instance.targetAlmostReachedGroundEvent -= OnTargetAlmostReachedGround;
            
            base.Dispose();
        }

        private void OnTargetSpawned(TargetController target)
        {
            _target = target;
        }

        private void OnTargetDestroying(TargetController target)
        {
            _target = null;
        }
        
        private void Update()
        {
            if (!_isInitialized) return;
            
            HandleRotation();
            HandleAim();
            HandleShooting();
        }

        private void HandleRotation()
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) 
                {
                    var touch = Input.GetTouch(0);
 
                    _rotY += touch.deltaPosition.x * SettingsManager.instance.touchSensitivity;
                    _rotX += touch.deltaPosition.y * SettingsManager.instance.touchSensitivity;
                }
            }
            else
            {
                _rotY += Input.GetAxis("Mouse X") * SettingsManager.instance.mouseSensitivity;
                _rotX += Input.GetAxis("Mouse Y") * SettingsManager.instance.mouseSensitivity;
            }
            
            
            _rotX = Mathf.Clamp(_rotX, _minVerticalAngle, _maxVerticalAngle);
            _rotY = Mathf.Clamp(_rotY, _minHorizontalAngle, _maxHorizontalAngle);

            _camera.localEulerAngles = new Vector3(-_rotX, _rotY, 0.0f);
        }

        private void HandleAim()
        {
            if (_target == null) return;

            var ray1 = _target.transform.position - _camera.position;
            var ray2 = _camera.forward * 100.0f;

            float angle = Vector3.Angle(ray1, ray2);

            if (_isTargetFound)
            {
                if (angle > 3.0f)
                {
                    _isTargetFound = false;
                    OnTargetLost(_target);
                }
            }
            else
            {
                if (angle < 3.0f)
                {
                    _isTargetFound = true;
                    OnTargetFound(_target);
                }
            }
        }

        
        
        private DateTime _targetFoundDateTime;
        private float _ticksRequired;
        private float _ticksCurrent;


        private void HandleShooting()
        {
            if (_aimedTarget == null) return;

            _ticksCurrent += Time.deltaTime;

            if (_ticksCurrent >= _ticksRequired)
            {
                TimeToShoot();
            }
        }
        
        private void OnTargetFound(TargetController target)
        {
            _aimedTarget = target;
            targetFoundEvent?.Invoke(_target);
            
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
        
        private void OnTargetAlmostReachedGround(TargetController target)
        {
            if (_isTargetFound)
            {
                TimeToShoot();
            }
        }
        
        private void OnTargetLost(TargetController target)
        {
            targetLostEvent?.Invoke(_aimedTarget);
            _ticksCurrent = 0.0f;
            _ticksRequired = float.MaxValue;
            _aimedTarget = null;
        }

        private void TimeToShoot()
        {
            _gun.Fire();
            
            var targetFlyingDuration = DateTime.Now - _target.firedDateTime;
            if (targetFlyingDuration.TotalSeconds <= 4.0f)
            {
                //100% hit chance
                targetHitEvent?.Invoke(_target);
                TargetsManager.instance.Hit(_aimedTarget);
            }
            else
            {
                //lucky shot calculation
                double chance = (DateTime.Now - _targetFoundDateTime).TotalSeconds;
                double rand = UnityEngine.Random.Range(0, 10) / 10.0d; //0.3

                if (chance >= rand)
                {
                    Debug.Log($"[HIT] Chance: {chance} Rand: {rand}");
                    targetHitEvent?.Invoke(_target);
                    TargetsManager.instance.Hit(_aimedTarget);
                }
                else
                {
                    Debug.Log($"[MISS] Chance: {chance} Rand: {rand}");
                    targetMissEvent?.Invoke(_target);
                }
            }
        }
    }
}