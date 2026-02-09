using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectUtilities.Parallax.Core
{
    /// <summary>
    /// Simple camera mover for testing parallax: hold A/D to move left/right on X.
    /// Attach to the camera you want to move.
    /// </summary>
    public class CameraKeyboardMover : MonoBehaviour
    {
        [Tooltip("Units per second the camera moves when holding A/D.")]
        [SerializeField] private float _speed = 5f;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            float direction = 0f;

            if (keyboard.aKey.isPressed)
            {
                direction -= 1f;
            }

            if (keyboard.dKey.isPressed)
            {
                direction += 1f;
            }

            if (Mathf.Approximately(direction, 0f))
            {
                return;
            }

            var pos = transform.position;
            pos.x += direction * _speed * Time.deltaTime;
            transform.position = pos;
        }
    }
}

