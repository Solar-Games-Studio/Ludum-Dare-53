using qASIC;
using UnityEngine;

namespace Game.Runtime.Sprites
{
    public class Sprite3DController : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite[] sprites;
        public Transform trans;
        public Transform transToCompare;
        public float rotationOffset;


        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            trans = transform;
            transToCompare = transform.parent;
        }

        private void Update()
        {
            float angle = Vector3.Angle(trans.forward, transToCompare.forward) *
                (Vector3.Cross(trans.forward, transToCompare.forward).y >= 0 ? 1f : -1f) +
                rotationOffset;

            angle = NormalizeAngle(angle);

            if (spriteRenderer == null || sprites.Length == 0)
                return;

            int spriteIndex = Mathf.FloorToInt(angle / 360f * sprites.Length);

            spriteRenderer.sprite = sprites[spriteIndex];
        }

        float NormalizeAngle(float angle)
        {
            while (angle < 0f)
                angle += 360f;

            while (angle > 360f)
                angle -= 360f;

            return angle;
        }
    }
}
