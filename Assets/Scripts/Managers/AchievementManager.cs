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
	AchievementManager holds the status of all in-game achievements and listens for respective events.
	</summary>
*/
public class AchievementManager : MonoBehaviour {
	
	[Header ("Status Counters")]
	public int enemyCompletedCount = 0;	//倒した敵の数  Number of enemies killed
	public int enemyTotal;
	
	public int missionCompletedCount = 0; 	//コンプリートしたミッション数  Completed Missions
	public int missionTotal;

	public int	itemsCompletedCount = 0;	//アイテムの数  Number of items

	[Header ("Combo Bar Settings")]
	public int combo;
	public float comboTime;
	public const int COMBOMAX = 120;
	public const int COMBOTIMEOUT = 1500; // time in milliseconds

	public static int[] letters = {0, 0, 0, 0, 0, 0, 0, 0, 0};
	public static int[] secrets = {0, 0, 0, 0, 0, 0, 0, 0};

	private static AchievementManager achievementManager;
	public static AchievementManager instance {
		get {
			if (!achievementManager) {
				achievementManager = FindObjectOfType(typeof(AchievementManager)) as AchievementManager;

				if (!achievementManager) {
					achievementManager = new GameObject ().AddComponent<AchievementManager> ();
					achievementManager.gameObject.name = "Achievement Manager";						
				}
				achievementManager.Init ();
			}

			return achievementManager;
		}
	}

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("Enemy", EnemyListener);
		EventManager.StartListening("Mission", MissionListener);
		EventManager.StartListening("ItemPurchased", ItemListener);
		EventManager.StartListening("Letter", LetterListener);
	}

	void OnDisable() {
		EventManager.StopListening("Enemy", EnemyListener);	
		EventManager.StopListening("Mission", MissionListener);	
		EventManager.StopListening("ItemPurchased", ItemListener);
		EventManager.StopListening("Letter", LetterListener);
	}

	public void Init() {
		combo = 0;
		comboTime = 0;
		letters = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0};
		secrets = new int[] {0, 0, 0, 0, 0, 0, 0, 0};
		missionCompletedCount = 0;
		itemsCompletedCount = 0;
		itemsCompletedCount = 0;
	}
	
	/* Init Game Vars */
	void Start () {
		Init();
	}
	
	void Update () {
		if (comboTime > 0) { // if player has active combo
			comboTime = comboTime - (Time.deltaTime * 1000f); // deduct time in milliseconds
			if (comboTime <= 0) { // combo bar time expired?
				if (combo >= COMBOMAX) {
					SetSecret(4); // maximum combo reached, unlock secret
				}
				combo = 0; // restart combo bar status
			}
		}		
	}

	public void EnemyListener(GameEvent ev) {
		enemyCompletedCount++;
		if (enemyCompletedCount == enemyTotal) { SetSecret(2); } // unlock secret if all enemies in level are killed
	}

	public void ItemListener(GameEvent ev) {
		itemsCompletedCount++;
		if (itemsCompletedCount == 9) { SetSecret(3); } // unlock secret if all items of a shop have been purchased
	}

	public void MissionListener(GameEvent ev) {
		missionCompletedCount++;
		if (missionCompletedCount == missionTotal) { SetSecret(1); } // unlock secret if all missions in level have been completed
	}

	public void LetterListener(GameEvent ev) {
		int number = (int)ev.data;
		letters[number] = 1; // toggle letter visibility
		// count number of active letters
		var countLetters = 0;
		foreach(int letter in letters) {
			if ( letter == 1 ) countLetters++;
		}
		if (countLetters == 9) { SetSecret(5); }; // unlock secret if all letters have ben activated
	}

	public void SetSecret(int secretNumber) {
		switch(secretNumber) {
		case 1://全ミッションクリア  Clear all missions
			EventManager.TriggerEvent("Secret", 1); // room #7, Life +20 
			EventManager.instance.WriteEventMsg(I18N.Localize("Completed all missions."), "#FF00FF");
			break;
		case 2://全敵クリア	Clear all enemies
			EventManager.TriggerEvent("Secret", 2); // room # 103, RCVx2
			EventManager.TriggerEvent("Secret", 6); // boss
			EventManager.instance.WriteEventMsg(I18N.Localize("To subdue all enemies."), "#FF00FF");
			break;
		case 3://全アイテム購買	All items purchase      
			EventManager.TriggerEvent("Secret", 3); // room #44, ACT+20
			EventManager.instance.WriteEventMsg(I18N.Localize("All items were purchased."), "#FF00FF");
			break;
		case 4://マックスコンボ	Max Combo
			if(secrets[4-1] == 0){
				EventManager.TriggerEvent("Secret", 4); //room #13, lvx200
				EventManager.instance.WriteEventMsg(I18N.Localize("MAXIMUM COMBO"), "#FFFF00");
			}
			break;
		case 5: // NEKOGAMES bonus word letters
			if(secrets[5-1] == 0){
				EventManager.TriggerEvent("Secret", 5); //room #49, ATKx2
				EventManager.instance.WriteEventMsg(I18N.Localize("Complete NEKOGAMES!"), "#FF00FF");
			}
			break;
		case 6://ラスボス倒す	Rasubosu (Last Boss) knocks down
			break;
		}

		secrets[secretNumber - 1] = 1; // mark secret as unlocked
		//全クリアチェック		Full clear check
		int secretCount = 0;
		for (int i = 0; i < 3; i++) {
			secretCount += secrets[i];
		}
		// TODO secretCount check

		//全ミッション、全的クリアでボス登場		All missions, boss appearing in full clear
		if ( secrets[0] != 0 && secrets[5] != 0 ) {
			// TODO
			//			mc_boss2.g.enable();
		}
	}

}
