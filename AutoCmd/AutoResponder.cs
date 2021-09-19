using System;
using System.Diagnostics;

namespace AutoCmd
{
    public class AutoResponder : IResponder
    {
        private Process _process;
        private bool _isActive;
        private int _ptr;

        public string Match { get; set; }
        public Action<Process> Respond { get; set; }

        public AutoResponder()
        {
        }

        public void SetProcess(Process process)
        {
            _process = process;
        }

        public void Read(char character)
        {
            if (!_isActive)
            {
                if (character == Match[0])
                {
                    _isActive = true;
                    _ptr = 0;
                }
            }

            if (_isActive)
            {
                if (character == Match[_ptr])
                {
                    _ptr++;
                    // We've matched all the characters, time to respond
                    if (_ptr == Match.Length)
                    {
                        Respond(_process);
                        Reset();
                    }
                }
                else
                {
                    // the character did not match, reset
                    Reset();
                }
            }
        }

        private void Reset()
        {
            _isActive = false;
            _ptr = 0;
        }

    }
}