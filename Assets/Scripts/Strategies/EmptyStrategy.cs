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

public class EmptyStrategy : MonoBehaviour, IParametersStrategy {

	public int CalculateDamageTo(Room attacked) {
		int damage = 0;
		return damage;
	}

	public int CalculateDamageBy(Room attacker) {
		int damage = 0;
		return damage;
	}

	public float CalculateRecoveryFor(Room attacker) {
		float recovery = 0f;
		return recovery;
	}

	public int CalculateNextLevelEXP() {
		int exp = 1;
		return exp;
	}

	public float CalculatePlayerRecoveryStat(Player.PlayerStats stat) {
		float recovery = 0;
		return recovery;
	}

	public int CalculateRoomRewardByType(Room room, RewardManager.Rewards rewardType) {
		int reward = 0;
		return reward;
	}

	public int CalculateMaxLifeFor(Room room) {
		int life = 1;
		return life;
	}

	public int CalculateMissionGrindReward(Mission mission) {
		int reward = 0;
		return reward;
	}

	public float CalculateMissionCost(Mission mission) {
		float cost = 0;
		return cost;
	}

	public int CalculateMissionProgress(Mission mission) {
		int progress = 0;
		return progress;
	}

	public int CalculateStoreSkillPoints(Store store) {
		int skillPoints = 0;
		return skillPoints;
	}

	public int CalculateStorePotionEffect(Store store) {
		int effect = 0;
		return effect;
	}

}
