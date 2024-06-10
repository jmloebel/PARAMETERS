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

[ExecuteInEditMode]
public class GameManager : MonoBehaviour {

	LevelLoader levelLoader;

	public enum States {standby, turn, stay, end, push, result};
	public const int FRAMESPEED = 30;

	private static GameManager gameManager;
	public static GameManager instance {
		get {
			if (!gameManager) {
				gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

				if (!gameManager) {
					gameManager = new GameObject ().AddComponent<GameManager> ();
					gameManager.gameObject.name = "Game Manager";						
				}
				gameManager.Init ();
			}

			return gameManager;
		}
	}

	void Awake () {
		// load balancing strategy methods for game, select first strategy in component list
		IParametersStrategy strategy = ParameterManager.instance.GetComponent<IParametersStrategy>();
		ParameterManager.ProvideStrategy(strategy);

		// init level loader
		levelLoader = (LevelLoader)FindObjectOfType(typeof(LevelLoader));
	}

	void Init() {
	}

	public void InitGame() {
		AchievementManager.instance.Init();
		levelLoader.SetupLevel ();
		ParameterManager.LoadParameters();
	}

	void Start () {
		InitGame();
	}


}
