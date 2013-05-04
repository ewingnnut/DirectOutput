﻿using System;
using System.IO;

namespace DirectOutput
{
    /// <summary>
    /// A simple logger used to record important events and exceptions.<br/>
    /// </summary>
    public class Log
    {
        static StreamWriter Logger;
        static bool IsInitialized = false;
        static bool IsOk = false;

        private static object Locker = new object();

        private static string _Filename=".\\DirectOutput.log";

        /// <summary>
        /// Gets or sets the filename for the log.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public static string Filename
        {
            get { return _Filename; }
            set { _Filename = value; }
        }
        

        /// <summary>
        /// Initializes the log using the file defnied in the Filename property.
        /// </summary>
        public static void Init()
        {
            lock (Locker)
            {
                if (!IsInitialized)
                {
                    try
                    {
                        Logger = File.AppendText(Filename);

                        Logger.WriteLine("{0}\t{1}", DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss.fff"), "DirectOutput Logger initialized");


                        IsOk = true;

                    }
                    catch
                    {
                        IsOk = false;
                    }

                    IsInitialized = true;
                }
            }
        }

        public void Finish()
        {
            lock (Locker)
            {
                if (Logger != null)
                {
                    Write("Logging stopped");
                    Logger.Flush();
                    Logger.Close();
                    IsOk = false;
                    IsInitialized = false;
                    Logger = null;
                }
            }
        }


        /// <summary>
        /// Writes the specified message to the logfile.
        /// </summary>
        /// <param name="Message">The message.</param>
        public static void Write(string Message)
        {
            
            lock (Locker)
            {
                if (IsOk)
                {
                    if (Message.IsNullOrWhiteSpace())
                    {
                        Logger.WriteLine("{0}\t{1}", DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss.fff"), "");
                    }
                    else
                    {
                        foreach (string M in Message.Split(new[] { '\r', '\n' }))
                        {
                            Logger.WriteLine("{0}\t{1}", DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss.fff"), M);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a warning message to the log.
        /// </summary>
        /// <param name="Message">The message.</param>
        public static void Warning(string Message)
        {
            Write("Warning: {0}".Build(Message));
        }

        /// <summary>
        /// Writes a exception message to the log.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="E">The Exception to be logged.</param>
        public static void Exception(string Message = "", Exception E = null)
        {
            lock (Locker)
            {
                if (!Message.IsNullOrWhiteSpace())
                {
                    Write("EXCEPTION: {0}".Build(Message));
                }
                if (E != null)
                {
                    Write("EXCEPTION: {0}".Build(E.Message));

                    int Level = 1;
                    while (E.InnerException != null)
                    {
                        E = E.InnerException;
                        Write("EXCEPTION: InnerException {0}: {1}".Build(Level, E.Message));
                        Level++;

                        if (Level > 20)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes a exception to the log.
        /// </summary>
        /// <param name="E">The Exception to be logged.</param>
        public static void Exception(Exception E = null)
        {
            Exception("",E);
        }

    }
}
