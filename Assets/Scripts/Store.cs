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
	Store implements a store for the player to buy weapons, armor, or keys. Currently, stores can run out of weapons and armor but not keys.
	</summary>
*/
[ExecuteInEditMode]
public class Store : Room {

	[Header ("Store Settings")]
	public int amount; // -1 == unlimited
	public int price;
	public string item;
	public enum StoreTypes {Weapon, Armor, Key, Health}
	public StoreTypes sells;


	[Header ("UI Connections")]
	public Image itemSprite;
	public Text itemCountText;
	public Sprite weapon;
	public Sprite armor;
	public Sprite key;
	public Sprite potion;

    private Sprite spritebuffer;
	// store internal game logic
	int itemCount;
	int skillPoints;


	public Store() {
		type = roomType.store;
	}

	void Start () {
        if (GetComponentsInChildren<Image>().Length > 3)
        {
            Image lockIcon;
            lockIcon = GetComponentsInChildren<Image>()[3];
            lockIcon.sprite = I18N.GetSprite(I18N.Localize("silverLock"));
        }

        RoomInit();
	}

	// set up store UI and game logic
	public override void RoomInit ()
	{
		type = roomType.store;
		statsText.text = I18N.Localize("MON") + price;
// DEBUG		if ( !item.Equals("KEY") ) { price = (int)((m / 10) + Mathf.Pow(height-31, 2f) + (height - 31) * 45); } // 価格 price
		itemCount = 0;			// 所持数		The number of possession
		if ( item.Equals("ATK") ) {
			sells = StoreTypes.Weapon;
			skillPoints = ParameterManager.GetStrategy().CalculateStoreSkillPoints(this);
            // load sprite
            // itemSprite.sprite = weapon;
            spritebuffer = I18N.GetSprite(I18N.Localize("weapon-image"));
            if (spritebuffer != null) {
                itemSprite.sprite = spritebuffer;
            }
            else {
                itemSprite.sprite = weapon;
            }
        }

		if ( item.Equals("DEF") ) {
			sells = StoreTypes.Armor;
			skillPoints = ParameterManager.GetStrategy().CalculateStoreSkillPoints(this);
            // load sprite
            //itemSprite.sprite = armor;
            spritebuffer = I18N.GetSprite(I18N.Localize("armor-image"));
            if (spritebuffer != null) {
                itemSprite.sprite = spritebuffer;
            }
            else {
                itemSprite.sprite = armor;
            }
        }

		if ( item.Equals("KEY") ) {
			sells = StoreTypes.Key;
			//itemSprite.sprite = key; // load sprite
            spritebuffer = I18N.GetSprite(I18N.Localize("key-image"));
            if (spritebuffer != null) {
                itemSprite.sprite = spritebuffer;
            }
            else {
                itemSprite.sprite = key;
            }
            statsBar.color = new Color (0.72941176f, 0.72156863f, 0.22745098f);
			statsBar.fillAmount = 1.0f;
			if ( itemCountText != null ) itemCountText.gameObject.SetActive (false);
		}

		if ( item.Equals("HEALTH") ) {
			sells = StoreTypes.Health;
            //itemSprite.sprite = potion; // load sprite
            spritebuffer = I18N.GetSprite(I18N.Localize("health-image"));
            if (spritebuffer != null) {
                itemSprite.sprite = spritebuffer;
            }
            else {
                itemSprite.sprite = potion;
            }
        }

	}

	// display store UI
	void OnGUI () {
		statsText.text = I18N.Localize("MON") + price;
		itemCountText.text = "x" + itemCount;
	}


	public override void DoRoomAction() {
		// check room lock
		if ( isLocked ) {
			if ( Player.instance.UseKey(Player.KeyTypes.SilverKey) ) { // try to unlock with silver key
				SetLocked (false);
				EventManager.instance.WriteEventMsg (I18N.Localize("Unlock the item"), "#FFFFFF");
				return;
			} else {
				EventManager.instance.WriteEventMsg(I18N.Localize("Locked"), "#888888");
				return;
			}
		}

		if (amount >=0 && itemCount == amount) { // did store run out of items ?
			EventManager.instance.WriteEventMsg(I18N.Localize("items have reached the limit"), "#FFFFFF");
			return;
		}

		if ( Player.instance.gold >= price ) { // does player have enough money to purchase item ?
			Player.instance.gold -= price; // deduct purchase price
			var points = (int) (skillPoints * (1 - (itemCount * 0.05))); // calculate points to add to player stats
			if (points < 0) { points = 1; } // add at least one point to attack or defense stat
			itemCount++; // count purchased items

			switch (item) {
				case "KEY":
					// add silver key to player inventory
					Player.instance.silverKeys += 1;
					EventManager.instance.WriteEventMsg (I18N.Localize("Get key"), "#66FFFF");
					return;

				case "ATK":
					if (itemCount == 1) { EventManager.TriggerEvent ("ItemPurchased"); }
					// increase attack stat by points
					Player.instance.attack += points;
					Player.instance.attackMax += points;
					EventManager.TriggerEvent("ATK");
					// update UI
					statsBar.color = new Color (1.0f, 0.38823529f, 0.59607843f);
					statsBar.fillAmount = 1.0f;
					EventManager.instance.WriteEventMsg(I18N.Localize("BUY WEAPON > ATK")+" "+points+" "+I18N.Localize("UP!"), "#FF6699");
					break;

				case "DEF":
					if (itemCount == 1) { EventManager.TriggerEvent ("ItemPurchased"); }
					// increase defense stat by points
					Player.instance.defense += points;
					Player.instance.defenseMax += points;
					EventManager.TriggerEvent("DEF");
					// update UI
					statsBar.color = new Color (0.14901961f, 0.80784314f, 0.38039216f);
					statsBar.fillAmount = 1.0f;
					EventManager.instance.WriteEventMsg(I18N.Localize("BUY ARMOR > DEF.")+" "+points+" "+I18N.Localize("UP!"), "#33CC66");
					break;
				
				case "HEALTH":
					// increase player health
					Player.instance.life += ParameterManager.GetStrategy().CalculateStorePotionEffect(this);;
					if (Player.instance.life > Player.instance.lifeMax) { Player.instance.life = Player.instance.lifeMax; } // don't go over maximum life points
					// update UI
					statsBar.color = new Color (0.72941176f, 0.22745098f, 0.22745098f);
					statsBar.fillAmount = 1.0f;
					EventManager.instance.WriteEventMsg (I18N.Localize("REPAIR LIFE"), "#66FFFF");				
					break;
			}

			// close store if more than 9 weapons or armor purchased
			if ( amount >=0 && itemCount == amount ) { DisableRoom(); }
			
			// update UI with new price
			statsText.text = I18N.Localize("MON") + price;
		}

	}

	// update UI after store ran out of items
	void DisableRoom() {
		statsText.color = Color.black;
		itemCountText.color = Color.black;
		if (item.Equals ("ATK")) {
			statsBar.color = new Color (0.43137255f, 0.2745098f, 0.34117647f);
		}
		if (item.Equals ("DEF")) {
			statsBar.color = new Color (0.20784314f, 0.32156863f, 0.23921569f);
		}
		if (item.Equals ("HEALTH")) {
			statsBar.color = new Color (0.42941176f, 0.11745098f, 0.11745098f);
		}

	}
}
