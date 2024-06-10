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
	SkillManager class manages player stats upgrades and increases player-selected stats by a specified range
	</summary>
*/
public class SkillManager : MonoBehaviour {

	[Header ("Skill Settings")]
	public int addRecoveryMin;
	public int addRecoveryMax;
	public int addAttackMin, addAttackMax;
	public int addDefenseMin, addDefenseMax;
	public int addLifeMin, addLifeMax;
	public int addActionMin, addActionMax;


	public void AddRCV() {
		int skillPoints = Random.Range (addRecoveryMin, addRecoveryMax + 1);
		Player.instance.recovery += skillPoints;
		EventManager.instance.WriteEventMsg (I18N.Localize("RCV.> +" + skillPoints), "#00FFFF");
		Player.instance.AddSkillPoints(-1);
	}

	public void AddATK() {
		int skillPoints = Random.Range (addAttackMin, addAttackMax + 1);
		Player.instance.attack += skillPoints;
		Player.instance.attackMax += skillPoints;
		EventManager.instance.WriteEventMsg (I18N.Localize("ATK.> +") + skillPoints, "#FF6097");
		Player.instance.AddSkillPoints(-1);
	}

	public void AddDEF() {
		int skillPoints = Random.Range (addDefenseMin, addDefenseMax + 1);
		Player.instance.defense += skillPoints;
		Player.instance.defenseMax += skillPoints;
		EventManager.instance.WriteEventMsg (I18N.Localize("DEF.> +") + skillPoints, "#33cc66");
		Player.instance.AddSkillPoints(-1);
	}

	public void AddLIFE() {
		int skillPoints = Random.Range (addLifeMin, addLifeMax + 1);
		Player.instance.life += skillPoints;
		Player.instance.lifeMax += skillPoints;
		EventManager.instance.WriteEventMsg (I18N.Localize("LIFE.> +" + skillPoints), "#FFCC00");
		Player.instance.AddSkillPoints(-1);
	}

	public void AddACT() {
		int skillPoints = Random.Range (addActionMin, addActionMax + 1);
		Player.instance.action += skillPoints;
		Player.instance.actionMax += skillPoints;
		EventManager.instance.WriteEventMsg (I18N.Localize("ACT.> +" + skillPoints), "#FF00FF");
		Player.instance.AddSkillPoints(-1);
	}

}
