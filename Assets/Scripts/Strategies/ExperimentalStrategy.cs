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

/**
	<summary>
	ExperimentalStrategy can be used to test out new calculation methods for balancing the game.
	It takes it's initial settings from DefaultStrategy.
	</summary>
*/
public class ExperimentalStrategy : MonoBehaviour, IParametersStrategy {

	public int CalculateDamageTo(Room attacked) {
		int damage = 0;

		switch ( attacked.GetType().ToString() ) {
			case "Enemy": // calculate damage done to enemy by player
				Enemy enemy = (Enemy)attacked;

				damage = (int) Player.instance.attack * 2;
				/*
				var def = (enemy.height * enemy.height / 100) + (Screen.height - enemy.posY); //防御力	Defense power
				damage = (int)(Player.instance.attack * (1 - def / 2600) * 0.5);
				if ( enemy.height > enemy.width ) {
					damage = (int)(Player.instance.attack * 0.25 +(UnityEngine.Random.value * 10)-5);
				}
				if (damage < 0) { damage = 1; }
				*/
				break;

			case "Boss": // calculate damage done to boss by player
				Boss boss = (Boss)attacked;
				damage = (int)((Player.instance.attack - boss.defensePower) * 0.2);
				break;
			
			default:
				// unknown attacked room
				Debug.Log("Attack on unknown enemy class "+attacked.GetType().ToString()+" not implemented!");
				break;
		}

		return damage;
	}

	public int CalculateDamageBy(Room attacker) {
		int damage = 0;

		switch ( attacker.GetType().ToString() ) {
			case "Enemy": // damage to player done by enemy
				damage = (int) (attacker.height * 2) - (int) (Player.instance.defense * 2);
				damage = Mathf.Max(0, damage);
/*				damage = (20 + (attacker.height + attacker.width) / 2); */
				break;

			case "Boss": // damage to player done by boss
				damage = 100 - (int) ((Boss)attacker).life / 4;
				break;
			
			default:
				// unknown attacker
				Debug.Log("Attack by unknown enemy class "+attacker.GetType().ToString()+" not implemented!");
				break;
		}

		// int defense = (int) Player.instance.defense;
		// if (defense > 300) { defense = 300; } // maximum defense for player.
		// damage = (int) (damage * 0.5 * (1- (defense/300) )); // calculate damage
		// if (damage < 5) { // do minimum of 5 points damage
		// 	damage = (int) (5 + UnityEngine.Random.value * 10);
		// }

		return damage;
	}

	public float CalculateRecoveryFor(Room attacker) {
		float recovery = 0f;

		switch ( attacker.GetType().ToString() ) {
			case "Enemy":
				Enemy enemy = (Enemy)attacker;
				recovery = Time.deltaTime * (Screen.height - enemy.posY) * 0.01f;
				recovery = Mathf.Max(0.001f, recovery);
				// recovery = Time.deltaTime*GameManager.FRAMESPEED*(float)((Screen.height - enemy.posY) * 0.0001 * (enemy.lifeMax / 100) + Player.instance.recovery * 0.01);
				break;

			case "Boss":
				Boss boss = (Boss)attacker;
				recovery = (boss.lifeMax - boss.life) * 0.002f + 0.001f;;
				break;
			
			default:
				// unknown attacker
				Debug.Log("Recovery of unknown enemy class "+attacker.GetType().ToString()+" not implemented!");
				break;			
		}

		return recovery;
	}

	public int CalculateNextLevelEXP() {
		int exp;
		exp = 10 + ( Player.instance.level - 1 ) * ( Player.instance.level - 1 );
		// exp = 100 + ( Player.instance.level - 1 ) * ( Player.instance.level - 1 );
		return exp;
	}

	public float CalculatePlayerRecoveryStat(Player.PlayerStats stat) {
		float recovery = 0;

		switch (stat) {
			case Player.PlayerStats.Life:
				recovery = Player.instance.lifeRecoverMin + Player.instance.recovery * Player.instance.lifeRecoverRate;
				break;

			case Player.PlayerStats.Attack:
				recovery = Player.instance.attackRecoverMin + Player.instance.recovery * Player.instance.attackRecoverRate;
				break;

			case Player.PlayerStats.Defense:
				recovery = Player.instance.defenseRecoverMin + Player.instance.recovery * Player.instance.defenseRecoverRate;
				break;

			case Player.PlayerStats.Action:
				recovery = Player.instance.actionRecoverMin + Player.instance.recovery * Player.instance.actionRecoverRate;
				break;
		}

		return recovery;
	}

	public int CalculateRoomRewardByType(Room room, RewardManager.Rewards rewardType) {
		int reward = 0;

		switch (rewardType) {
			case RewardManager.Rewards.EXP:
				if ( room.GetType() == typeof(Mission) ) {
					reward = room.height / 5;
					// reward  = (int) (room.width * room.height * 0.1 * (1 + UnityEngine.Random.value) * 2 / 10);
				}
				if ( room.GetType() == typeof(Enemy) ) {
					reward = room.width * 2;
					// reward  = room.width * room.height / 15;
				}
				break;

			case RewardManager.Rewards.Gold:
				if ( room.GetType() == typeof(Mission) ) {
					reward = room.width / 5;
					// reward  = (int) (room.width * room.height * 0.03 * (1 + UnityEngine.Random.value) * 2 / 10);
				}
				if ( room.GetType() == typeof(Enemy) ) {
					reward = room.width * 2;
					// reward  = room.width * room.height / 10;
				}
				break;

			case RewardManager.Rewards.SilverKey:
				if ( room.GetType() == typeof(Mission) ) {
					reward  = 0; // don't throw keys for misions
				}
				if ( room.GetType() == typeof(Boss) ) {
					reward  = (int) ((float)room.width * (float)room.height / 1000f) % 4 + 1;
				}
				if ( room.GetType() == typeof(Enemy) ) {
					reward = 2;
					// reward  = (int) ((float)room.width * (float)room.height / 1000f) % 4 + 1;
				}
				break;

			default:
				Debug.Log("Reward calculation for "+rewardType.ToString()+" not implemented!");
				break;
		}

		return reward;
	}

	public int CalculateMaxLifeFor(Room room) {
		int life = 0;

		switch ( room.GetType().ToString() ) {
			case "Mission": // mission progress is determined by UI "room" size
				life = 100;
				break;

			case "Boss":
				life = 5 * 48;
				break;

			case "Enemy": // hitpoints of enemy are determined by UI "room" size
				life = room.width;
				break;
			
			default:
				// unknown room
				Debug.Log("Life calculation for room type "+room.GetType().ToString()+" not implemented!");
				break;			
		}

		return life;
	}

	public int CalculateMissionGrindReward(Mission mission) {
		int reward;
		// reward  =  (int)((mission.width * mission.height / 10) + UnityEngine.Random.value * mission.width * mission.height / 100) / 10;
		reward = mission.height / 10;
		return reward;
	}

	public float CalculateMissionCost(Mission mission) {
		float cost = mission.width * mission.height * 0.01f;
		return cost;
	}

	public int CalculateMissionProgress(Mission mission) {
		int progress;
		
		// progress = (int) (mission.progressMin + UnityEngine.Random.value * mission.progressRange);
		progress = 20;
		return progress;
	}

	public int CalculateStoreSkillPoints(Store store) {
		int skillPoints = 0;

		switch (store.sells) {
			case Store.StoreTypes.Weapon:
				skillPoints = (int) ( store.width * store.height / 100);
				// skillPoints = (int)( (( (store.width - 1) * (store.height - 1) ) / 100) * 0.45f); // 効果	effect
				break;

			case Store.StoreTypes.Armor:
				skillPoints = (int) ( store.width * store.height / 100);
				// skillPoints = (int)( (( (store.width - 1) * (store.height - 1) ) / 100) * 0.5f); // 効果	effect
				break;
		}

		return skillPoints;
	}

	public int CalculateStorePotionEffect(Store store) {
		int effect = 0;
		
		switch (store.sells) {
			case Store.StoreTypes.Health:
			return 20;
			break;
		}

		return effect;
	}
	
}
