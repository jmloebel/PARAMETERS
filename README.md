# PARAMETERS
PARAMETERS - A serious game for teaching game design (reimplementation)

This is an educational tool. See GitHub Project for instructions and setup.

- Copyright 2024 Jochen Koubek (Universität Bayreuth) and Jens-Martin Loebel (Hochschule Magdeburg-Stendal)
- Redesigned from provided ActionScript source code Copyright by Yoshio Ishii (Nekogames)

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


# Parameters Remake
[http://nekogames.jp/index2en.html]

* パラメータ:Parameters - Klikacz Tygodnia [https://www.youtube.com/watch?v=Loy3fRiIGP8]
* Speedrun: [https://www.youtube.com/watch?v=_enRQeNqlOg]
* Beschreibung: [http://almostidle.com/game/parameters]
* Diskussion: [https://www.reddit.com/r/WebGames/comments/t53u3/parameters_abstract_rpg_where_everything_is/]
* Game Design Review: [http://gamedesignreviews.com/scrapbook/parameters/]


## How to Play
* Complete the game by defeating every enemy
* Money ($) and EXP are collected by mousing over them

## About Item
* Keys can be earned by defeating enemies, or can be purchased in the shop
* You can buy weapons and armor for ATK and DEF bonuses, up to 9 of each individual type.

## About parameter
* RCV determines how fast you recover life, when not fighting enemies, and activity points (ACT), when not on a mission
* ATK determines the strength of your attacks
* DEF determines how much damage you take from enemies

## UI
* Level:
..* $: Can be used at shops to increase you attributes, status or buy keys.
..* Grey Keys: Can be used to unlock a section with a grey lock. Every enemy defeated yields 2 or more grey keys.
..* Gold Keys: Can be used to unlock special sections with gold locks. Obtained at random from enemies.

* EXP . /100
* LIFE ./100
* ACT. ./50 

* RCV.
* ATK.
* DEF.

## Textausgabe der aktuellen Aktion mit Timestamp

## NEKOGAMES: Buchstaben sind Achievements:
*     Level 49, RCV 425, ATK 401, DEF 624, LIFE 200, ACT 175, second to last boss

Collect/Combo Bar (Blau, die sich beim Looten pro Item füllt und dabei Cyan wird)
The combo bar is goes up a bit, each time you pick up money, and it is reset when you dont pickup any money for a certain amount of time (~2 sec)
https://gaming.stackexchange.com/questions/65145/what-happens-in-nekogames-parameters-when-the-mysterious-beat-bar-fills-up

## Map
Das Level besteht aus insgesamt 108 Räumen, die als Rechtecke unterschiedlicher Größe visualisiert werden.
Details finden sich hier
https://medienwissenschaft.uni-bayreuth.de/wolke/index.php/s/o24JsxEPJyyr3rh

Eine kompakte Beschreibung gibt es bei JayIsGames im Parameters Walkthrough:
https://jayisgames.com/review/parameters.php

## Raumtypen (Objekte)
Monster: Hintergrundfarbe gelb, die Lebenspunkte (LP) des Monsters werden angezeigt, z.B. 126/126. Schwerere Monsterräume sind verschlossen.

### Missions: Raum ohne Hintergrund mit Anzeige 0%. 

Händler (Raumnr): Gekennzeichnet mit $-Preisen 
    Armor (DEF): 26, 62, 69
    Waffen (ATK): 15, 42, 48, 73,
    Schlüssel; 96, 
    
Bonus (Goldenes Schloss)
4: AKT++
51: Life Recovery
54: Add Param (+1 Skill Pt)
64: Life++

Geheime Räume (Fragezeichen, grüne Kist in der unteren, rechten Ecke)
Es gibt fünf grüne Schatzkisten. Green chests open after completing certain objectives:
Fill the blue bar by collecting a lot of money at once (oben rechts, bringt Lv x 200$)
Collect the NEKOGAMES letters
Buy one of every weapon / By buying 1x of the most expensive armor and weapon
By beating the bottom boss a second time to reveal the copyright.
By beating the black square which changes color and which has ridiculous amount of defense.

Slot Machine (78)
Kostet Geld, drei zufällige Zeichen müssen übereinstimmen.
Bringt Geld. Jackpot: A bunch of $777 pops out
Slot Machine: Reach level 40 to unlock the slot machine. The payouts are roughly as follows:

@@@ = 200 exp

$$$ = $1,400 payout

*** = $9,600 payout

777 = $15,000 payout

Your chance of winning a prize is 1/16, and every prize is worth more than your average spend to win it.

High Defense Boss (76)
Ein schwarzes Feld, das die Farbe ändert und dadurch sehr oft geklickt werden kann

## Aktionen
Unlock Mission / Monster
Ein verschlossener Raum muss mit einem silbernen oder goldenen Schlüssel geöffnet werden

## Run Mission 
vgl. http://www.progresswars.com/
Ein Klick auf einen offenen Questraum kostet ACT, bringt aber EXP und $, gleichzeitig füllt sich die Prozentzahl
Amount that's complete depends on how many action points you had when you clicked.
Sind nicht genügend ACT vorhanden, ist der Run nicht erfolgreich
Eine abgeschlossene Mission gibt einen SkillPoint. 
Ein Mission, die zu 100% beendet wurde, kann noch ein bisschen Geld abwerfen (Grinding)
Die Anzahl von EXP und $ korreliert evtl. mit der Raumgröße

## Angreifen
Ein Klick auf einen offenen Raum mit einem Monster greift ein Monster an. 
Die Schadenswerte werden zufällig bestimmt

Der Monsterschaden hängt ab von Spielerstärke (ATK) und Monstergröße
Der Spielerschaden hängt ab von der Monstergröße und Spielerverteidigung (DEF)

Monster heilen sehr schnell

## Kaufen
Ein Klick in einen Shop verringert die $ und fügt das Item hinzu
$96 -> +3, +2 DEF
$90 -> +3, +2 ATK
Gleichzeitig wird LIFE auf den Maximalwert gesetzt.

## Items  sammeln (mousing over)
Mousing over Dollar oder EXP erhöht den jeweiligen Wert
XP: Grüne Zahlen
$: Gelbe Zahlen mit $-Zeichen

## Aufleveln
Wir beginnen in Level 1. Sobald ausreichend EXP gesammelt sind, wird ein neuer Level erreicht. Die für den nächsten Level benötigten EXP berechnen sich nach folgender Formel: 100 + Level^2

Mit jedem Level werden 3 Skillpoints hinzugefügt.

## Ende
* Das Spiel ist gewonnen, sobald der Endboss besiegt ist. Quests müssen dafür nicht abgeschlossen werden
