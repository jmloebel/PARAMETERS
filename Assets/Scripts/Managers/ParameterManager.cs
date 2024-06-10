/*
	PARAMETERS Remake
    Copyright Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif


/** <summary> Interface for Game Objects to optionally trigger recalculation after new parameter values have been pushed.</summary> */
public interface IParameterUpdatable { void RecalculateParameters(); }

/** <summary> Interface for Game Objects to implement balancing strategies for the game.</summary> */
public interface IParametersStrategy { 
	int   CalculateDamageTo(Room attacked); // calculates damage done to enemies and bosses by player
	int   CalculateDamageBy(Room attacker);	// calculates damage done to player by attackers like enemies and bosses
	float CalculateRecoveryFor(Room attacker);
	int   CalculateMaxLifeFor(Room room);
	int   CalculateNextLevelEXP();
	float CalculatePlayerRecoveryStat(Player.PlayerStats stat);
	int   CalculateRoomRewardByType(Room room, RewardManager.Rewards rewardType);
	int   CalculateMissionGrindReward(Mission mission);
	float CalculateMissionCost(Mission mission);
	int   CalculateMissionProgress(Mission mission);
	int   CalculateStoreSkillPoints(Store store);
	int   CalculateStorePotionEffect(Store store);
	
 }

/**
	<summary>
	ParameterManager is a game internal class which provides balancing support for the game.
	Parameters are read from CSV files.
	</summary>
*/
[ExecuteInEditMode]
public class ParameterManager : MonoBehaviour {

	[HideInInspector]
	public List<string> parameterSheets = new List<string>();
	[HideInInspector]
	public List<string> strategies = new List<string>();
	[HideInInspector]
	public int sheetIndexToLoad;
	[HideInInspector]
	public int strategyIndexToLoad;

	public TextAsset defaultSheet;

	private static IParametersStrategy defaultStrategy;


	// DEBUG fallback parameters
	private static Dictionary<string, string> parameters = new Dictionary<string, string>() {
		// parameters for Player
		{ "Player.level", "1" },
		{ "Player.life", "100" },
		{ "Player.lifeMax", "100" },
		{ "Player.recovery", "10" },
		{ "Player.silverKeys", "0" },
		{ "Player.goldKeys", "0" },
		{ "Player.action", "50" },
		{ "Player.actionMax", "50" },
		{ "Player.attack", "10" },
		{ "Player.attackMax", "10" },
		{ "Player.defense", "10" },
		{ "Player.defenseMax", "10" },
		{ "Player.gold", "0" },
		{ "Player.exp", "0" },
		{ "Player.skillPoints", "3" },
		{ "Player.actionRecoverMin", "0.06" },
		{ "Player.actionRecoverRate", "0.01" },
		{ "Player.lifeRecoverMin", "0.004" },
		{ "Player.lifeRecoverRate", "0.002" },
		{ "Player.attackRecoverMin", "0.03" },
		{ "Player.attackRecoverRate", "0.01" },
		{ "Player.defenseRecoverMin", "0.03" },
		{ "Player.defenseRecoverRate", "0.01" },
		// parameters for Boss
		{ "Boss.defensePower", "200" },
		// parameters for Enemy
		{ "Enemy.letterChance", "25" },
		{ "Enemy.turnTime", "500" },
		// parameters for Casino
		{ "Casino.unlockLevel", "40" },
		{ "Casino.winDollarItems", "8" },
		{ "Casino.winDollarAmount", "200" },
		{ "Casino.winDollarType", "GOLD" },
		{ "Casino.winAtItems", "10" },
		{ "Casino.winAtAmount", "20" },
		{ "Casino.winAtType", "EXP" },
		{ "Casino.winSevenItems", "20" },
		{ "Casino.winSevenAmount", "777" },
		{ "Casino.winSevenType", "GOLD" },
		{ "Casino.winStarItems", "12" },
		{ "Casino.winStarAmount", "800" },
		{ "Casino.winStarType", "GOLD" },
		// parameters for Mission
		{ "Mission.progressMin", "10" },
		{ "Mission.progressRange", "3" },
		{ "Mission.letterChance", "3" },
		{ "Mission.missionCompleteBonusEXP", "1.5" },
		{ "Mission.missionCompleteBonusGold", "1.5" },
		// parameters for Reward
		{ "Reward.force",     "5.5" },
		{ "Reward.gravityY",  "9.81" },
		{ "Reward.lifeTime",  "1.0" },
		{ "Reward.angle",    "30.0" },
		// parameters for SkillManager
		{ "SkillManager.addRecoveryMin", "1" },
		{ "SkillManager.addRecoveryMax", "1" },
		{ "SkillManager.addAttackMin", "1" },
		{ "SkillManager.addAttackMax", "3" },
		{ "SkillManager.addDefenseMin", "1" },
		{ "SkillManager.addDefenseMax", "3" },
		{ "SkillManager.addLifeMin", "1" },
		{ "SkillManager.addLifeMax", "1" },
		{ "SkillManager.addActionMin", "1" },
		{ "SkillManager.addActionMax", "1" }
	};

	private static ParameterManager parameterManager;
	public static ParameterManager instance {
		get {
			if (!parameterManager) {
				parameterManager = FindObjectOfType(typeof(ParameterManager)) as ParameterManager;

				if (!parameterManager) {
					parameterManager = new GameObject ().AddComponent<ParameterManager> ();
					parameterManager.gameObject.name = "Parameter Manager";						
				}
			}

			return parameterManager;
		}
	}

	void OnEnable() {
		Object[] balancingSheets = Resources.LoadAll("Balancing", typeof(TextAsset));
		if ( balancingSheets == null ) { 
			Debug.Log("Balancing directory in resources not found!");
			return;
		}
		parameterSheets.Clear ();
		foreach (var sheet in balancingSheets) {
			parameterSheets.Add (sheet.name);
		}
		strategies.Clear();
		IParametersStrategy[] managerStrategies = ParameterManager.instance.GetComponents<IParametersStrategy>();
		foreach (var strategy in managerStrategies) {
			strategies.Add (strategy.ToString().Substring(strategy.ToString().IndexOf("(")));
		}

	}

	public struct userAttributes
	{
		//any attributes you might want to use for segmentation, empty if nothing
	}
	public struct appAttributes
	{
		//any attributes you might want to use for segmentation, empty if nothing
	}


	void Awake()
	{
		// Add a listener to apply settings when successfully retrieved:
		ConfigManager.FetchCompleted += ApplyRemoteSettings;

		// Set the user’s unique ID:
		ConfigManager.SetCustomUserID("some-user-id");

		// Set the environment ID:
		ConfigManager.SetEnvironmentID("an-env-id");

		// Fetch configuration setting from the remote service:
		ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
	}

	// Create a function to set your variables to their keyed values:
	void ApplyRemoteSettings(ConfigResponse configResponse)
	{
		switch (configResponse.requestOrigin)
		{
			case ConfigOrigin.Default:
				//Debug.Log("No settings loaded this session; using default values.");
				break;
			case ConfigOrigin.Cached:
				//Debug.Log("No settings loaded this session; using cached values from a previous session.");
				break;
			case ConfigOrigin.Remote:
				//Debug.Log("New settings loaded this session; update values accordingly.");
				int remoteLife = ConfigManager.appConfig.GetInt("LIFE");
				//Debug.Log("Variable LIFE: " + remoteLife);
				break;
		}
	}

	public static void LoadParameters() {
		LoadParameters (ParameterManager.instance.defaultSheet);

	}

	public static void LoadParameters(string sheetName) {
		LoadParameters (Resources.Load ("Balancing/" + sheetName) as TextAsset);
	}

	public static void LoadParameters(TextAsset sheet) {
		if (sheet == null) {
			ApplyParameters ();
			return;
		}
		// load parameters from CSV Text Asset
		bool isHeader = true;
		parameters.Clear ();
		string[] lines = sheet.text.Split ('\n');
		string className = "";
		foreach (string line in lines) {
			if ( isHeader ) { // don't parse CSV table header
				isHeader = false;
				continue;
			}
			string[] fields = line.Split (';');
			if ( fields.Length != 3 ) continue;
			fields [0] = fields [0].Trim ();
			fields [1] = fields [1].Trim ();
			fields [2] = fields [2].Trim ();
			if (fields [0].Length > 0) { className = fields [0]; }

			if (className.Length > 0) {
				parameters.Add (className + "." + fields [1], fields [2]);
			//	Debug.Log(className + "." + fields[1] + " - " + fields[2]);
			}
		}
		ApplyParameters();
	}

	// using reflection, inject public variables of specified game objects with values loaded from CSV
	private static void ApplyParameters() {
		foreach (string name in parameters.Keys) {
			string className = name.Split ('.')[0];
			string paramName = name.Split ('.')[1];
			foreach (var obj in FindObjectsOfType(System.Type.GetType(className))) {
				if ( obj.GetType().GetField(paramName) != null ) { // check if class has param variable and if it is publicly accessible
					System.Type fieldType = obj.GetType ().GetField (paramName).FieldType;
					// assign int variable
					if ( fieldType == typeof(int)) {
						int intValue;
						int.TryParse(GetParamValue(name), out intValue);
						obj.GetType().GetField(paramName).SetValue(obj, intValue);
					}
					// assign float variable
					if ( fieldType == typeof(float)) {
						float floatValue;
						float.TryParse(GetParamValue(name), out floatValue);
						obj.GetType().GetField(paramName).SetValue(obj, floatValue);
					}
					// assign string variable
					if ( fieldType == typeof(string)) {
						obj.GetType().GetField(paramName).SetValue(obj, GetParamValue(name));
					}

				} else Debug.Log("ParameterManager: Could not set "+name+" because class var is not public or does not exist");
			}
		}
	}

	private static string GetParamValue(string name) {
		string value;
		if ( parameters.TryGetValue(name, out value) ) { return value; } 
		return null;
	}

	public static void SetParam (object paramClass, string name, out int param) {
		SetParam (paramClass.GetType ().ToString (), name, out param);
	}

	public static void SetParam (object paramClass, string name, out float param) {
		SetParam (paramClass.GetType ().ToString (), name, out param);
	}

	public static void SetParam (object paramClass, string name, out string param) {
		SetParam (paramClass.GetType ().ToString (), name, out param);
	}

	public static void SetParam (string className, string name, out int param) {
		int.TryParse(GetParamValue(className+"."+name), out param);
	}

	public static void SetParam (string className, string name, out float param) {
        float.TryParse(GetParamValue(className + "." + name), NumberStyles.Float, CultureInfo.InvariantCulture, out param);
	}

	public static void SetParam (string className, string name, out string param) {
		param = GetParamValue(className+"."+name);
	}

	public static IParametersStrategy GetStrategy() {
		// provide first strategy in component list for edit mode
		if ( defaultStrategy == null && !Application.isPlaying ) return ParameterManager.instance.GetComponent<IParametersStrategy>();
		// provide strategy
		if ( defaultStrategy == null ) Debug.Log("No Strategy provided. Game cannot proceed!");
		return defaultStrategy;		
	}

	public static void ProvideStrategy(IParametersStrategy strategy) {
		defaultStrategy = strategy;
	}
}

#if UNITY_EDITOR
/**
	<summary>
	ParameterManagerInspector is a game internal class which adds a custom inspector
	to load new game balancing parameters while the game is running in the editor.
	</summary>
*/
[CustomEditor(typeof(ParameterManager))]
public class ParameterManagerInspector : Editor {

	int choice = 0;
	int choiceStrategy = 0;

	public override void OnInspectorGUI() {
		GUILayout.Space (10f);
		EditorGUILayout.LabelField("Test Balance Sheets", EditorStyles.boldLabel);
		choice = Mathf.Min (choice, ((ParameterManager)target).parameterSheets.Count - 1);
		GUILayout.BeginHorizontal("box");
		choice = EditorGUILayout.Popup(choice, ((ParameterManager)target).parameterSheets.ToArray());
		((ParameterManager)target).sheetIndexToLoad = choice;

		if ( GUILayout.Button("Load", GUILayout.Width(50)) ) { 
			ParameterManager.LoadParameters(((ParameterManager)target).parameterSheets[ ((ParameterManager)target).sheetIndexToLoad ]);
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.LabelField("Test Strategies", EditorStyles.boldLabel);
		choiceStrategy = Mathf.Min (choiceStrategy, ((ParameterManager)target).strategies.Count - 1);
		GUILayout.BeginHorizontal("box");
		choiceStrategy = EditorGUILayout.Popup(choiceStrategy, ((ParameterManager)target).strategies.ToArray());
		((ParameterManager)target).strategyIndexToLoad = choiceStrategy;

		if ( GUILayout.Button("Load", GUILayout.Width(50)) ) {
			IParametersStrategy[] managerStrategies = ParameterManager.instance.GetComponents<IParametersStrategy>();
			ParameterManager.ProvideStrategy(managerStrategies[((ParameterManager)target).strategyIndexToLoad]);
		}
		GUILayout.EndHorizontal();


		GUI.enabled = true;
		DrawDefaultInspector();

	}
}
#endif