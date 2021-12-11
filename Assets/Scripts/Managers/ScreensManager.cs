using System;
using System.Collections.Generic;
using UnityEngine;
using Screen = UI.Screen;

namespace Managers
{
    public class ScreensManager : Manager<ScreensManager>
    {
        private Dictionary<Type, Screen> _screens;
        private List<Screen> _openedScreens;

        public override void Init()
        {
            _screens = new Dictionary<Type, Screen>();
            _openedScreens = new List<Screen>();

            foreach (var prefab in Resources.LoadAll<GameObject>("Prefabs/UI/Screens"))
            {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                var screen = go.GetComponent<Screen>();
                screen.Init();
                _screens.Add(screen.GetType(), screen);
            }
        }

        public void Open<T>()
        {
            if (_screens.ContainsKey(typeof(T)))
            {
                var screen = _screens[typeof(T)];
                screen.Show();
                _openedScreens.Add(screen);
            }
        }

        public void CloseLast()
        {
            if (_openedScreens.Count < 1)
                return;
            
            var screen = _openedScreens[_openedScreens.Count - 1];
            screen.Hide();
            _openedScreens.Remove(screen);
        }

        public override void Dispose()
        {
            while(_openedScreens.Count > 0)
                CloseLast();
            
            foreach (var screen in _screens.Values)
                screen.Dispose();
            
            _screens.Clear();
        }
    }
}