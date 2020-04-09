using Game.Base;
using Game.Character;
using Game.Config;
using Game.Helper;
using Game.Level;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Manager
{
    public class LevelManager : MonoBehaviourSingleton<LevelManager>
    {
        public UnityEvent completeEvent { get; private set; }
        public ScoreChange scoreChangeEvent { get; private set; }

        private PlayerInput playerInput;
        private CameraController cameraController;
        private bool isRun;
        private float startTime;
        private float speed;
        private float speedMultiplier;
        private float speedLimit;
        private CharacterControllerComponent characterController;
        private LevelScroller levelScroller;

        public static void CreateInstance(PlayerInput input, CameraController cameraController)
        {
            if (instance != null) return;
            var manager = new GameObject {name = "Level Manager"}.AddComponent<LevelManager>();
            manager.playerInput = input;
            manager.completeEvent = new UnityEvent();
            manager.scoreChangeEvent = new ScoreChange();
            manager.cameraController = cameraController;
        }

        public void StartLevel(GameConfig config, LevelConfig levelConfig)
        {
            isRun = false;
            speed = config.startSpeed;
            speedLimit = config.speedLimit;
            speedMultiplier = config.speedMultiplier;

            SetupCharacter(config.character);
            SetupCamera(characterController.transform);
            SetupLevelController(config, levelConfig);

            levelScroller.StartLevel();
        }

        public void Run()
        {
            isRun = true;
            startTime = Time.time;
        }

        private void Update()
        {
            if (!isRun) return;
            speed = Mathf.Min(speed + (Time.time - startTime) * speedMultiplier, speedLimit);
            characterController.speed = speed;
            levelScroller.Update(characterController.position);
        }

        private void FixedUpdate()
        {
            if (!isRun) return;
            // Shift the level when the character passes one chunk length
            if (characterController.position.z < levelScroller.chunkLength) return;
            var shift = new Vector3(0, 0, -levelScroller.chunkLength);
            levelScroller.OriginOffset(shift);
            characterController.position += shift;
        }

        private void SetupCharacter(GameObject characterPrefab)
        {
            if (characterController != null)
            {
                characterController.UnsubscribeFromInput(playerInput);
                characterController.GetComponent<Wallet>().onAmountChange.RemoveAllListeners();
                characterController.onFailEvent.RemoveAllListeners();
                Destroy(characterController.gameObject);
            }
            characterController = Instantiate(characterPrefab).GetComponent<CharacterControllerComponent>();
            characterController.speed = 0;
            characterController.onFailEvent.AddListener(OnCharacterOnFailEvent);
            characterController.GetComponent<Wallet>().onAmountChange.AddListener(scoreChangeEvent.Invoke);
            characterController.SubscribeToInput(playerInput);
        }

        private void SetupCamera(Transform target)
        {
            cameraController.Detach();
            cameraController.Reset();
            cameraController.AttachTo(target);
        }

        private void SetupLevelController(GameConfig config, LevelConfig levelConfig)
        {
            levelScroller?.Destroy();
            var container = transform;
            var pool = new LevelObjectsPool(levelConfig, 2, container);
            var generator = new ChunkGenerator(levelConfig, pool, config.chunkGraphLength, config.chunkGraphWidth);
            levelScroller = new LevelScroller(generator, container);
        }

        private void OnCharacterOnFailEvent()
        {
            isRun = false;
            cameraController.Detach();
            completeEvent.Invoke();
        }

        public class ScoreChange : UnityEvent<int> {}

    }
}