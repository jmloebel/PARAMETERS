/*
	PARAMETERS Remake
    Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
    Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

	This is an educational tool. See GitHub Project for instructions and setup.
	
	THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public static class I18N {

	private static Dictionary<string, string> dict;
	private static Dictionary<string, string> dictEN;
    public static string lang;

    [MenuItem("Semantic/English")]
    static void SelectEnglish() {
        WriteSemantic("English");
        Debug.Log("Switch to English");
    }

    [MenuItem("Semantic/Fantasy")]
    static void SelectFantasy() {
        WriteSemantic("Fantasy");
        Debug.Log("Switch to Fantasy");
    }

    [MenuItem("Semantic/German")]
    static void SelectGerman() {
        WriteSemantic("German");
        Debug.Log("Switch to German");
    }

    [MenuItem("Semantic/Abstract")]
    static void SelectAbstract() { 
        WriteSemantic("Abstract");
        Debug.Log("Switch to Abstract");
    }


    static void WriteSemantic(string semantic) {
        string path = "Assets/Resources/I18N/semantic.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(semantic);
        writer.Close();
    }


    static I18N() {
		dict = new Dictionary<string, string>();
		dictEN = new Dictionary<string, string>();
        string path = "Assets/Resources/I18N/semantic.txt";
        StreamReader reader = new StreamReader(path, false);
        lang = reader.ReadLine();
        reader.Close();
        if (lang=="") lang = "English";
        SetLocale(dict, lang);
        SetLocale(dictEN, SystemLanguage.English.ToString()); //fallback
	}

	public static string Localize(string key) {
		if (!dict.ContainsKey (key)) {
			if (dictEN.ContainsKey (key))
				return dictEN [key];
			else
				return key;
		}
		return dict [key];
	}

    public static Sprite GetSprite(string name) {
        if (Resources.Load<Sprite>("Artwork/" + lang + "/" + name))
            return Resources.Load<Sprite>("Artwork/" + lang + "/" + name);
        else
            return Resources.Load<Sprite>("Artwork/English/" + name);
    }

    private static void SetLocale(Dictionary<string, string> loadDict, String lang) {
		loadDict.Clear ();
		TextAsset textAsset = Resources.Load("I18N/" + lang) as TextAsset;
		if (textAsset == null) {
			Debug.LogError ("I18N string resource not found: " + lang + ".txt");
			return;
		}

		string[] lines = textAsset.text.Split('\n'); // new string[] { "\r\n", "\n" }
		string key, value;
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#"))
			{
				key = lines[i].Substring(0, lines[i].IndexOf("="));
				value = lines[i].Substring(lines[i].IndexOf("=") + 1);
				loadDict.Add(key, value);
			}
		}
	}
}
