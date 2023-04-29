using UnityEngine;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Events;

namespace Game.Runtime.Combat
{
    public class Enemy : MonoBehaviour
    {
        public enum State
        {
            NotActivated,
            Idle,
            PlayerDetected,
        }

        public State EnemyState { get; private set; } = State.NotActivated;

        private static bool _showDebug = false;
        private static Action _onChangeShowDebug;
        public static bool ShowDebug
        {
            get => _showDebug;
            set
            {
                _showDebug = value;
                _onChangeShowDebug?.Invoke();
            }
        }

        [Label("States")]
        [BeginGroup("Activation")]
        [SerializeField] bool activateOnAwake = true;
        [SerializeField] float activationLength;
        [EndGroup]
        public UnityEvent OnActivate;

        [BeginGroup("Detecting Player")]
        [SerializeField][Layer] int playerLayer;
        [SerializeField] float detectRange;
        [EndGroup]
        public UnityEvent OnDetectPlayer;

        [BeginGroup("Losing Player")]
        [SerializeField] float loseRange;
        [EndGroup]
        public UnityEvent OnLosePlayer;

        [Label("Debug")]
        [EditorButton(nameof(Activate))]
        [SerializeField] TMP_Text debugText;

        private void Awake()
        {
            RefreshDebugTextState();
            _onChangeShowDebug += RefreshDebugTextState;

            if (activateOnAwake)
                Activate();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, loseRange);
        }

        private void FixedUpdate()
        {
            switch (EnemyState)
            {
                case State.NotActivated:
                    break;
                case State.Idle:
                    if (Physics.CheckSphere(transform.position, detectRange, playerLayer))
                        DetectPlayer();
                    break;
                case State.PlayerDetected:
                    if (!Physics.CheckSphere(transform.position, loseRange, playerLayer))
                        LosePlayer();
                    break;
            }

            GenerateDebugText();
        }

        public void Activate()
        {
            EnemyState = State.Idle;
            OnActivate.Invoke();
        }

        public void DetectPlayer()
        {
            EnemyState = State.PlayerDetected;
            OnDetectPlayer.Invoke();
        }

        public void LosePlayer()
        {
            EnemyState = State.Idle;
            OnLosePlayer.Invoke();
        }

        void RefreshDebugTextState()
        {
            if (debugText != null)
                debugText.gameObject.SetActive(ShowDebug);
        }

        void GenerateDebugText()
        {
            if (debugText == null)
                return;

            debugText.text = $"State: {EnemyState}\n";
        }
    }
}
