using System;
using System.Diagnostics;

namespace AutoCmd
{
    public class BufferedAutoResponder : IResponder
    {
        private Process _process;
        private readonly int _bufferSize;
        private readonly char[] _buffer;
        private bool _isActive;
        private int _ptr;

        public string Match { get; set; }
        public Action<Process> Respond { get; set; }

        public BufferedAutoResponder(int bufferSize)
        {
            _bufferSize = bufferSize;
            _buffer = new char[bufferSize];
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
                    ClearBuffer();
                    _ptr = 0;
                }
            }

            if (_isActive)
            {
                _buffer[_ptr] = character;
                _ptr++;

                if (_ptr == Match.Length)
                {
                    // see if the buffer matches the prompt
                    var comp = new string(_buffer);
                    comp = comp.Substring(0, comp.IndexOf('\0'));
                    if (comp == Match)
                    {
                        Respond(_process);
                        _isActive = false;
                        ClearBuffer();
                        _ptr = 0;
                    }
                }

                if (_ptr >= _bufferSize)
                {
                    _isActive = false;
                    ClearBuffer();
                    _ptr = 0;
                }
            }
        }

        public void ClearBuffer()
        {
            // alternative
            // Array.Fill(_buffer, '\0');

            for (var i = 0; i < 10; i++)
            {
                _buffer[i] = '\0';
            }
        }
    }
}