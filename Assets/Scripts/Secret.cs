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
	Secret implements one time secret bonus for the player. Secrets are activated by certain achievements.
	</summary>
*/
[ExecuteInEditMode]
public class Secret : Room {

	[Header ("Secret Settings")]
	public string secret;
	public bool isEnabled, used;


	public Secret() {
		type = roomType.secret;
	}

	// attach listeners
	void OnEnable() {
		EventManager.StartListening("Secret", SecretListener);
	}

	void OnDisable() {
		EventManager.StopListening("Secret", SecretListener);
	}

	public override void RoomInit ()
	{
		type = roomType.secret;
		isEnabled = false;
		used = false;
        if (GetComponentsInChildren<Image>().Length > 3)
        {
            Image chestIcon;
            chestIcon = GetComponentsInChildren<Image>()[3];
            chestIcon.sprite = I18N.GetSprite(I18N.Localize("secret"));
        }
        
    }

	// disable secret room UI
	void DisableRoom() {
		gameObject.GetComponent<Button>().interactable = false;
		statsBar.color = new Color (0.18431373f, 0.28235294f, 0.32156863f);
		statsText.color = Color.black;
		used = true;
		isEnabled = false;
	}

	public void SecretListener(GameEvent ev) {
		int secretNumber = (int)ev.data;
		switch (secretNumber) {
			case 1:
				if (secret.Equals ("LIFE")) { //Life +20
					statsText.text = "LIFE\n+20";
					isEnabled = true;
				}
				break;
			case 2:
				if (secret.Equals ("RCVx2")) {
					statsText.text = "RCV.\nx2";
					isEnabled = true;
				}
				break;
			case 3:
				if (secret.Equals ("ACT+20")) { // Buy one of every weapon / By buying 1x of the most expensive armor and weapon (öffnet: ?)
					statsText.text = "ACT.\n+20";
					isEnabled = true;
				}
				break;
			case 4:
				if (secret.Equals ("$200")) { // Fill the blue bar by collecting a lot of money at once (öffnet oben rechts: Lv x 200$)
					statsText.text = "$200\nxLv.";
					isEnabled = true;
				}
				break;
			case 5:
				if (secret.Equals ("ATKx2")) { // Collect the NEKOGAMES letters (öffnet ?)
					statsText.text = "ATK.\nx2";
					isEnabled = true;
				}
				break;
		}

		// set up secret UI if activated
		if (isEnabled) {
			statsBar.fillAmount = 1.0f;
			statsBar.color = new Color (0f, 0.60784314f, 1f);
		}
	}

	public override void DoRoomAction() {
		if ( isEnabled && !used ) { // let player collect secret reward
			switch (secret) {
			case "LIFE":
				Player.instance.lifeMax += 20;
				break;
			case "RCVx2":
				Player.instance.recovery *= 2;
				break;
			case "ACT+20": // add 20 action points
				Player.instance.actionMax += 20;
				break;
			case "$200":
				for (int i = 1; i < Player.instance.level; i++) {
					Player.instance.gold += 200;
				}
				break;
			case "ATKx2":
				Player.instance.attackMax *= 2;
				break;
			}

			DisableRoom(); // deactivate secret after collection
		}
	}

	void Start () {
		RoomInit ();
	}
}
