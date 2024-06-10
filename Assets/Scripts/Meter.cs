/*
	PARAMETERS Remake
    Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
	<summary>
	Meter implements the UI for a generic status meter. Currently supported stats are: LIFE, ACT, ATK, DEF, RCV, EXP
	</summary>
*/
public class Meter : MonoBehaviour {

	[Header ("Meter Settings")]
	public StatsKind kind;
	public enum StatsKind { LIFE, ACT, ATK, DEF, RCV, EXP, COMBO }

	// meter internal logic
	Player player;
	int mc;
	int mc_max;
	int txt;
	int w;
	float targetValue;
	float value;
	int valueMax;
	int WIDTH_MAX = 200;
	int WIDTH_BONUS = 20;

	// meter internal UI
	private Image statsBar;
	private Text statsText;
	private Text statsLabel;
	private RectTransform uiTransform;


	void Awake() {
		// get UI components
		statsBar = GetComponentsInChildren<Image> () [2];
		statsText = GetComponentsInChildren<Text> ()[0];
		statsLabel = GetComponentsInChildren<Text> () [1];
		player = (Player)FindObjectOfType(typeof(Player));
		uiTransform = GetComponent<RectTransform> ();
		// set up UI
		gameObject.GetComponent<Image>().color = statsBar.color;
		statsLabel.text = I18N.Localize(kind.ToString());
		statsLabel.color = statsBar.color;
	}

	void Update () {
		switch (kind) {
			case StatsKind.LIFE:
			case StatsKind.ACT:
			case StatsKind.ATK:
			case StatsKind.DEF:
				// calculate smooth transition of status bar to destination value
				var d = (float)(targetValue - value) / 10f;
				value += d;
				
/*				if (valueMax != 0)
					w = valueMax * (value / valueMax);
				else
					w = 0;
				if (w > WIDTH_MAX) {
					w = WIDTH_MAX;
				}
*/
				// update status bar UI
				if ( valueMax != 0 ) { statsBar.fillAmount = ((float)value / (float)valueMax); }
				statsText.text = Mathf.RoundToInt(value) + "/" + valueMax;
				var m = valueMax;
				if (m > WIDTH_MAX) { m = WIDTH_MAX; }
//				statsBar.fillAmount = (float)m; //mc_max.x = m;
				break;

			// recovery meter is always fully filled
			case StatsKind.RCV:
				statsBar.fillAmount = 1.0f;
				statsText.text = Mathf.Round (valueMax).ToString();
				break;

			// calculate smooth transition of EXP status bar to destination value
			case StatsKind.EXP:
				d = (targetValue - value) / 10f;
				value += d;
				statsBar.fillAmount = ((float)value / (float)valueMax);
				statsText.text = player.expTotal + "/" + player.expTotalMax;
				break;

			// TODO combo bar not yet implemented in generic meter class
			case StatsKind.COMBO:
				if (targetValue - value < 0) {
					d = (targetValue - value) / 5;
				}
				else {
					d = (targetValue - value) / 10;
				}
				value += d;

/*				w = 752 * (value / valueMax);
				if (w >= 752) {
					w = 752;
					//mc.gotoAndStop(2);
				}
				else {
					//mc.gotoAndStop(1);
				}
				statsBar.fillAmount = (float)w;
*/
				break;
		}
	}

	// set new target values for meter UI
	public void SetStatValue(int current, int maximum) {
		targetValue = current;
		valueMax = maximum;
		if ( kind == StatsKind.EXP && targetValue < value ) { value = 0; } // restart EXP meter for new level
		if ( targetValue == 0 ) { value = 0; } // immediately set value 0 zero for any meter

		// update UI meter width
		int width = WIDTH_BONUS + valueMax;
		width = Mathf.Min (width, WIDTH_MAX);
		uiTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, uiTransform.position.x, width);

	}

	// set meter UI kind
	public void setKind(StatsKind statsKind) {
		kind = statsKind;
	}

}
