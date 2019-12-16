using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace ThreadConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //SimpleThread();
            //SleepThread();
            //LockThread();
            //PassingDatgaThread();
            //SimpleThreadPool();
            //ThreadVSThreadPool();
            MultiThreading(args);
        }

        #region SimpleThread

        static void SimpleThread()
        {
            // Create a thread and start it
            Thread t = new Thread(Print1);
            t.Start();

            // This code will run randomly
            for (int i = 0; i < 1000; i++)
                Console.Write("M");

            Console.ReadLine();
        }

        static void Print1()
        {
            for (int i = 0; i < 1000; i++)
                Console.Write("T");
        }

        #endregion

        #region Sleep Example

        // With sleep() the thread is suspended for the designated amount of time
        static void SleepThread()
        {
            int num = 1;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(num);

                // Pause for 1 second
                Thread.Sleep(1000);

                num++;
            }

            Console.WriteLine("Thread Ends");
            Console.ReadLine();
        }

        #endregion

        #region Lock Example
        // lock keeps other threads form entering a statement block until another thread leaves
        static void LockThread()
        {
            BankAccount account = new BankAccount(10);
            Thread[] threads = new Thread[15];

            // CurrentThread gets you the current exeuting thread
            Thread.CurrentThread.Name = "main";

            // Create 15 threads that will call for IssueWithdraw to exercise
            for (int i = 0; i < 15; i ++)
            {
                // You can only point at methods without arguments and that return nothing
                Thread t = new Thread(new ThreadStart(account.IssueWithdraw));
                t.Name = i.ToString();
                threads[i] = t;
            }

            // Have threads try to exercise
            for (int i = 0; i < 15; i++)
            {
                // Check if thread has started
                Console.WriteLine("Thread {0} Alive : {1}", threads[i].Name, threads[i].IsAlive);

                // Start thread
                threads[i].Start();

                // Check if thread has started
                Console.WriteLine("Thread {0} Alive : {1}", threads[i].Name, threads[i].IsAlive);

                // Get thread priority (Normal Default)
                // Also lowest, BelowNormal, AboveNormal, and Highest Changin priority doesn't guarantee
                // the highest precedence though
                // It is best to not mess with this
                Console.WriteLine("Current Priority : {0}", Thread.CurrentThread.Priority);
                Console.WriteLine("Thread {0} Ending", Thread.CurrentThread.Name);
                Console.ReadLine();
            }

        }

        #endregion

        #region Passing Date to Threads
        
        static void PassingDatgaThread()
        {
            Thread t = new Thread(() => CountTo(10));
            t.Start();

            new Thread(() =>
            {
                CountTo(5);
                CountTo(6);
            }).Start();

            Console.ReadLine();
        }

        static void CountTo(int maxNum)
        {
            for(int i = 0; i <= maxNum; i++)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine("End CountTo");
        }

        #endregion

        #region SimpleThreadPool

        static void SimpleThreadPool()
        {
            // Queue the task.
            ThreadPool.QueueUserWorkItem(ThreadProc);
            Console.WriteLine("Main thread does some work, then sleeps.");
            Thread.Sleep(1000);

            Console.WriteLine("Main thread exits.");
            Console.ReadLine();
        }

        // This thread procedure performs the task
        static void ThreadProc(Object stateInfo)
        {
            // No state object was passed to QueueUserWorkItem, so stateInfo is null.
            Console.WriteLine("Hello form thread pool.");
        }

        #endregion

        #region Thread VS ThreadPool

        static void ThreadVSThreadPool()
        {
            Stopwatch watch = new Stopwatch();

            Console.WriteLine("Thread Pool Execution");

            watch.Start();
            ProcessWithThreadPoolMethod();
            watch.Stop();

            Console.WriteLine("Time consumed by ProcessWithThreadPoolMethod is : " + watch.ElapsedTicks.ToString());
            watch.Reset();

            Console.WriteLine("Thread Execution");

            watch.Start();
            ProcessWithThreadMethod();
            watch.Stop();

            Console.WriteLine("Time consumed by ProcessWithThreadMethod is : " + watch.ElapsedTicks.ToString());
            Console.ReadLine();
        }

        static void ProcessWithThreadPoolMethod()
        {
            for (int i = 0; i <= 10; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Process));
            }
        }

        static void ProcessWithThreadMethod()
        {
            for (int i = 0; i <= 10; i++)
            {
                Thread thread = new Thread(Process);
                thread.Start();
            }
        }

        static void Process(object callback)
        {

        }

        #endregion

        #region MultiThreading

        static void MultiThreading(string[] args)
        {
            // custom thread parameter object
            ThreadParams tParams = new ThreadParams();

            // get the file name from command prompt - user input
            tParams.sourceFileName = args[0];
            tParams.targetFileName = args[1];

            // queue the file copy thread for execution
            ThreadPool.QueueUserWorkItem(new WaitCallback(copyFileThreadPool), tParams);

            // we do not allow the main thread to terminate because it
            // will also terminate the file copy thread
            Console.ReadKey();
        }

        // Object used for passing parameters to the CopyFileThreadPool method
        class ThreadParams
        {
            public string sourceFileName { get; set; }
            public string targetFileName { get; set; }
        }

        private static void copyFileThreadPool(object tParams)
        {
            // read the custom thread parameter object
            ThreadParams tParamsInner = tParams as ThreadParams;
            string sourceFileName = @tParamsInner.sourceFileName;
            string targetFileName = @tParamsInner.targetFileName;

            Console.WriteLine();
            Console.WriteLine("File Copy Operation using the ThreadPool Class");
            Console.WriteLine();
            Console.WriteLine("File to copy: " + @sourceFileName);

            // count file lines
            StreamReader sr = new StreamReader(@sourceFileName);
            float totalLines = File.ReadLines(@sourceFileName).Count();
            sr.Close();

            string line = "";
            float progress = 0;

            Console.WriteLine();
            Console.WriteLine("Copying file ...");

            int count = 0;

            // start reading the file and copying it to the second file
            using (StreamReader reader = new StreamReader(@sourceFileName))
            {
                StreamWriter writer = new StreamWriter(@targetFileName);

                while ((line = reader.ReadLine()) != null)
                {
                    ++count;

                    // avoid adding blank line in the end fo file
                    if (count != totalLines)
                        writer.WriteLine(line);
                    else
                        writer.Write(line);

                    // progress report
                    progress = (count / totalLines) * 100;
                    Console.WriteLine("Progress: " + Math.Round(progress, 2).ToString() + "% (rows copied: " + count.ToString() + ")");
                }

                writer.Close();
            }

            Console.WriteLine();
            Console.WriteLine("Original File: " + @sourceFileName);
            Console.WriteLine("New File: " + @targetFileName);
            Console.WriteLine();
            Console.WriteLine("press any key to exit....");
        }

        #endregion


    }
}


class BankAccount
{

    private Object accountLock = new Object();
    double Balance { get; set; }
    string Name { get; set; }

    public BankAccount(double balance)
    {
        Balance = balance;
    }

    public double Withdraw(double amount)
    {
        if ((Balance - amount) < 0)
        {
            Console.WriteLine($"Sorry ${Balance} in Account");
            return Balance;
        }
        
        
        lock (accountLock)
        {
            if (Balance >= amount)
            {
                Console.WriteLine("Removed {0} and {1} left in Account", amount, (Balance - amount));
                Balance -= amount;
            }

            return Balance;

        }
    }

    // You can only point at methodds without arguments and that return nothing
    public void IssueWithdraw()
    {
        Withdraw(1);
    }

}