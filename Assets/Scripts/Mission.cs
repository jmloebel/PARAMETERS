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
	Mission implements a challenge for the player. Upon completions money and EXP rewards are generated.
	Some missions may need to be unlocked with a silver key first.
	</summary>
*/
[ExecuteInEditMode]
public class Mission : Room {
	[Header ("Mission Settings")]
	public int progressMin;
	public int progressRange;
	public float missionCompleteBonusEXP;
	public float missionCompleteBonusGold;
	public int letterChance; // percentage of bonus letter appearing

	[Header ("UI Connections")]
	public Color completedColor;
	
	// internal game logic for missions
	int progress;
	int progressMax;
	float cost;
	int gold, exp;
	int keyCount;
	bool completed;

	public Mission() {
		type = roomType.mission;
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

	public override void RoomInit () {
		type = roomType.mission;
		// set up mission game logic
		progress = 0;
		progress = 0;
		completed = false;
		progressMax = ParameterManager.GetStrategy().CalculateMaxLifeFor(this);
		gold = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.Gold);
		exp = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.EXP);
		keyCount = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.SilverKey);		
		cost = ParameterManager.GetStrategy().CalculateMissionCost(this);
		statsText.text = "" + (int)Mathf.Floor(100*progress/progressMax)+"%";
	}

	// display mission progress in UI
	void OnGUI()
	{
		if (progressMax != 0)
		{
			statsText.text = "" + (int)Mathf.Floor(100 * progress / progressMax) + "%";
			statsBar.fillAmount = (float)progress / progressMax;

		}
	}

	//ミッションの実行		Mission execution
	public override void DoRoomAction ()
	{
		// check room lock
		if ( isLocked ) {
			if ( Player.instance.UseKey(Player.KeyTypes.SilverKey) ) { // try to unlock with silver key
				this.SetLocked (false);
				EventManager.instance.WriteEventMsg (I18N.Localize("Unlock the mission"), "#FFFFFF");
				return;
			} else {
				EventManager.instance.WriteEventMsg (I18N.Localize("Locked"), "#888888");
				return;
			}
		}
		// try to complete mission
		if (Player.instance.action >= cost) { // does player have enough action points to go on mission ?
			Player.instance.action -= cost;
			
			if ( !completed ) { // progress mission
				progress += ParameterManager.GetStrategy().CalculateMissionProgress(this);
				if ( progress >= progressMax ) { // mission completed ?
					progress = progressMax;
					Player.instance.AddSkillPoints(1); // player gets one skill point for a completed mission
					// trigger event and update UI					
					EventManager.TriggerEvent("Mission");
					EventManager.instance.WriteEventMsg(I18N.Localize("Mission completed"), "#FF00FF");
					statsBar.color = completedColor;
					statsText.color = Color.black;
					completed = true;

					// generate rewards for completed mission
					gold = (int) (gold * missionCompleteBonusGold);
					exp = (int) (exp * missionCompleteBonusEXP);
				}	
				else { // player progressed in mission
					EventManager.instance.WriteEventMsg(I18N.Localize("Run>Success"), "#FFFFFF");
				}
			}	
			else { // completed missions let player grind for money
				//ミッション完了時		When the mission is completed
				exp = 0; // no experience points for completed missions
				gold = ParameterManager.GetStrategy().CalculateMissionGrindReward(this); // calculate reward
				EventManager.instance.WriteEventMsg(I18N.Localize("Collect the money"), "#FFFF00");
			}
			rewardManager.GenerateReward(exp, gold, keyCount, 0, Input.mousePosition);
			// randomly generate bonus word letter
			if (UnityEngine.Random.value * 100 < letterChance) {
				rewardManager.GenerateLetter(Input.mousePosition);
			}

		}
		// if player does not have enough action points to go on mission --> write message
		else {
			EventManager.instance.WriteEventMsg(I18N.Localize("Run>Attempted:Lack of ACT."), "#888888");
		}
	}
}
