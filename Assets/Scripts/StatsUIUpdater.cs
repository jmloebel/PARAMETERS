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
	StatsUIUpdater monitors and updates the UI for player stats and level.
	</summary>
*/
public class StatsUIUpdater : MonoBehaviour {

	[Header ("UI Connections")]
	public Text level;
	public Text gold;
	public Text silverKeys;
	public Text goldKeys;
	public Text addParam;


    public void Start() {
        silverKeys.GetComponentInParent<Image>().sprite = I18N.GetSprite(I18N.Localize("silverKey"));
        goldKeys.GetComponentInParent<Image>().sprite = I18N.GetSprite(I18N.Localize("goldKey"));

    }

    // display game stats
    public void OnGUI() {
		level.text = I18N.Localize("LEVEL") + " " + Player.instance.level;
		gold.text = I18N.Localize("MON") + " " + Player.instance.gold;
		silverKeys.text = Player.instance.silverKeys.ToString();
		goldKeys.text = Player.instance.goldKeys.ToString();

		// are skill points available?
		if (Player.instance.skillPoints > 0) { addParam.text = "+" + Player.instance.skillPoints; }
		else { addParam.text = ""; }
	}

}
