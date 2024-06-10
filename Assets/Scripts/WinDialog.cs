/*
	PARAMETERS Remake
    Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
	<summary>
	WinDialog represents the UI for the game winning screen. It displays a congratulation message.
	</summary>
*/
public class WinDialog : MonoBehaviour {

	[Header ("UI Connections")]
	public GameObject panel;
	public Text heading, winText, buttonText;

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("WinGame", WinListener);
	}

	void OnDisable() {
		EventManager.StopListening("WinGame", WinListener);
	}

	// game finished event --> set up panel UI and display
	public void WinListener(GameEvent ev) {
		winText.text = I18N.Localize("Parameters Cleared! TIME")+" ";
		winText.text += Mathf.FloorToInt (Time.realtimeSinceStartup / 3600).ToString ("00")
			+ ":" + Mathf.FloorToInt (Time.realtimeSinceStartup / 60).ToString ("00")
			+ ":" + Mathf.FloorToInt (Time.realtimeSinceStartup % 60).ToString ("00")+"\n";
		winText.text += I18N.Localize("Turn your computer off and go to sleep!");
		panel.SetActive (true);
	}

	// init panel logic
	void Start () {
		panel.SetActive (false);
		heading.text = I18N.Localize("Congratulations!");
		buttonText.text = I18N.Localize("OK");
	}

	public void DismissDialog() {
		panel.SetActive (false);
		// reload game
		GameManager.instance.InitGame();
	}
}
