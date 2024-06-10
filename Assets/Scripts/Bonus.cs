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
	Bonus class implements bonus rooms that contain power ups for purchase and can be unlocked with a gold key.
	</summary>
*/
[ExecuteInEditMode]
public class Bonus : Room {

	// game logic for "Bonus"
	[Header ("Bonus Settings")]
	public int price;
	// TODO implement limited bonus items
	public int amount; // -1 == unlimited, currently bonus room items are always unlimited
	public string item;

	[Header ("UI Connection")]
	public Text bonusText;


	public Bonus() {
		type = roomType.bonus;
	}

	void Start () {
        if (GetComponentsInChildren<Image>().Length > 3){
            Image lockIcon;
            lockIcon = GetComponentsInChildren<Image>()[3];
            lockIcon.sprite = I18N.GetSprite(I18N.Localize("goldLock"));
        }
        RoomInit();
	}

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("PlayerDied", PlayerListener);
		EventManager.StartListening("LevelUp", PlayerListener);
		
	}

	void OnDisable() {
		EventManager.StopListening("PlayerDied", PlayerListener);
		EventManager.StopListening("LevelUp", PlayerListener);
	}

	// player died or leveled up, reset price
	void PlayerListener(GameEvent ev) {
		ResetPrice();
	}

	// recalculates the purchase price
	void ResetPrice() {
		price = 100 + Player.instance.level * 20; // TODO make this configurable --> strategy pattern
	}

	public override void RoomInit ()
	{
		type = roomType.bonus;
        statsText.text = I18N.Localize("MON") + price; // set price for Unity Editor

		// set UI name of bonus item
		switch (item) {
			case "ACT++":
				bonusText.text = I18N.Localize("ACT.++");
				break;
			case "LIFE++":
				bonusText.text = I18N.Localize("LIFE++");
				break;
			case "LIFE":
				bonusText.text = I18N.Localize("LIFE Rcv.");
				break;
			case "PARAM":
				bonusText.text = I18N.Localize("Add Parm.");
				break;
		}
	}

	void OnGUI () {
		statsText.text = I18N.Localize("MON") + price; // display bonus item current price
	}

	public override void DoRoomAction() {
		// check room lock
		if ( isLocked ) {
			if ( Player.instance.UseKey(Player.KeyTypes.GoldKey) ) { // try to unlock with gold key
				SetLocked (false);
				// set up room UI
				statsBar.fillAmount = 1.0f;
				statsBar.color = new Color (0.4f, 0f, 1f);
				EventManager.instance.WriteEventMsg(I18N.Localize("Unlock"), "#FFFFFF");
				return;
			} else {
				EventManager.instance.WriteEventMsg(I18N.Localize("Locked"), "#888888");
				return;
			}
		}

		// can player afford item?
		if ( Player.instance.gold >= price ) { 
			switch (item) {
			case "ACT++": // increase maximum action points by one until player hits 200 action points
				if (Player.instance.actionMax < 200) {
					Player.instance.actionMax++;
					Player.instance.gold -= price;
					price = Mathf.RoundToInt(price * 1.05f); // increase price
					EventManager.instance.WriteEventMsg (I18N.Localize("ACT.> +1"), "#FF00FF");
				}
				break;

			case "LIFE++": // increase maximum life points by one until player hits 200 life points
				if (Player.instance.lifeMax < 200) {
					Player.instance.lifeMax++;
					Player.instance.gold -= price;
					price = Mathf.RoundToInt(price * 1.05f); // increase price
					EventManager.instance.WriteEventMsg (I18N.Localize("LIFE> +1"), "#FFCC00");
				}
				break;
			
			case "LIFE": // increase current life points by 100
				Player.instance.gold -= price;
				Player.instance.life += 100;
				if (Player.instance.life > Player.instance.lifeMax) { Player.instance.life = Player.instance.lifeMax; } // don't go over maximum life points
				price = Mathf.RoundToInt (price * 1.5f);  // increase price
				if (price > 3600) { price = 3600; } // maximum price for this item
				EventManager.instance.WriteEventMsg (I18N.Localize("REPAIR LIFE"), "#66FFFF");
				break;

			case "PARAM": // add one stats level-up point
				Player.instance.gold -= price;
				Player.instance.AddSkillPoints(1);
				EventManager.instance.WriteEventMsg (I18N.Localize("PURCHASE POINTS"), "#66FFFF");
				break;
			}


		}

	}
}
