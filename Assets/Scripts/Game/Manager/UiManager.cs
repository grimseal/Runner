using System;
using Game.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Manager
{
    public class UiManager : MonoBehaviourSingleton<UiManager>
    {
        public enum State
        {
            Main,
            Start,
            Game,
            Complete
        }
        
        public State state { get; private set; }
        
        [SerializeField] private GameObject gameHud;
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject completeMenu;
        [SerializeField] private GameObject tips;
        [SerializeField] private Text scoreTextHud;
        [SerializeField] private Text scoreTextTitle;
        [SerializeField] private GameObject eventSystem;

        protected override void Awake()
        {
            base.Awake();
            ResetCounters();
            SetState(State.Main);
        }

        public void ScoreChangeHandler(int value)
        {
            scoreTextHud.text = value.ToString();
            scoreTextTitle.text = value.ToString();
        }
        public void SetState(State uiState)
        {
            state = uiState;
            gameHud.SetActive(false);
            mainMenu.SetActive(false);
            completeMenu.SetActive(false);
            tips.SetActive(false);
            eventSystem.SetActive(false);
            switch (state)
            {
                case State.Main:
                    mainMenu.SetActive(true);
                    eventSystem.SetActive(true);
                    break;
                case State.Start:
                    tips.SetActive(true);
                    ResetCounters();
                    break;
                case State.Game:
                    gameHud.SetActive(true);
                    break;
                case State.Complete:
                    completeMenu.SetActive(true);
                    eventSystem.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void StartHandle()
        {
            GameManager.instance.StartNewLevel();
        }

        public void ExitHandle()
        {
            Application.Quit();
        }

        private void ResetCounters()
        {
            scoreTextHud.text = 0.ToString();
            scoreTextTitle.text = 0.ToString();
        }

    }
}
