using System.Collections;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static Logger Instance;

    private FileStream stream;

    private void OnEnable()
    {
        Globals.OnLogsUpdated += WriteToLog;
    }

    private void OnDisable()
    {
        Globals.OnLogsUpdated -= WriteToLog;
        if(stream == null) { return; }
        stream.Flush();
        stream.Close();
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
        }
    }

    public IEnumerator FindOrCreateLog()
    {
        string date = System.DateTime.Today.ToShortDateString();
        bool fileExists = File.Exists(Application.dataPath + @"/Logs/" + date + ".txt");
        if (!fileExists)
        {
            print("File does not exist. Creating...");
            stream = File.Create(Application.dataPath + @"/Logs/" + date + ".txt");
            yield break;
        }

        print("File exists. Opening...");
        stream = File.Open(Application.dataPath + @"/Logs/" + date + ".txt", FileMode.Open);
        yield return null;
    }

    public IEnumerator WriteToLog(string text)
    {
        if(stream != null)
        {
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(text);
            sw.Flush();
            sw.Close();
        }
        yield return null;
    }

}
