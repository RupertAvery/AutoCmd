# AutoCmd

AutoCmd is a simple library to monitor the standard output of a console application and write to standard input when a key word is detected.

The use case this was made for was the automation of entry of credentials into a console application.

# Usage

Create an `AutoProcess` instance, and set the `FileName` of the command line executable. You can also set the `Arguments` and `WorkingDirectory` as required. These are passed directly to the `Process` object later.

```cs
var auto = new AutoProcess()
{
    FileName = "getcred.cmd"
};
```

Create one or more `AutoResponder`s.  Set the `Match` property to the exact string to match when a response will occur.  Set the `Respond` property to a delegate or anonymous function that accepts a `Process` instance. Within the response method, you can call `process.StandardInput.WriteLine` to send text

```cs
var usernameResponder = new AutoResponder()
{
    Match = "Username:",
    Respond = (proc) =>
    {
        proc.StandardInput.WriteLine("ADMIN");
        // Because we're not typing to the screen, we need to insert a newline as if we pressed enter
        // This is not actually needed and doesn't affect the response, it only makes it look "normal" on screen
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
```

Add them to the `AutoProcess` with `AddResponder`, and finally call `Start()` on the `AutoProcess`.

```cs
auto.AddResponder(usernameResponder);
auto.AddResponder(passwordResponder);
auto.AddResponder(pressAnyKeyResponder);
auto.Start();            
```

This will launch the process and begin tracking the output. 

# How it works

A Process is created with StandardInput and StandardOuput redirected and the process is started.  A thread is created to read the StandardOutput stream character by character. Note that this may cause some latency in the output.

Each read character is fed into the responders with the `Read(char character)` method, which starts matching characters when the first character in the `Match` property is matched.  This is just a simple match that advances the character pointer each time a match is made, and resets it on failure.

If the match completes, the `Respond` method is called with the process instance as the argument.

# Notes

The library calls `WaitForExit()` on the process to wait until it completes. Due to the latency in output, it's possible that the process may complete while the thread is still busy reading characters and outputting to screen. To prevent the process being closed prematurely, an `AutoResetEvent` is used to synchronize the completion of a `StandardInput.Read()` to empty the buffer with the completion of `WaitForExit()`. This hasn't really been tested extensively.

This library cannot read a dynamically updated console output, such as progress information updated on the same line, or the `TIMEOUT` command. 

This was put together for a specific, simple use case and may not exactly fit your needs.

# Extending AutoCmd

You can create your own Responder by implementing `IResponder`.

Adding stuff like regex might be a bit more complicated as regex doesn't work on a character level. I've thought about it and you might need to wait for the output to stop by checking how long before `Read` was called before doing anything. This is because matching something that "starts with" a pattern would require the output to "complete" before matching.

# Other libraries

I looked at some other libraries but they did not seem to be able to do exactly what was needed.

* https://github.com/Tyrrrz/CliWrap
