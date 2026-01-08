namespace TcpIpSocketsLearn.Chapter4;

internal class MyThreadClass
{
    // Class that takes a String greeting as input, then outputs that
    // greeting to the console 10 times in its own thread with a random
    // interval between each greeting.

    private const int RANDOM_SLEEP_MAX = 500; // Max random milliseconds to sleep
    private const int LOOP_COUNT       = 10;  // Number of times to print message

    private readonly string _Greeting; // Message to print to console

    public MyThreadClass(string greeting)
    {
        _Greeting = greeting;
    }

    public void RunMyThread()
    {
        var rand = new Random();

        for (int x = 0; x < LOOP_COUNT; x++)
        {
            Console.WriteLine("Thread-" + Thread.CurrentThread.GetHashCode() + ": " + _Greeting);
            try
            {
                // Sleep 0 to RANDOM_SLEEP_MAX milliseconds
                Thread.Sleep(rand.Next(RANDOM_SLEEP_MAX));
            }
            catch (ThreadInterruptedException)
            {
            } // Will not happen
        }
    }
}

internal static class ThreadExample
{
    public static void Example(string[] args)
    {
        var mtc1 = new MyThreadClass("Hello");
        new Thread(mtc1.RunMyThread).Start();

        var mtc2 = new MyThreadClass("Aloha");
        new Thread(mtc2.RunMyThread).Start();

        var mtc3 = new MyThreadClass("Ciao");
        new Thread(mtc3.RunMyThread).Start();
    }
}