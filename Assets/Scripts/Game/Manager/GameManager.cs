using System;
using Game.Base;
using Game.Character;
using Game.Config;
using Game.Helper;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Manager
{
    public class GameManager : MonoBehaviourSingletonPersistent<GameManager>
    {
        public GameConfig config;
 
        public LevelConfig[] levelConfig;

        private PlayerInput input;

        protected override void Awake()
        {
            base.Awake();
            input = GetComponent<PlayerInput>();
            if (input == null) input = gameObject.AddComponent<PlayerInput>();
            if (LevelManager.instance == null)
            {
                if (Camera.main == null) throw new Exception("Main camera required");
                LevelManager.CreateInstance(input, Camera.main.GetComponent<CameraController>());
            }
            LevelManager.instance.scoreChangeEvent.AddListener(UiManager.instance.ScoreChangeHandler);
            LevelManager.instance.completeEvent
                .AddListener(() => UiManager.instance.SetState(UiManager.State.Complete));
        }

        public void StartNewLevel()
        {
            LevelManager.instance.StartLevel(config, levelConfig[Random.Range(0, levelConfig.Length)]);
            UiManager.instance.SetState(UiManager.State.Start);
            
            // Press any key
            input.onPressAnyKeyEvent.AddListener(() =>
            {
                UiManager.instance.SetState(UiManager.State.Game);
                input.onPressAnyKeyEvent.RemoveAllListeners();
                LevelManager.instance.Run();
            });
        }
    }
}