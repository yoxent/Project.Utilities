using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectUtilities.InputSystem
{
    /// <summary>
    /// Thin wrapper over the New Input System, exposing common events.
    /// Assign an InputActionAsset and map actions by name.
    /// </summary>
    public class InputRouter : MonoBehaviour
    {
        [Header("Actions")]
        [SerializeField] private InputActionAsset _actionsAsset;
        [SerializeField] private string _moveActionName = "Move";
        [SerializeField] private string _lookActionName = "Look";
        [SerializeField] private string _confirmActionName = "Confirm";
        [SerializeField] private string _cancelActionName = "Cancel";

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _confirmAction;
        private InputAction _cancelAction;

        public event Action<Vector2> OnMove;
        public event Action<Vector2> OnLook;
        public event Action OnConfirm;
        public event Action OnCancel;

        private void OnEnable()
        {
            if (_actionsAsset == null)
            {
                Debug.LogWarning("InputRouter has no InputActionAsset assigned.");
                return;
            }

            _moveAction = _actionsAsset.FindAction(_moveActionName, true);
            _lookAction = _actionsAsset.FindAction(_lookActionName, true);
            _confirmAction = _actionsAsset.FindAction(_confirmActionName, true);
            _cancelAction = _actionsAsset.FindAction(_cancelActionName, true);

            _moveAction.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            _moveAction.canceled += ctx => OnMove?.Invoke(Vector2.zero);

            _lookAction.performed += ctx => OnLook?.Invoke(ctx.ReadValue<Vector2>());
            _lookAction.canceled += ctx => OnLook?.Invoke(Vector2.zero);

            _confirmAction.performed += _ => OnConfirm?.Invoke();
            _cancelAction.performed += _ => OnCancel?.Invoke();

            _actionsAsset.Enable();
        }

        private void OnDisable()
        {
            if (_actionsAsset != null)
            {
                _actionsAsset.Disable();
            }
        }
    }
}

