/*
	PARAMETERS Remake
    Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.IO;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Logger : MonoBehaviour
{
    #if UNITY_EDITOR
    public TextAsset logFile;
    public TextAsset logTemplate;

    StreamWriter writer;
    int entryNumber = 0;

    void OnEnable()
    {
        EventManager.StartListening("Log", LogMessage);
    }

    void OnDisable()
    {
        EventManager.StopListening("Log", LogMessage);
    }

    void Start()
	{
        string path;
        string pathToTemplate;

        if (logFile != null)
        {
            path = AssetDatabase.GetAssetPath(logFile);
        }
        else 
        {
            path = "Assets/Logs/log.html";
        }

        if (logTemplate != null)
        {
            pathToTemplate = AssetDatabase.GetAssetPath(logTemplate);
        }
        else {
            pathToTemplate = "Assets/Logs/logTemplate.html";
        }

        File.Copy(pathToTemplate, path, true);
        writer = new StreamWriter(path, append:true);
        string monthVar = System.DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
        writer.WriteLine("<h1>Parameters Log " + monthVar + "</h1>");
	}

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            EventManager.TriggerEvent("Log","Input:A Pressed");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            EventManager.TriggerEvent("Log", "System:S Pressed");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            EventManager.TriggerEvent("Log", "GameLogic:D Pressed");
        }

    }


    void OnApplicationQuit()
    {
        if (writer != null) { writer.Close(); }
    }

    public void LogMessage(GameEvent ev)
    {
        String logText = (string)ev.data;
        string[] logTextParts = logText.Split(':');
        string logClass = logTextParts[0];
        string logMessage = logTextParts[1];
        writer.Write("<p class='" + logClass + "'>");
        writer.Write("<span class='time'>" + Math.Round(Time.realtimeSinceStartup, 4) + "</span>");
        writer.Write("<a onclick=\"hide('trace"+entryNumber+"')\">STACK</a> ");
        writer.Write(logMessage);
        writer.Write("<pre id='trace"+entryNumber+"'>" + Environment.StackTrace + "</pre>");
        writer.WriteLine("</p>");
        writer.Flush();
        entryNumber++;
    }
    #endif
}
