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
	Room implements a generic UI room for the player to interact with. Derived classes define functionality such as enemies or missions.
	</summary>
*/
[ExecuteInEditMode]
public class Room : MonoBehaviour {

	// common room game logic vars
	public enum roomType { bonus, boss, casino, miniboss, mission, enemy, secret, store	}

	// super class for all Parameter rooms
	// specific room types inherit methods and vars from this class
	[Header ("Room UI Connections")]
	protected Image background;
	protected Image statsBar;
	protected Text statsText;
	protected RectTransform uiTransform;
	protected GameObject lockIcon;

	[Header ("Generic Settings")]
	public int posX;
	public int posY;
	public int width;
	public int height;
	public roomType type;
	public bool isLocked;

	protected RewardManager rewardManager;


	// set up room UI connections and display
	protected virtual void Awake() {
		uiTransform = GetComponent<RectTransform> ();
		background = GetComponentsInChildren<Image> () [1];
		statsBar = GetComponentsInChildren<Image> () [2];
		statsText = GetComponentInChildren<Text> ();

		if ( GetComponentsInChildren<Image> ().Length > 3 ) lockIcon = GetComponentsInChildren<Image> () [3].gameObject;
		
		rewardManager = (RewardManager)FindObjectOfType(typeof(RewardManager));

	}

	public virtual void DoRoomAction() {
		// implemented in sub-classes
	}

	public virtual void RoomInit() {
		// implemented in sub-classes
	}

	// lock or unlock room, set lock icon UI
	public virtual void SetLocked(bool locked) {
		isLocked = locked;
        if (lockIcon) {
            lockIcon.SetActive(isLocked);
        }
	}

	// set room dimensions
	public virtual void SetValues(int x, int y, int width, int height){
		posX = x;
		posY = y;
		this.width = width;
		this.height = height;
	}

}
