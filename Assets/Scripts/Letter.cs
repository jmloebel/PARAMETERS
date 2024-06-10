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
	Letter implements the UI for a reward that throws a single letter of the bonus word NEKOGAMES to be collected.
	</summary>
*/
public class Letter : Reward {
	public int letterNumber; // index of letter in bonus word NEKOGAMES

	// set up letter UI
	public override void RewardInit(int amount) {
		letterNumber = amount;
		GetComponentsInChildren<Text>(true)[amount+1].gameObject.SetActive(true);
		collectPos = new Vector3(350+amount*12, Screen.height - 10, 0); // letter position in UI bonus word
	}

	// letters are always collected, trigger event
	public void OnDestroy() {
		EventManager.TriggerEvent("Letter", letterNumber);
	}
}
