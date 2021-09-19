using System;
using AutoCmd;

namespace Test
{
  
    class Program
    {

        static void Main(string[] args)
        {
            var auto = new AutoProcess()
            {
                FileName = "getcred.cmd"
            };

            var usernameResponder = new AutoResponder()
            {
                Match = "Username:",
                Respond = (proc) =>
                {
                    proc.StandardInput.WriteLine("ADMIN");
                    // Because we're not typing to the screen, we need to insert a newline as if we pressed enter
                    // This doesn't affect the response, it only makes it look "normal" on screen
                    Console.WriteLine();
                }
            };

            var passwordResponder = new AutoResponder()
            {
                Match = "Password:",
                Respond = (proc) =>
                {
                    proc.StandardInput.WriteLine("12345!");
                    Console.WriteLine();
                }
            };

            var pressAnyKeyResponder = new AutoResponder()
            {
                Match = "Press any key to continue . . .",
                Respond = (proc) =>
                {
                    proc.StandardInput.WriteLine();
                    Console.WriteLine();
                }
            };

            
            auto.AddResponder(usernameResponder);
            auto.AddResponder(passwordResponder);
            auto.AddResponder(pressAnyKeyResponder);

            auto.Start();
            
            Console.ReadLine();
        }
    }
}
