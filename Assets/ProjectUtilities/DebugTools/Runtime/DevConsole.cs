using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace ProjectUtilities.DebugTools.Runtime
{
    /// <summary>
    /// Minimal in-game dev console. Intended for development builds.
    /// </summary>
    public class DevConsole : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _outputText;

        private readonly Dictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>(StringComparer.OrdinalIgnoreCase);

        private void Awake()
        {
            if (_root != null)
            {
                _root.SetActive(false);
            }
        }

        public void Toggle()
        {
            if (_root == null)
            {
                return;
            }

            var isActive = !_root.activeSelf;
            _root.SetActive(isActive);

            if (isActive && _inputField != null)
            {
                _inputField.text = string.Empty;
                _inputField.ActivateInputField();
            }
        }

        public void RegisterCommand(string name, Action<string[]> handler)
        {
            if (string.IsNullOrEmpty(name) || handler == null)
            {
                return;
            }

            _commands[name] = handler;
        }

        public void SubmitCommand()
        {
            if (_inputField == null)
            {
                return;
            }

            var text = _inputField.text;
            _inputField.text = string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            AppendOutput($"> {text}");

            var parts = text.Split(' ');
            var command = parts[0];
            var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

            if (_commands.TryGetValue(command, out var handler))
            {
                try
                {
                    handler(args);
                }
                catch (Exception ex)
                {
                    AppendOutput($"Error: {ex.Message}");
                }
            }
            else
            {
                AppendOutput("Unknown command.");
            }
        }

        private void AppendOutput(string line)
        {
            if (_outputText == null)
            {
                return;
            }

            var sb = new StringBuilder(_outputText.text.Length + line.Length + 2);
            if (!string.IsNullOrEmpty(_outputText.text))
            {
                sb.AppendLine(_outputText.text);
            }

            sb.Append(line);
            _outputText.text = sb.ToString();
        }
    }
}

