using Olympus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using System;

namespace Olympus
{
    public class LogFileGenerator : MonoBehaviour
    {
        public enum LogLevel
        {
            Normal,
            Warning,
            Error,
        }
        public class LogData
        {
            public LogData(string msg, int time, LogLevel logLevel)
            {
                message = msg;
                timeStamp = time;
                level = logLevel;
            }

            public string message;
            public int timeStamp;
            public LogLevel level;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void Awake()
        {
            logPath = Application.dataPath + '\\' + "logs" + '\\';
            string fullPath;
            string timeStamp = DateTime.Today.Year + "_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;

            fullPath = logPath + logFileName + timeStamp + ".txt";

            LogUtil.Log(fullPath);

            if (Directory.Exists(logPath) == false)
            {
                Directory.CreateDirectory(logPath);
            }

            logFile = new StreamWriter(fullPath);

        }

        private void Update()
        {
            if (myLog != string.Empty)
            {
                logFile?.WriteLine(myLog);
            }
            if (stack != string.Empty)
            {
                logFile?.WriteLine(stack);
            }
            myLog = string.Empty;
            stack = string.Empty;
        }

        private void OnDestroy()
        {
            logFile?.Close();
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        private string output;
        private string stack;
        private string myLog = "";
        List<LogData> logs = new();
        static readonly LogData INVALID_LOG = new LogData(null, -1, LogLevel.Error);
        string logPath;
        string logFileName = "game_log_";
        System.IO.StreamWriter logFile;

        public string inputBuffer;

        LogData GetLog(int index)
        {
            if (index >= logs.Count && index < logs.Count)
            {
                return INVALID_LOG;
            }
            return logs[index];
        }
    }
}