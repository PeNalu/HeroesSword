using System.Collections.Generic;
using UnityEngine;

public class Logger : Singleton<Logger>
{
    [SerializeField]
    private LogLine template;

    [SerializeField]
    private int logCount = 10;

    Queue<LogLine> queue = new Queue<LogLine>(); 

    public void Log(string text)
    {
        LogLine line = Instantiate(template, transform);
        line.Initialize(text);
        queue.Enqueue(line);

        if(queue.Count > logCount)
        {
            LogLine logLine = queue.Dequeue();
            Destroy(logLine.gameObject);
        }
    }
}
