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
	Enemy implements an enemy which has a life and can be attacked and killed by the player.
	Some enemies may need to be unlocked with a silver key first.
	</summary>
*/
[ExecuteInEditMode]
public class Enemy : Room {

	[Header ("Enemy Settings")]
	public float life; // enemy hitpoints
	public float lifeMax; // enemy max hitpoints
	public string item; // enemies may leave a gold key behind
	public int letterChance; // percentage of bonus letter appearing
	public int turnTime; // time for enemy attack in milliseconds

	// internal game logic for "Enemy"
	GameManager.States state;
	int keyCount;
	float turnCount;

	[Header ("UI Connections")]
	public Color deadColor;

	public Enemy() {
		type = roomType.enemy;
	}

	// display enemy stats
	void OnGUI() {
		statsText.text = Mathf.RoundToInt(life).ToString()+"/"+Mathf.RoundToInt(lifeMax).ToString();
		statsBar.fillAmount = (float)life / (float)lifeMax;
	}

	protected override void Awake() {
		base.Awake ();
	}

	void Start () {
        if (GetComponentsInChildren<Image>().Length > 3){
            Image lockIcon;
            lockIcon = GetComponentsInChildren<Image>()[3];
            lockIcon.sprite = I18N.GetSprite(I18N.Localize("silverLock"));
        }

        RoomInit();
	}
	
	public override void RoomInit() {
		type = roomType.enemy;
		// set up enemy game logic
		state = GameManager.States.stay;
		lifeMax = ParameterManager.GetStrategy().CalculateMaxLifeFor(this);
		life = lifeMax;
		keyCount = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.SilverKey);
	}

	
	void Update () {
		/* set up editor view */
		if ( Application.isEditor && !Application.isPlaying ) {
			life = lifeMax;
			statsText.text = life.ToString()+"/"+lifeMax.ToString();
		}

		if ( Application.isPlaying ) {
			switch (state) {
				case GameManager.States.stay: // recover enemy hitpoints if idle
					life += ParameterManager.GetStrategy().CalculateRecoveryFor(this);
					if (life > lifeMax) { life = lifeMax; } // don't go over maximum hitpoints
					break;

				case GameManager.States.turn: //敵の攻撃	Enemy attack
					turnCount += Time.deltaTime * 1000; // time in milliseconds
					if ( turnCount >= turnTime ) {
						// do damage to player
						Player.instance.SetDamage( ParameterManager.GetStrategy().CalculateDamageBy(this) );
						// update states
						state = GameManager.States.stay;
						Player.instance.state = GameManager.States.standby;
					}
					break;
			}
		}
	}

	// update UI after enemy has been defeated, disable interaction with enemy
	void DisableRoom() {
		background.color = deadColor;
		statsText.color = Color.black;
		gameObject.GetComponent<Button>().interactable = false;
		state = GameManager.States.end;
	}

	public override void DoRoomAction() {
		// check room lock
		if ( isLocked ) {
			if ( Player.instance.UseKey(Player.KeyTypes.SilverKey) ) { // try to unlock with silver key
				this.SetLocked (false);
				EventManager.instance.WriteEventMsg (I18N.Localize("Unlock the enemy"), "#FFFFFF");
				return;
			} else {
				EventManager.instance.WriteEventMsg (I18N.Localize("Locked"), "#888888");
				return;
			}
		}

		// if the player isn't idle --> do nothing
		if (Player.instance.state != GameManager.States.standby) { return; }

		// if player has no life left or boss is dead --> do nothing
		if (Player.instance.life <= 0 || life <= 0) { return; }

		Attack(); // attack enemy
	}

	void Attack() {
		// calculate damage
		int damage = ParameterManager.GetStrategy().CalculateDamageTo(this);

		// hit enemy
		life -= damage;
		EventManager.instance.WriteEventMsg(I18N.Localize("ATK.>")+damage+" "+I18N.Localize("damage to enemy"), "#FF00FF");

		if ( life <= 0 ) { // enemy defeated ?
			life = 0;
			DisableRoom();

			// generate rewards
			int gold = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.Gold);
			int exp = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.EXP);
			EventManager.instance.WriteEventMsg(I18N.Localize("You win!"), "#66FFFF");
			EventManager.TriggerEvent("Enemy"); //Look for Achievement
			// check if enemy had a gold key
			int goldkeys = 0;
			if (item.Equals ("GOLDKEY")) { goldkeys = 1; }
			// throw rewards
			rewardManager.GenerateReward(exp, gold, keyCount, goldkeys, Input.mousePosition);
			if (UnityEngine.Random.value * 100 < letterChance) { rewardManager.GenerateLetter(Input.mousePosition); }

		}
		// enemy still alive ? --> set up another round
		if ( life > 0 ) {
			turnCount = 0f;
			state = GameManager.States.turn;
			Player.instance.state = GameManager.States.turn;
		}
	}
}
