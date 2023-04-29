using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

namespace Game.Runtime.Combat
{
    public class Enemy : MonoBehaviour
    {
        public enum State
        {
            NotActivated,
            Activating,
            Idle,
            Targeting,
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

        [Label("Animations")]
        [SerializeField] Animator anim;

        [Label("States")]
        [BeginGroup("Activation")]
        [SerializeField] bool activateOnAwake = true;
        [SerializeField] float activationLength;
        [EndGroup]
        [SerializeField] string activactionAnimationTrigger;

        [BeginGroup("Triggering")]
        [SerializeField][Layer] int playerLayer;
        [EndGroup]
        [SerializeField] float triggerRange;

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
            Gizmos.DrawWireSphere(transform.position, triggerRange);
        }

        private void FixedUpdate()
        {
            switch (EnemyState)
            {
                case State.NotActivated:
                    break;
                case State.Activating:
                    break;
                case State.Idle:
                    if (Physics.CheckSphere(transform.position, triggerRange, playerLayer))
                        TriggerTargeting();
                    break;
                case State.Targeting:
                    break;
            }

            GenerateDebugText();
        }

        public void Activate()
        {
            if (CanPlayAnimation(activactionAnimationTrigger))
                anim.SetTrigger(activactionAnimationTrigger);

            EnemyState = State.Idle;
        }

        public void TriggerTargeting()
        {
            EnemyState = State.Targeting;
        }

        bool CanPlayAnimation(string animName) =>
            anim != null && 
            anim.parameters
            .Select(x => x.name)
            .Contains(animName);

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
