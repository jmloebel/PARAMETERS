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

/**
	<summary>
	Player implements the main game logic and stats for the player.
	</summary>
*/
public class Player : MonoBehaviour {

	[Header ("Player Stats")]
	public int level;
	public int gold;
	public float life;
	public float lifeMax;
	public float lifeRecoverMin, lifeRecoverRate;
	public float defense, defenseMax;
	public float defenseRecoverMin, defenseRecoverRate;
	public float attack, attackMax;
	public float attackRecoverMin, attackRecoverRate;
	public int recovery;
	public int exp, expMax;
	public int expTotal;
	public int expTotalMax;
	public float action;
	public float actionMax;
	public float actionRecoverMin, actionRecoverRate;
	public int skillPoints;
	public int silverKeys;
	public int goldKeys;
	public enum KeyTypes {SilverKey, GoldKey};
	public enum PlayerStats {Attack, Defense, Life, Action}
	public GameManager.States state;

	[Header ("Status Meter Connections")]
	public Meter lifeMeter;
	public Meter expMeter;
	public Meter actMeter;
	public Meter rcvMeter;
	public Meter atkMeter;
	public Meter defMeter;

	// Player is a Singleton
    private static Player player;
	public static Player instance {
        get {
            if (!player) {
                player = FindObjectOfType(typeof(Player)) as Player;

                if ( !player ) {
					player = new GameObject ().AddComponent<Player> ();
					player.gameObject.name = "Player";
                }
            }

            return player;
        }
    }

	// attach listeners
	void OnEnable(){
		EventManager.StartListening("Reward", AddReward);
	}

	void OnDisable(){
		EventManager.StopListening("Reward", AddReward);	
	}

	void Start () {
		lifeMax = life;
		actionMax = action;
		attackMax = attack;
		defenseMax = defense;

		expTotal = 0;
		expMax = ParameterManager.GetStrategy().CalculateNextLevelEXP(); // xp to next level
		expTotalMax = expMax;

		state = GameManager.States.standby;
	}
	
	void Update () {
		if ( state == GameManager.States.standby ) { Recover(); } // if player is idle, recover stat points
		Display(); // update UI
	}

	// recover player stats at recovery rate
	void Recover() {
		life += ParameterManager.GetStrategy().CalculatePlayerRecoveryStat(PlayerStats.Life);
		if (life > lifeMax) { life = lifeMax; }

		action += ParameterManager.GetStrategy().CalculatePlayerRecoveryStat(PlayerStats.Action);
		if (action > actionMax) { action = actionMax; }

		defense += ParameterManager.GetStrategy().CalculatePlayerRecoveryStat(PlayerStats.Defense);
		if (defense > defenseMax) { defense = defenseMax; }

		attack += ParameterManager.GetStrategy().CalculatePlayerRecoveryStat(PlayerStats.Attack);
		if (attack > attackMax) { attack = attackMax; }
	}

	// update player stats UI meters
	void Display() {
		lifeMeter.SetStatValue(Mathf.RoundToInt(life), Mathf.RoundToInt(lifeMax));
		expMeter.SetStatValue(Mathf.RoundToInt(exp), Mathf.RoundToInt(expMax));
		actMeter.SetStatValue(Mathf.RoundToInt(action), Mathf.RoundToInt(actionMax));
		rcvMeter.SetStatValue(Mathf.RoundToInt(recovery), Mathf.RoundToInt(recovery)); // recovery
		atkMeter.SetStatValue(Mathf.RoundToInt(attack), Mathf.RoundToInt(attackMax));
		defMeter.SetStatValue(Mathf.RoundToInt(defense), Mathf.RoundToInt(defenseMax));
	}

	// try and use a key, returns false if player is out of keys
	public bool UseKey(KeyTypes keyType) {
		switch (keyType) {
			case KeyTypes.SilverKey:
				if ( silverKeys > 0 ) { // does player have silver keys ? --> use one
					silverKeys--;
					return true;
				}
				break;

			case KeyTypes.GoldKey:
				if( goldKeys > 0 ) { // does player have gold keys ? --> use one
					goldKeys--;
					return true;
				}
				break;
		}
		return false;
	}

	// add different rewards to player
	void AddReward(GameEvent gameEvent) {
		String reward = (String) gameEvent.data;
		RewardManager.Rewards kind; //Reward
		int amount; //amount

		if (reward.Substring(0, 1) == I18N.Localize("MON")) { // add money
			kind = RewardManager.Rewards.Gold;
			String amountString = reward.Substring(1,reward.Length-1);
			int.TryParse(amountString, out amount);
		}
		else { // add EXP
			kind = RewardManager.Rewards.EXP;
			int.TryParse(reward, out amount);
		}

		// add a silver key to inventory
		if (reward.Equals("SilverKey")) { kind = RewardManager.Rewards.SilverKey; }

		// add a gold key to inventory
		if (reward.Equals("GoldKey")) { kind = RewardManager.Rewards.GoldKey; }

		// give player reward
		switch (kind) {
			case RewardManager.Rewards.Gold:
				gold += amount;
				break;

			case RewardManager.Rewards.EXP:
				exp += amount;
				expTotal += amount;
				if (exp >= expMax) { // new level reached ?
					// increase player level
					level = level + 1;
					EventManager.TriggerEvent ("LevelUp", level);
					EventManager.instance.WriteEventMsg(I18N.Localize("LEVEL")+" "+level, "#66FF00");

					// deduct exp and calculate exp for next level up
					exp -= expMax;
					expMax = ParameterManager.GetStrategy().CalculateNextLevelEXP();
					expTotalMax += expMax;
					
					AddSkillPoints(3); // add 3 skill points for player to use
					life = lifeMax; // restore player life points
					action = actionMax; // restore player action points
				}
				break;

			case RewardManager.Rewards.SilverKey:
				silverKeys++;
				break;

			case RewardManager.Rewards.GoldKey:
				goldKeys++;
				break;
		}

		// reset combo bar timeout after reward and add point
		AchievementManager manager = FindObjectOfType<AchievementManager> (); // TODO make this a singleton
		if (manager.comboTime > 0) manager.combo++; else manager.combo = 0;
		manager.comboTime = AchievementManager.COMBOTIMEOUT;
	}

	// add skill points for player to increase stats
	public void AddSkillPoints(int points) {
			skillPoints += points;
	}

	// damage player life points by amount
	public void SetDamage(int damage) {
		EventManager.instance.WriteEventMsg(damage + " "+I18N.Localize("damage!"), "#FF0000");

		// deduct damage from player life
		life -= damage;

		if ( life < 0 ) { // is player dead ?
			// reset player stats to zero
			life = 0;
			attack = 0;
			defense = 0;
			// trigger event
			EventManager.TriggerEvent("PlayerDied");
		}
		Display(); // display player stats
	}

}
