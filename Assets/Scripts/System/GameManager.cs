using System.Collections.Generic;
using Entity;
using Entity.Enemies;
using Terrain;
using Tools;
using UnityEditor;
using UnityEngine;

namespace System
{
    public class GameManager : Singleton<GameManager>
    {
        public CenterCircle centerCircle;
        public Player player;
        public CameraController cameraController;
        public List<BaseMonster> monsters = new List<BaseMonster>();
        public AudioSource audioSource;
        public AudioClip bgmClip;
        public AudioClip scoreClip;
        private float score = 0.0f;

        public float Score
        {
            get => score;
            set
            {
                AudioSource.PlayClipAtPoint(scoreClip, Instance.transform.position);
                score = value;
            }
        }
        
        public float summonInterval = 1.0f;
        private float nowTime = 0.0f;

        // 游戏状态
        private bool _isGamePaused = false;
        public bool IsGamePaused
        {
            get => _isGamePaused;
            private set
            {
                _isGamePaused = value;
                Time.timeScale = _isGamePaused ? 0 : 1;
                OnGamePauseStateChanged?.Invoke(_isGamePaused);
            }
        }

        // 事件系统
        public delegate void GameStateChangeHandler(bool isPaused);
        public event GameStateChangeHandler OnGamePauseStateChanged;

        protected override void Awake()
        {
            base.Awake();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // 初始化游戏组件
            if (centerCircle == null)
                Debug.LogError("CenterCircle reference not set in GameManager!");
            if (player == null)
                Debug.LogError("Player reference not set in GameManager!");
            if (cameraController == null)
                Debug.LogError("CameraController reference not set in GameManager!");
            if (audioSource == null)
                Debug.Log("No Audio Source");

            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = 0.2f;
            audioSource.Play();
            player.gameObject.SetActive(false);
            monsters = new List<BaseMonster>(FindObjectsByType<BaseMonster>(FindObjectsSortMode.InstanceID));
            Time.timeScale = 0.0f;
            score = 0f;
            PauseGame();
        }

        public void PauseGame()
        {
            IsGamePaused = true;
            Time.timeScale = 0.0f;
        }

        public void ResumeGame()
        {
            IsGamePaused = false;
            Time.timeScale = 1.0f;
        }

        public void TogglePause()
        {
            IsGamePaused = !IsGamePaused;
        }

        public void RestartGame()
        {
            Time.timeScale = 1.0f;
            score = 0.0f;
            if (player != null)
            {
                player.gameObject.SetActive(true);
                player.ResetPlayer();
                MonsterPool.Instance.ReturnAllMonsters();
                monsters.Clear();
            }
            ResumeGame();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

            if (nowTime <= summonInterval) nowTime += Time.deltaTime;
            else
            {
                nowTime = 0.0f;
                var monster = MonsterPool.Instance.GetRandomMonster();

                if (monster.TryGetComponent<BaseMonster>(out var component))
                {
                    component.Reset();
                    var middleAngle = player.CurrentAngle - Mathf.PI;
                    var rad = GetRandomRadianInSemiCircle(middleAngle);
                    switch (component)
                    {
                        case DashMonster dashMonster:
                        {
                            var pos = GetRandomVectorInSemicircle(middleAngle) * centerCircle.higherRadius;
                            dashMonster.SetOrigin(pos);
                            break;
                        }
                        case BombMonster bombMonster:
                            bombMonster.SetNowAngle(rad);
                            break;
                        case ShieldMonster shieldMonster:
                            shieldMonster.SetNowAngle(rad);
                            break;
                    }
                    monsters.Add(component);
                }
            }
        }

        public static float GetRandomRadianInSemiCircle(float centerRadian, float spreadAngle = Mathf.PI)
        {
            spreadAngle = Mathf.Min(spreadAngle, Mathf.PI);
        
            float halfSpread = spreadAngle * 0.5f;
            float randomRadian = centerRadian + UnityEngine.Random.Range(-halfSpread, halfSpread);

            return randomRadian;
        }
        public static Vector3 GetRandomVectorInSemicircle(float centerRadian, float spreadAngle = Mathf.PI)
        {
            var randomRadian = GetRandomRadianInSemiCircle(centerRadian, spreadAngle);
            return new Vector3(
                Mathf.Cos(randomRadian),
                Mathf.Sin(randomRadian),
                0
            ).normalized;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}