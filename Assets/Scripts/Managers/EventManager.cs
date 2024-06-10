/*
	PARAMETERS Remake
    Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class UnityGameEvent : UnityEvent<GameEvent> { }

public class GameEvent {
    public GameEvent(object parameter) { data = parameter; }
    public object data;
}


public class EventManager : MonoBehaviour {

    private Dictionary<string, UnityGameEvent> eventDictionary;
	private Text eventLog;


    private static EventManager eventManager;
    public static EventManager instance {
        get {
            if (!eventManager) {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager) {
					eventManager = new GameObject ().AddComponent<EventManager> ();
					eventManager.gameObject.name = "Event Manager";
                }
				eventManager.Init();
            }

            return eventManager;
        }
    }

	void Awake() {
		if ( GameManager.instance == null ) Debug.Log("Game Manager not found in Scene!");
	}

	public void WriteEventMsg (string message, string color = "white") {
        //Debug.Log("Writing");
		eventLog.text += Mathf.FloorToInt(Time.realtimeSinceStartup/3600).ToString("00")
			+":"+Mathf.FloorToInt(Time.realtimeSinceStartup/60).ToString("00")
			+":"+Mathf.FloorToInt(Time.realtimeSinceStartup%60).ToString("00")
			+" <color="+color+">"+message+"</color>\n";
		eventLog.transform.parent.parent.gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0); // scroll to bottom of event log

		// truncate log if necessary
		int numLines = eventLog.text.Split('\n').Length;
		if (numLines > 20) {
			eventLog.text = eventLog.text.Substring(eventLog.text.IndexOf('\n')+1);
			eventLog.transform.parent.parent.gameObject.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0); // scroll to bottom of event log
		}
	}

    void Init() {
        if (eventDictionary == null) { eventDictionary = new Dictionary<string, UnityGameEvent>(); }

		if (eventLog == null) {
			eventLog = GameObject.FindGameObjectWithTag ("EventLog").GetComponent<Text>();
			eventLog.text = "";
		}
    }

    public static void StartListening(string eventName, UnityAction<GameEvent> listener) {
        UnityGameEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new UnityGameEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<GameEvent> listener) {
        if (eventManager == null) return;
        UnityGameEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, object parameter) {
        UnityGameEvent thisGameEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisGameEvent)) {
            thisGameEvent.Invoke( new GameEvent(parameter) );
        }
    }

    public static void TriggerEvent(string eventName) {
        TriggerEvent(eventName, null);
    }
    
}