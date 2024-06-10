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
using System.IO;
#if UNITY_EDITOR
	using UnityEditor;

/**
	<summary>
	LevelLoaderInspector is a game internal class which adds a custom inspector to the unity editor to (re)load and save levels while in edit mode.
	</summary>
*/
[CustomEditor(typeof(LevelLoader))]
public class LevelLoaderInspector : Editor {

	public override void OnInspectorGUI() {
		GUILayout.Space (20f);
		if ( GUILayout.Button("Reload Level") ) { 
			((LevelLoader)target).SetupLevel();
		}
		GUILayout.Label (" ");
		if (!((LevelLoader)target).LevelDataLoaded ())
			GUI.enabled = false;
		if ( GUILayout.Button("Save Level") ) { 
			((LevelLoader)target).WriteLevelData();
		}
		GUI.enabled = true;
		GUILayout.Space (20f);
		DrawDefaultInspector();

	}
}
#endif

/**
	<summary>
	LevelLoader is a game internal class which loads the current level from a CSV data file.
	</summary>
*/
[ExecuteInEditMode]
public class LevelLoader : MonoBehaviour {

	[Header("Level Data")]
	public TextAsset levelData;

	// TODO make this a lookup table
	[Header("Room Assets")]
	public GameObject enemy;
	public GameObject casino;
	public GameObject mission;
	public GameObject bonus;
	public GameObject boss;
	public GameObject secret;
	public GameObject store;

	[Header("UI Adjustments")]
	public int borderX = -1;
	public int borderY = +2;
	public int offsetY = +16;

	int missions;
	int enemies;

	public void SetupLevel() {
		missions = 0;
		enemies = 0;
		// clear level canvas
		while (transform.childCount > 0)
		{
			Transform child = transform.GetChild(0);
			child.SetParent(null,false);
			DestroyImmediate(child.gameObject);
		}

		// construct level from CSV
		string[] lines = levelData.text.Split ('\n');
		foreach (string line in lines) {
			string[] fields = line.Split(';');

			GameObject room = null;
			if (fields.Length > 1) switch (fields [1]) {
			case "bonus":
				room = Instantiate (bonus);
				room.GetComponent<Bonus> ().price = int.Parse (fields [7]);
				room.GetComponent<Bonus> ().amount = int.Parse (fields [8]);
				room.GetComponent<Bonus> ().item = fields [10];
				break;
			case "boss":
				room = Instantiate (boss);
				break;
			case "casino":
				room = Instantiate (casino);
				room.GetComponent<Casino> ().price = int.Parse (fields [7]);
				break;
			case "enemy":
				enemies++;
				room = Instantiate (enemy);
				room.GetComponent<Enemy> ().item = fields [9];
				break;
			case "mission":
				missions++;
				room = Instantiate (mission);
				break;
			case "secret":
				room = Instantiate (secret);
				room.GetComponent<Secret> ().secret = fields [11];
				break;
			case "store":
				room = Instantiate (store);
				room.GetComponent<Store> ().price = int.Parse (fields [7]);
				room.GetComponent<Store> ().amount = int.Parse (fields [8]);
				room.GetComponent<Store> ().item = fields [9];
				break;

			}

			// init room
			if (room != null) {
				room.name = fields [0] + ": " + room.name.Substring(0, room.name.Length-7);
				RectTransform roomTransform = room.GetComponent<RectTransform> ();
				room.transform.SetParent (transform);
				// TODO remove magic numbers for width and height border --> coordinate CSV with Jochen
				roomTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, int.Parse (fields [2]), int.Parse (fields [4])+borderX);
				roomTransform.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, int.Parse (fields [3]), int.Parse (fields [5])+borderY);
				room.GetComponent<Room> ().SetValues(int.Parse (fields [2]), int.Parse (fields [3]), int.Parse (fields [4]), int.Parse (fields [5]));

				room.GetComponent<Room> ().RoomInit();
				bool locked = false;
				
				if (fields [6] == "true")
					locked = true;
				room.GetComponent<Room> ().SetLocked (locked);
			}
		}
		// update stats
		AchievementManager manager = FindObjectOfType<AchievementManager>();
		manager.enemyTotal = enemies;
		manager.missionTotal = missions;

	}

	/**
	 * DEBUG: output GUI level to CSV
	 */
	#if UNITY_EDITOR
	public void WriteLevelData() {
		int height = Mathf.RoundToInt(GetComponent<RectTransform> ().rect.height);
//		string path = "Assets/Scenes/"+levelData.name+"-out.csv"; // use different name until this is fully implemented
		string path = AssetDatabase.GetAssetPath(levelData);

		StreamWriter writer = new StreamWriter(path);
		// write CSV header
		writer.WriteLine("Nr;type;x;y;width;height;locked;price;amount;item;bonus;secret");
		// write Room data from GUI
		for (int i=0; i < transform.childCount; i++) {
			RectTransform roomTransform = transform.GetChild (i).GetComponent<RectTransform>();
			Room room = transform.GetChild (i).GetComponent<Room> ();
			writer.Write ((i+1) + ";"); // room ID
			writer.Write (room.type + ";"); // type
            writer.Write(Mathf.RoundToInt(roomTransform.localPosition.x) + ";");  // x
            writer.Write(Mathf.RoundToInt(-roomTransform.localPosition.y) + ";"); // y
            writer.Write (Mathf.RoundToInt(roomTransform.rect.width-borderX) + ";"); // width
			writer.Write (Mathf.RoundToInt(roomTransform.rect.height-borderY) + ";"); // height
			if (room.isLocked) writer.Write("true;"); else writer.Write(";"); // locked
			// price + amount + item
			if (room.type == Room.roomType.bonus || room.type == Room.roomType.store || room.type == Room.roomType.enemy || room.type == Room.roomType.casino) {
				if (room.type == Room.roomType.bonus) {
					if (((Bonus)room).price > 0) writer.Write (((Bonus)room).price + ";"); else writer.Write (";");
					writer.Write (((Bonus)room).amount + ";");
					writer.Write(";");
//					writer.Write(((Bonus)room).item+";");
				} else if (room.type == Room.roomType.store) {
					if (((Store)room).price > 0) writer.Write (((Store)room).price + ";"); else writer.Write (";");
					writer.Write (((Store)room).amount + ";");
					writer.Write(((Store)room).item+";");
				} else if (room.type == Room.roomType.enemy) {
					writer.Write(";;"+((Enemy)room).item+";");					
				} else {
					writer.Write (((Casino)room).price + ";;;");
				}
			} else writer.Write(";;;");
			// bonus
			if (room.type == Room.roomType.bonus) {
				writer.Write(((Bonus)room).item+";");
			} else writer.Write(";");
			// secret
			if (room.type == Room.roomType.secret) writer.Write(((Secret)room).secret);

			writer.WriteLine ();
		}


		writer.Close();
	}
	#endif

	void Start () {
		
	}

	public bool LevelDataLoaded() {
		if (transform.childCount > 0) { return true; }
		
		return false;
	}
	
}
