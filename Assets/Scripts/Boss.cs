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
	Boss implements the final boss which has multiple lives and is tougher than regular enemies.
	</summary>
*/
[ExecuteInEditMode]
public class Boss : Room {

	[Header ("Boss Settings")]
	public float life;
	public float lifeMax;
	public int defensePower; // Defense power
	public int index;
	public float fill;

	[Header ("UI Connections")]
	public Color deadColor;

	// boss internal game logic
	GameManager.States state;
	float count;
	int keyCount;
	bool bossEnabled;
	Image[] statsBars = new Image[7]; // UI connections

	public Boss() {
		type = roomType.boss;
	}

	void Start () {
		RoomInit ();
	}

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("Secret", SecretListener);
	}

	void OnDisable() {
		EventManager.StopListening("LevelUp", SecretListener);
	}

	public void SecretListener(GameEvent ev) {
		int secretNumber = (int) ev.data;
		// activate boss if secret number 6 is achieved (all enemies killed)
		if ( secretNumber == 6 ) {
			// set up UI
			statsBar.fillAmount = 1f;
			statsBars [7].color = new Color (1f, 0.80392157f , 0f);
			bossEnabled = true;
		}
	}

	public override void RoomInit ()
	{
		type = roomType.boss;
		bossEnabled = false;
		foreach (Transform child in transform) { child.gameObject.SetActive(true); }
		statsBars = GetComponentsInChildren<Image>(true);

		lifeMax = ParameterManager.GetStrategy().CalculateMaxLifeFor(this);
		life = lifeMax;
		keyCount = ParameterManager.GetStrategy().CalculateRoomRewardByType(this, RewardManager.Rewards.SilverKey);

		state = GameManager.States.stay;
	}

	// display boss stats if boss is activated
	void OnGUI() {
		if ( bossEnabled ) {
			statsText.text = Mathf.RoundToInt (life).ToString () + "/" + Mathf.RoundToInt (lifeMax).ToString ();
			fill = (life % 48f) / 48f;
			if (fill == 0)
				fill = 1;
			if (life == 0)
				fill = 0;
			index = Mathf.CeilToInt (life / 48f) + 2;
			if (index < 7)
				statsBars [index + 1].fillAmount = 0;
			statsBars [index].fillAmount = fill;
		}
	}

	public override void DoRoomAction() {
		// only act if player is in standby (our turn)
		if (Player.instance.state != GameManager.States.standby) { return; }

		// check if boss is activated
		if ( !bossEnabled ) return;

		// display message if boss killed
		if ( state == GameManager.States.end ) {
			EventManager.instance.WriteEventMsg(I18N.Localize("You win!"), "#66FFFF");
			return;
		}

		// if player has no life left or boss is dead --> do nothing
		if (Player.instance.life <= 0 || life <= 0) { return; }

		Attack(); // attack boss
	}

	void Update () {		
		switch (state) {
			case GameManager.States.stay:
				life += ParameterManager.GetStrategy().CalculateRecoveryFor(this);
				if (life > lifeMax) { life = lifeMax; }
				break;
			case GameManager.States.turn: // Enemy attack
				count += Time.deltaTime;
				if (count >= 0.5) {
					count = 0;

					Player.instance.SetDamage( ParameterManager.GetStrategy().CalculateDamageBy(this) );
					state = GameManager.States.stay;
					Player.instance.state = GameManager.States.standby;
				}
				break;
		}
	}

	private void Attack() {
		var damage = ParameterManager.GetStrategy().CalculateDamageTo(this);
		if ( damage < 0 ) { damage = 1; } // do at least one point damage to boss

		// hit boss
		life -= damage;
		EventManager.instance.WriteEventMsg(I18N.Localize("ATK.>")+damage+" "+I18N.Localize("damage to enemy"), "#FF00FF");
		
		if ( life <= 0 ) { // is boss dead ?
			life = 0;
            state = GameManager.States.end;
			// update UI
			statsBars[index].fillAmount = 0;
			background.color = deadColor;
			statsText.color = Color.black;
			EventManager.instance.WriteEventMsg(I18N.Localize("You win!"), "#66FFFF");

			// generate rewards
			int gold = 10000;
			int exp = 9999;
			int val = (int)(gold / 10);
			int v =  (int)(exp / 15);
			rewardManager.GenerateReward(v, val,keyCount,0,Input.mousePosition);
			rewardManager.GenerateLetter(Input.mousePosition);

			// last boss defeated --> trigger game win event
			EventManager.TriggerEvent ("WinGame");
			EventManager.instance.WriteEventMsg (I18N.Localize("You win the Game!"), "#FFFFFF");
		}

		// Enemy (your turn)
		if ( life > 0 ) {
			count = 0;
			state = GameManager.States.turn;
			Player.instance.state = GameManager.States.turn;
		}

	}

}
