using System.Diagnostics;

namespace AutoCmd
{
    public interface IResponder
    {
        void SetProcess(Process process);
        void Read(char character);
    }
}