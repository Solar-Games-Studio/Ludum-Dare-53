using UnityEngine;

namespace Game.Runtime.Combat.Enemies
{
    public class EnemyMove : MonoBehaviour
    {
        [Label("Assign")]
        [SerializeField] CharacterController characterController;
        [SerializeField] Enemy enemyBrain;

        [Label("Distances")]
        [SerializeField] float escapeDistance = 4f;
        [SerializeField] float minDistance = 10f;
        [SerializeField] bool keepInMaxDistance = true;
        [SerializeField] float maxDistance = 40f;

        [Label("Movement")]
        [SerializeField] float speed = 3f;
        [SerializeField] float minDirectionChangeTime = 7f;
        [SerializeField] float maxDirectionChangeTime = 10f;

        float _time;
        bool _reverseDirection;

        private void Awake()
        {
            ResetTimer();
        }

        private void Update()
        {
            if (enemyBrain.Target == null) return;

            _time -= Time.deltaTime;

            if (_time < 0f)
            {
                ResetTimer();
                _reverseDirection = !_reverseDirection;
            }

            var trans = characterController.transform;

            var currentDifference = trans.position - enemyBrain.Target.transform.position - enemyBrain.Target.center;
            currentDifference.y = 0f;

            var path = (Vector3.ClampMagnitude(currentDifference + trans.right * (_reverseDirection ? -1f : 1f) * Time.deltaTime, currentDifference.magnitude) - currentDifference).normalized;

            if (currentDifference.magnitude < minDistance)
                path = (path - trans.forward).normalized;

            if (currentDifference.magnitude > maxDistance && keepInMaxDistance)
                path = (path + trans.forward).normalized;

            if (currentDifference.magnitude < escapeDistance)
                path = currentDifference.normalized;

            path *= speed;

            characterController.Move(path * Time.deltaTime);
            characterController.Move(Vector3.zero);
        }

        void ResetTimer()
        {
            _time = Random.Range(minDirectionChangeTime, maxDirectionChangeTime);
        }
    }
}
