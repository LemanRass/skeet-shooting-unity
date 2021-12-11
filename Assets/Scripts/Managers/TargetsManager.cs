using System;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class TargetsManager : Manager<TargetsManager>
    {
        public event Action<TargetController> targetSpawnedEvent;
        public event Action<TargetController> targetAlmostReachedGroundEvent;
        public event Action<TargetController> targetReachedGroundEvent;
        public event Action<TargetController> targetDestroyingEvent;
        
        [SerializeField] private Transform[] _parents;

        private TargetController _prefab;
        private TargetController _currentTarget;
        private bool _isInitialized;
        
        public override void Init()
        {
            if (_isInitialized)
            {
                Debug.LogError("[TargetSpawner -> Init] Already initialized!");
                return;
            }
            
            _prefab = Resources.Load<TargetController>("Prefabs/TargetPrefab");
            _isInitialized = true;
        }

        public bool CheckIfCanFire()
        {
            if (!_isInitialized)
            {
                Debug.LogError("[TargetSpawner -> Fire] Not initialized yet.");
                return false;
            }

            if (!ReferenceEquals(_currentTarget, null))
                return false;
            
            return true;
        }
        
        public void Fire()
        {
            if (!CheckIfCanFire()) return;
            
            var parent = FindRandomParent();

            var go = Instantiate(_prefab, parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            _currentTarget = go.GetComponent<TargetController>();

            var endPos = _currentTarget.transform.forward * Random.Range(80.0f, 120.0f);
            var maxHeight = Random.Range(5.0f, 20.0f);
            _currentTarget.Fire(endPos, maxHeight);

            _currentTarget.groundReachedEvent += OnGroundReached;
            _currentTarget.groundAlmostReachedEvent += OnGroundAlmostReached;
            
            targetSpawnedEvent?.Invoke(_currentTarget);
        }

        public void Hit(TargetController target)
        {
            if (_currentTarget != target)
                return;
            
            target.Explode();
            DestroyTarget(target);
        }

        private void OnGroundReached(TargetController target)
        {
            targetReachedGroundEvent?.Invoke(target);
            DestroyTarget(target);
        }

        private void OnGroundAlmostReached(TargetController target)
        {
            targetAlmostReachedGroundEvent?.Invoke(target);
        }
        
        private void DestroyTarget(TargetController target)
        {
            if (target != _currentTarget)
                return;
            
            targetDestroyingEvent?.Invoke(target);
            
            target.groundReachedEvent -= OnGroundReached;
            target.groundAlmostReachedEvent -= OnGroundAlmostReached;
            
            target.Dispose();
            Destroy(target.gameObject);
            _currentTarget = null;
        }
        
        private Transform FindRandomParent()
        {
            return _parents[Random.Range(0, _parents.Length)];
        }
    }
}