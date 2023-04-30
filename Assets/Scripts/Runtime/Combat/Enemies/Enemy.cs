using UnityEngine;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Events;

namespace Game.Runtime.Combat.Enemies
{
    public class Enemy : MonoBehaviour
    {
        public enum State
        {
            NotActivated,
            Idle,
            TargetDetected,
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

        public TargetBox Target { get; private set; }

        [Label("States")]
        [BeginGroup("Activation")]
        [SerializeField] bool activateOnAwake = true;
        [SerializeField] float activationLength;
        [EndGroup]
        public UnityEvent OnActivate;

        [BeginGroup("Detecting Target")]
        [SerializeField] LayerMask targetBoxLayer;
        [SerializeField] float detectRange;
        [EndGroup]
        public UnityEvent OnDetectPlayer;

        [BeginGroup("Losing Target")]
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
                    var target = FindTarget();

                    if (target != null)
                        DetectTarget(target);
                    break;
                case State.TargetDetected:
                    if (!Physics.SphereCastAll(transform.position, loseRange, Vector3.up, 0f, targetBoxLayer)
                        .Select(x => x.transform)
                        .Contains(Target.transform))
                    {
                        Target = FindTarget();
                        if (Target == null)
                            LoseTarget();
                    }
                    break;
            }

            GenerateDebugText();
        }

        TargetBox FindTarget()
        {
            var targets = Physics.SphereCastAll(transform.position, detectRange, Vector3.up, 0f, targetBoxLayer)
                .Select(x => x.collider.GetComponent<TargetBox>())
                .Where(x => x.team == TargetBox.Team.Friendly)
                .ToList();

            return targets.FirstOrDefault();
        }

        public void Activate()
        {
            EnemyState = State.Idle;
            OnActivate.Invoke();
        }

        public void DetectTarget(TargetBox target)
        {
            Target = target;
            EnemyState = State.TargetDetected;
            OnDetectPlayer.Invoke();
        }

        public void LoseTarget()
        {
            Target = null;
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
