using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens.Game
{
    public class SettingsPanel : Panel
    {
        [SerializeField] private Slider _touchSensitivitySlider;
        [SerializeField] private Slider _mouseSensitivitySlider;

        public override void Init()
        {
            _touchSensitivitySlider.value = SettingsManager.instance.touchSensitivity;
            _mouseSensitivitySlider.value = SettingsManager.instance.mouseSensitivity;
            
            _touchSensitivitySlider.onValueChanged.AddListener(OnTouchSensitivityChanged);
            _mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            base.Init();
        }

        private void OnTouchSensitivityChanged(float value)
        {
            SettingsManager.instance.touchSensitivity = value;
        }

        private void OnMouseSensitivityChanged(float value)
        {
            SettingsManager.instance.mouseSensitivity = value;
        }

        public override void Dispose()
        {
            _touchSensitivitySlider.onValueChanged.RemoveListener(OnTouchSensitivityChanged);
            _mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChanged);
            base.Dispose();
        }
    }
}