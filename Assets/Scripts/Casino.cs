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
	Casino implements a slot machine the player can use to win items and gold once a certain level is reached.
	</summary>
*/
public class Casino : Room {

	[Header ("Casino Settings")]
	public int price;
	public int unlockLevel;
	public int winDollarItems, winDollarAmount;
	public string winDollarType;
	public int winAtItems, winAtAmount;
	public string winAtType;
	public int winSevenItems, winSevenAmount;
	public string winSevenType;
	public int winStarItems, winStarAmount;
	public string winStarType;

	// internal casino game logic
	float cnt = 0;
	float mc_num;
	int skillPoints;
	float sx, sy;
	int[] slotList = new int[] {0, 0, 0};
	int s_idx = 0;
	Text[] slots = new Text[3];
	GameManager.States state;

	// casino symbols to display in slot machine
	String[][] str = new String[][]	
	{
		new String[] {"$", "$", "$", "@", "@", "7", "*", "*"},
		new String[] {"$", "$", "$", "@", "@", "7", "*", "$"},
		new String[] {"$", "@", "7", "*", "$", "@", "7", "*"}
	};


	public Casino() {
		type = roomType.casino;
	}

	void Start () {		
		RoomInit ();
	}

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("LevelUp", LevelListener);
	}

	void OnDisable() {
		EventManager.StopListening("LevelUp", LevelListener);
	}

	// enable Casino when player reaches unlock level
	public void LevelListener(GameEvent ev) {
		int levelNumber = (int) ev.data;
		if (levelNumber == unlockLevel) { EnableRoom (); }
	}

	// enable casino, set up UI
	void EnableRoom() {
		statsBar.fillAmount = 1f;
		for (int i = 0; i < 3; i++) {
			slots [i].gameObject.SetActive (true);
			slots [i].text = "7";
		}
	}

	// set up room game logic
	public override void RoomInit () {
		type = roomType.casino;
		state = GameManager.States.stay;
		slots = GetComponentsInChildren<Text>(true);
	}

	public override void DoRoomAction() {
		// check if player is idle and can play casino
		if ( ! (Player.instance.state == GameManager.States.standby) ) { return; }

		// do no let player play if unlock level has not been reached
		if (Player.instance.level < unlockLevel) { return; }

		switch (state) {
			case GameManager.States.stay: // play new round
				if ( Player.instance.gold >= price ) { // does player have enough money?
					Player.instance.gold -= price;
					//ルーレットスタート	Roulette start
					s_idx = 0;
					slotList = new int[]{0, 0, 0};
					state = GameManager.States.push; // push slot buttons
				}
				break;

			case GameManager.States.push:
				s_idx++; // push single slot, increase index (1st, 2nd, 3rd slot)
				if ( s_idx == 3 ) { // all 3 slots played?
					state = GameManager.States.result;  // get result
				}
				break;
		}
	}

	void Update () {
		switch (state) {
			case GameManager.States.push: // rotate slot symbols
				cnt += Time.deltaTime;
				if ( cnt > 0.066f ) {
					cnt = 0f;
					for ( int i = s_idx; i < 3; i++ ) {
						slotList[i] = UnityEngine.Random.Range(0,8);
						slots[i].text = str[i][slotList[i]];
					}
				}
				break;

			case GameManager.States.result: // get play result
				String mark = slots[0].text;
				if ( mark.Equals(slots[1].text) && mark.Equals(slots[2].text) ) { // do all 3 slots have the same symbol ?
					switch (mark) { // generate reward for player depending on slot symbol
						case "$":
							for ( int i = 0; i < winDollarItems; i++ ) {
								if ( winDollarType.Equals("GOLD") ) rewardManager.GenerateGold(winDollarAmount, Input.mousePosition);
								else  rewardManager.GenerateEXP( winDollarAmount, Input.mousePosition);
							}
							break;
						case "@":
							for ( int i = 0; i < winAtItems; i++ ) {
								if ( winAtType.Equals("GOLD") ) rewardManager.GenerateGold(winAtAmount, Input.mousePosition);
								else  rewardManager.GenerateEXP( winAtAmount, Input.mousePosition);
							}
							break;
						case "7":
							for ( int i = 0; i < winSevenItems; i++ ) {
								if ( winSevenType.Equals("GOLD") ) rewardManager.GenerateGold(winSevenAmount, Input.mousePosition);
								else  rewardManager.GenerateEXP( winSevenAmount, Input.mousePosition);
							}
							break;
						case "*":
							for ( int i = 0; i < winStarItems; i++ ) {
								if ( winStarType.Equals("GOLD") ) rewardManager.GenerateGold(winStarAmount, Input.mousePosition);
								else  rewardManager.GenerateEXP( winStarAmount, Input.mousePosition);
							}
							break;
					}
				}
				state = GameManager.States.stay; // set casino idle, ready for next play
				break;
		} 		
	}
}
