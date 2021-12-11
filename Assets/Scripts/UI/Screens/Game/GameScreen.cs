using Managers;
using UnityEngine;

namespace UI.Screens.Game
{
    public class GameScreen : Screen
    {
        [SerializeField] private AimPanel _aimPanel;
        [SerializeField] private TargetSpawnPanel _targetSpawnPanel;
        [SerializeField] private ScorePanel _scorePanel;
        [SerializeField] private SettingsPanel _settingsPanel;
        
        public override void Init()
        {
            base.Init();
            
            _aimPanel.Init();
       
            _targetSpawnPanel.Init();
            _targetSpawnPanel.spawnTargetClickEvent += OnSpawnTargetClicked;
            
            _scorePanel.Init();
            
            _settingsPanel.Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpawnTargetClicked();
            }
        }

        private void OnSpawnTargetClicked()
        {
            TargetsManager.instance.Fire();
        }
        
        public override void Dispose()
        {
            _aimPanel.Dispose();
            
            _targetSpawnPanel.spawnTargetClickEvent -= OnSpawnTargetClicked;
            _targetSpawnPanel.Dispose();
            
            _scorePanel.Dispose();
            
            _settingsPanel.Dispose();
            base.Dispose();
        }
    }
}
