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
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/**
	<summary>
	Reward implements the UI for a collectable reward for the player.
	</summary>
*/
public class Reward : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IParameterUpdatable {

	[Header ("Reward Settings")]
	public RectTransform collect;
	public float force;
	public float angle;
	public float gravityY;
	public float lifeTime;

	// internal game logic for rewards
	protected Vector3 collectPos;
	private float ground;
	private Vector3 direction;
	private Vector3 gravity;
	private float creationTime;
	bool active;
	enum RewardStates {incoming, stay, flicker, stayForever, collect};
	RewardStates state;
	String amount;
	float stay_cnt;
	bool canMove;
	
	// reward UI logic
	float r;
	float speed;
	float px,py;
	float ax,ay;
	float tx,ty;
	float landing_y;
	

	// init reward logic and UI params
	public void Awake () {
		ParameterManager.SetParam("Reward", "force", out force);
		ParameterManager.SetParam("Reward", "gravityY", out gravityY);
		ParameterManager.SetParam("Reward", "lifeTime", out lifeTime);
		ParameterManager.SetParam("Reward", "angle", out angle);

		gravity = new Vector3(0, -gravityY, 0);

		r = 90 + UnityEngine.Random.Range(-angle,angle);
		float launchAngle = r * Mathf.PI / 180;
		direction = new Vector3(Mathf.Cos(launchAngle),Mathf.Sin(launchAngle));
	 	direction *= force;

		ground += direction.x + r % 10;

		state = RewardStates.incoming;
		canMove = false;
	}

	// init reward amount and UI position
	public void Start () {
		amount = transform.GetChild(0).GetComponent<Text>().text;
		ground = transform.position.y + UnityEngine.Random.Range(-5.0f,0);
	}

	public void RecalculateParameters() {
		
	}

	// Trigger Reward collection via mouse
	public void OnPointerEnter(PointerEventData eventData) {
		MoveReward ();
	}
	public void OnPointerExit(PointerEventData eventData) {
		MoveReward ();
	}

	public virtual void RewardInit (int value) {
		// implemented in sub-classes
	}
	
	void Update () {
		Bounce(); // animate reward
	}

	void Bounce() {
		switch (state) {

			case RewardStates.incoming:
				direction += gravity*Time.deltaTime;
				if ((transform.position + direction).y < ground){
					direction.y *= -1;
					direction *= 0.5f;
					if ( direction.y < 0 ) { canMove = true; }
					if ( direction.y < 1 ) {
						stay_cnt = 1;
						canMove = true;
						state = RewardStates.stay;
					}
				}
				if ((transform.position + direction).x > (Screen.width-10) || (transform.position + direction).x < 10) {
					direction.x *= -1;
					canMove = true;
				}
				transform.position += direction;
				break;

			case RewardStates.stay:
				stay_cnt -= Time.deltaTime;
				if ( stay_cnt <= 0 ) {
					if (amount.Equals("GoldKey") || amount.Equals("SilverKey") || amount.Equals("Letter")){
						state = RewardStates.stayForever; //Keys and Letters don't vanish
					}
					else { state = RewardStates.flicker; }
				}
				break;

			case RewardStates.flicker: // Flicker Gold & XP
				stay_cnt += Time.deltaTime;
				float t = Mathf.PingPong(Time.time, 0.05f);
				if ( t > 0.025f ) { transform.GetChild(0).gameObject.SetActive(true); }
				else { transform.GetChild(0).gameObject.SetActive(false); }
				if ( stay_cnt > lifeTime ) { Destroy(gameObject); }
				break;

			case RewardStates.collect:
				transform.GetChild(0).gameObject.SetActive(true);
				direction.x = (collectPos.x - transform.position.x) * Time.deltaTime * 6;
				direction.y = (collectPos.y - transform.position.y) * Time.deltaTime * 3;
				transform.position += direction;
				if (Mathf.Abs(direction.x) < 1 && Mathf.Abs(direction.y) < 1) {
        			EventManager.TriggerEvent("Reward", amount);
					Destroy(gameObject);
				}
				break;

			default:
				break;
		}	
	}

 	public void MoveReward() {   
 		if (canMove) { state = RewardStates.collect; }		
		if ( state == RewardStates.stay || state == RewardStates.flicker || state == RewardStates.stayForever ) {
			state = RewardStates.collect;
		}
		else if ( state == RewardStates.incoming && direction.y < 0 ) {
			state = RewardStates.collect;
		}
    }
}
