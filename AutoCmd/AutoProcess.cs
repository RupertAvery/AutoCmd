using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace AutoCmd
{
    public class AutoProcess
    {
        private readonly List<IResponder> _responders;

        public string WorkingDirectory { get; set; }
        public string Arguments { get; set; }
        public string FileName { get; set; }


        public AutoProcess()
        {
            _responders = new List<IResponder>();
        }

        public void AddResponder(IResponder responder)
        {
            _responders.Add(responder);
        }

        public void Start()
        {
            Process process = null;
            var flag = new AutoResetEvent(false);

            try
            {
                process = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = WorkingDirectory,
                        Arguments = Arguments,
                        FileName = FileName,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    }
                };

                process.Start();

                var cancellationTokenSource = new CancellationTokenSource();

                foreach (var responder in _responders)
                {
                    responder.SetProcess(process);
                }

                new Thread(() =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            int current;

                            while ((current = process.StandardOutput.Read()) >= 0)
                            {
                                var curChar = (char) current;
                                Console.Write(curChar);

                                foreach (var responder in _responders)
                                {
                                    responder.Read(curChar);
                                }
                            }

                            // We use a semaphore here so that we can wait for the above loop to finish.
                            // The process might have exited but the output buffer may still have data

                            flag.Set();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }).Start();

                process.WaitForExit();

                //Console.WriteLine("Waiting for thread...");

                flag.WaitOne();

                //Console.WriteLine("Done");

                // Allow the thread to exit safely

                cancellationTokenSource.Cancel();

                process.Close();
            }
            finally
            {
                process?.Dispose();
            }

        }
    }
}
