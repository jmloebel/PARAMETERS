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
	BonusLetters implements the UI for the bonus word NEKOGAMES. If bonus letters are collected by the player, they will light up.
	</summary>
*/
[RequireComponent (typeof(Text))]
public class BonusLetters : MonoBehaviour {

	public Color[] bonusColors;

	Text bonusText;
	string bonusWord = "NEKOGAMES";

	public void Awake() {
		bonusText = GetComponent<Text> (); // set up UI connections
	}

	// display bonus word, color activated letters
	public void OnGUI() {
		string temp = "";
		for (int i = 0; i < bonusWord.Length; i++) {
			if ( AchievementManager.letters [i] != 0 )
				temp += "<color=#" + ColorUtility.ToHtmlStringRGB (bonusColors [i]) + ">" + bonusWord.ToCharArray () [i] + " </color>";
			else
				temp += bonusWord.ToCharArray () [i] + " ";
		}
		bonusText.text = temp;
	}

}
