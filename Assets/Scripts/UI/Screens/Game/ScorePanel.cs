using DefaultNamespace;
using Managers;
using TMPro;
using UnityEngine;

namespace UI.Screens.Game
{
    public class ScorePanel : Panel
    {
        [SerializeField] private TextMeshProUGUI _scoreLabel;

        private int _currentScore;
        
        [SerializeField] private int _targetScore = 100;
        
        public override void Init()
        {
            PlayerManager.instance.targetHitEvent += OnPlayerHit;
            
            base.Init();
        }

        private void OnPlayerHit(TargetController obj)
        {
            _currentScore++;
            SetNewScore(_currentScore);
        }

        private void SetNewScore(int score)
        {
            _scoreLabel.text = $"{score}/{_targetScore}";
        }

        public override void Dispose()
        {
            PlayerManager.instance.targetHitEvent -= OnPlayerHit;
            base.Dispose();
        }
    }
}
