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
using System;


// Factory
public class RewardManager : MonoBehaviour {

	public enum Rewards { EXP, Gold, SilverKey, GoldKey, Letter };
	public GameObject EXP;
	public GameObject Gold;
	public GameObject SilverKey;
	public GameObject GoldKey;
	public GameObject Letter;
	public GameObject levelPanel;
	
	/* from Main.as
	public function setItem(_items:Array,_px,_py):void {
			for (var i = 0; i < _items.length; i++) {
				var _kind = _items[i][0];
				var _value = _items[i][1];
				var itm = new ItemObj(this, _px, _py, _kind, _value);
			}
	*/

	public void GenerateReward(int exp, int gold, Vector3 pos) {
		GenerateReward(exp,gold,0,0,pos);
	}

	public void GenerateReward (int exp, int gold, int silverKeys, int goldKeys, Vector3 pos) {
		while (exp >= 100) {
			SpawnRewardItem(Rewards.EXP, 100, pos);
			exp -= 100;
		}
		while (exp >= 50) {
			SpawnRewardItem(Rewards.EXP, 50, pos);
			exp -= 50;
		}
		while (exp >= 10) {
			SpawnRewardItem(Rewards.EXP, 10, pos);
			exp -= 10;
		}
		if (exp > 0) {
			SpawnRewardItem(Rewards.EXP, exp, pos);
		}

		while (gold >= 100) {
			SpawnRewardItem(Rewards.Gold, 100, pos);
			gold -= 100;
		}	
		while (gold >= 50) {
			SpawnRewardItem(Rewards.EXP, 50, pos);
			gold -= 50;
		}
		while (gold >= 10) {
			SpawnRewardItem(Rewards.Gold, 10, pos);
			gold -= 10;
		}
		if (gold > 0) {
			SpawnRewardItem(Rewards.Gold, gold, pos);
		}

		for (int i=0; i < silverKeys; i++) {
			SpawnRewardItem(Rewards.SilverKey, 1, pos);
		}
		
		for (int i=0; i < goldKeys; i++) {
			SpawnRewardItem(Rewards.GoldKey, 1, pos);
		}
	}

	public void GenerateGold(int amount, Vector3 pos) {
		SpawnRewardItem(Rewards.Gold, amount, pos);
	}

	public void GenerateEXP(int amount, Vector3 pos) {
		SpawnRewardItem(Rewards.EXP, amount, pos);
	}

	public void GenerateLetter(Vector3 pos) {
		SpawnRewardItem(Rewards.Letter, (int)(UnityEngine.Random.Range(0,9)), pos);
	}

	public void SpawnRewardItem(Rewards type, int amount, Vector3 pos) {
		GameObject rewardInstance = null;

		switch(type) {
			case Rewards.EXP:
				rewardInstance = Instantiate(EXP);
				break;
			case Rewards.Gold:
				rewardInstance = Instantiate(Gold);
				break;
			case Rewards.SilverKey:
				rewardInstance = Instantiate(SilverKey);
				break;
			case Rewards.GoldKey:
				rewardInstance = Instantiate(GoldKey);
				break;
			case Rewards.Letter:
				rewardInstance = Instantiate(Letter);
				break;
		}

		if (rewardInstance != null) {
			RectTransform rewardInstanceTransform = rewardInstance.GetComponent<RectTransform>();
			rewardInstanceTransform.SetParent(levelPanel.transform);
			rewardInstanceTransform.position = pos;
			
			rewardInstance.GetComponent<Reward>().RewardInit(amount);
		}
	}
}
