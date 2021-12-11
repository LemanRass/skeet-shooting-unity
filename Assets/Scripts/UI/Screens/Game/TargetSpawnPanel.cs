using System;
using DefaultNamespace;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Game
{
    public class TargetSpawnPanel : Panel
    {
        [SerializeField] private Button _spawnBtn;

        public event Action spawnTargetClickEvent;

        public override void Init()
        {
            _spawnBtn.onClick.AddListener(OnSpawnTargetClicked);
            
            TargetsManager.instance.targetSpawnedEvent += OnTargetSpawned;
            TargetsManager.instance.targetDestroyingEvent += OnTargetDestroying;
            
            base.Init();
        }
        
        private void OnSpawnTargetClicked()
        {
            spawnTargetClickEvent?.Invoke();
        }

        private void OnTargetSpawned(TargetController target)
        {
            Hide();
        }

        private void OnTargetDestroying(TargetController target)
        {
            Show();
        }
        
        public override void Dispose()
        {
            _spawnBtn.onClick.RemoveListener(OnSpawnTargetClicked);
            
            TargetsManager.instance.targetSpawnedEvent -= OnTargetSpawned;
            TargetsManager.instance.targetDestroyingEvent -= OnTargetDestroying;
        }
    }
}