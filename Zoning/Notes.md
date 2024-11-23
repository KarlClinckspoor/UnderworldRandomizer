# Zoning

This file documents the "zones" of UW1, which means areas where you can move in "freely", without spells, glitches or bashing open doors (because that'd be annoying to constantly do). The main reason for this is to add some logic to the randomizer so that, for instance, a key item (typically a key itself) can't be placed behind its own locked door.

## Methodology

The lev.ark files were loaded and the tilemap was simplified and labeled. A label value of 0 means solid walls, a label value of 1 means an open tile, but with an unspecified area. These png files are then loaded into Gimp, transformed into an indexed image with the glasbey lookup table/palette (from ImageJ). The main reason for choosing this table was to provide very distinct colors, making the visualization of zones easier. Then, get a pencil with a pixel size of 1, select the color from the palette, and starting painting the areas, following UWE (krokots' Underworld Editor). Whenever you find a locked door that isn't trivially easy to open, you start a new section. If two sections meet, then the lower label value (likely placed first) will replace the other label, since that means you could've walked from one to the other, but discretion is advised. After painting an area, you have to save the image as a bmp, then either load into Python and convert the values into labels *again* using the lookup table, or load into imageJ, which, for whatever reason, can recognize it's labels, and then save it as a raw or binary file, which is then loaded back into Python. This will be transformed into lots of `Point`s in the main program, which will characterize the zones for the randomizer program.

The rest of the file are notes of the zoning. Part of this will become part of the traversal logic.

## Files

* `glasbey.lut`. Glasbey color lookup table. Format is 256 RGB values, bit depth of 8.
* `mapX.bin`. Raw binary files containing the initial zone labels. X is the level number. Order is ud-flipped so they match the maps.
* `mapX.png`. Png files converted from the raw binary files, with Python.
* `mapX.xcf`. Gimp files containing the indexed images.
* `mapX.bmp`. Files saved from gimp with the labeled images.

## Level 1

Status: **Mapping out level finished**. **Needs to map "leaks" to lvl2**. **Needs revision**.

### Zone 53 starting area

Turns out I completely missed the lock to the shrine of the silver sapling. So now I'm shoehorning in this area number.

Connected to:
* [Zone 2](#zone-2-main-level-1-areas). Door to sapling room, ID1, door to corridor, ID1.
* [Zone 10](#zone-10-close-to-outcasts). Door near bragit, ID1.

Keys found:

* ID1 (in backpack close to bragit)

### Zone 2: main Level 1 areas

Key IDs found:
* ID2 (1 on SE, close to acid slug, another on spiral stairs close to access to lvl2, NW)

Connected to:
* [Zone 53](#zone-53-starting-area), door to silver sapling  room, ID1, door to corridor, ID1.
* [Zone 3](#zone-3-gray-goblin-area), gray goblin area, by gray goblin doorman (Eb). Unbreakable portcullis.
* **Lvl 2, coords X 35 Y 20** (access in alcove, jumping gap SW of central pillar). Ultimately zone 2 wraps around to this area so, despite the lock with key ID2, it can be circumvented.
* **Lvl 2, coords X 7  Y 38** (spiral staircase, NWW of central pillar)
* [Zone 5](#zone-5-little-storage-area-with-mongbat), Key ID2. Breakable?
* [Zone 6](#zone-6-green-goblin-area), green goblin doorman (Drog).
* [Zone 8](#zone-8-skeleroom) Key ID2. Breakable?
* [Zone 9](#zone-9-pillar-height-puzzle-control-room-and-treasure). Key ID2.
* [Zone 10](#zone-10-close-to-outcasts). Door near water, breakable. Levitating up viewing area.

Other notes:

* The water below the shrine can be considered a softlock in a way, since bashing open the door becomes a necessity (and bashing doors isn't "accepted"). However, if you complete the jumping puzzle, you can continue, so this was considered a single zone.

### Zone 3: Gray goblin area

Status: **Unfinished!** Need to populate lvl2. Is secret door breakable?

Key IDs:
* ID3. With gray goblin Retichall.

Connected to:
* [Zone 2](#zone-2-main-level-1-areas). Button on wall close to portcullis.
* [Zone 4](#zone-4-gray-goblin-storage). Gray goblin storage. Key ID3. Angers goblins.
* [Zone 2](#zone-2-main-level-1-areas). Puzzle area through locked secret door, key ID3. Breakable?
* **Lvl 2, coords X 47 Y 53**, fountain area with lurker and tons of gray goblins. Is [Zone 2](#zone-2-continuing-from-lvl1)

Other notes: 
* Another storage there, easily opened by the button. Is it possible to get stuck in there if the pole isn't there?

### Zone 4: Gray goblin storage

Status: finished

Connected to:
* [Zone 3](#zone-3-gray-goblin-area): Gray goblin area. Key ID3. Breakable. Angers goblins.

### Zone 5: Little storage area, with mongbat

Status: **unfinished**. Is door breakable?

Connected to:
* [Zone 2](#zone-2-main-level-1-areas). Key ID2. Breakable?

### Zone 6: Green goblin area

Status: Finished

Key IDs found:

* ID4. With green goblin doorman Drog.
* ID4. With green goblin king Vernix.

Connected to:
* [Zone 7](#zone-7-green-goblin-storage). Key ID4. Breakable. Angers goblins.

### Zone 7: Green goblin storage

Status: Finished

Connected to:
* [Zone 6](#zone-6-green-goblin-area). Key ID4. Breakable. Angers goblins.

### Zone 8: Skeleroom

Status: **unfinished**. Breakable doors?

Connected to:
* [Zone 2](#zone-2-main-level-1-areas). Key ID2. Two doors. Breakable?.

### Zone 9: Pillar height puzzle control room and treasure

Status: finished.

Connected to:

* [Zone 2](#zone-2-main-level-1-areas). Key ID3, or falling down.

### Zone 10: Close to Outcasts

Status: finished.

Connected to:

* [Zone 2](#zone-2-main-level-1-areas). Key ID1. Breakable.
* [Zone 2](#zone-2-main-level-1-areas). Locked door that's triggered by a lever inside Zone 10 itself. Breakable.
* [Zone 2](#zone-2-main-level-1-areas). Falling down from the viewing area (or levitating up).
* [Zone 11](#zone-11-outcasts). Key ID1. Breakable.

### Zone 11: Outcasts

Status: Finished

Connected to:
* [Zone 10](#zone-10-close-to-outcasts). Key ID1. Breakable.

### Lvl1 Other notes.
* Color is very similar to zone 2, but it's a slightly different shade of green.


## Lvl 2

Status: **Unfinished**

### Zone 2 (continuing from Lvl1)

Status: **unfinished** This zone is connected to a one-way passage, which I labeled [Zone 19](#zone-19-one-way-passage-undertow). Will this reconnect back to Zone 2 eventually?

Connected to:

* **Lvl1, X 8 Y 34** (spiral stairs) [Zone2](#zone-2-main-level-1-areas)
* **Lvl1, X 26 Y 20** (stairs after river [Zone2](#zone-2-main-level-1-areas))
* [Zone 12](#zone-12-mountainfolk). Unbreakable portcullis, Key ID6, Brawnclan doorman.
* [Zone 12](#zone-12-mountainfolk). Unbreakable door, key ID5.
* [Zone 19](#zone-19-one-way-passage-undertow). One-way passage.
* [Zone 20](#zone-20-crystal-ball-room-with-skeletons). By floating up the viewing area or triggering the lever with a pole (?). Unbreakable portcullis.
* **Lvl3, X 42 Y 61** Northern area that splits in two, EW.
* [Zone 21](#zone-21-maze-room-with-potions) with breakable door, KeyID 6.
* [Zone 22](#zone-22-area-reached-by-flying) by flying/levitating ONLY.
* [Zone 83](#zone-83-moonorb-area), after you have gate traveled there.

Keys:

* Key ID5 (near stairs, S of central shaft.)
* Key ID6 (after zigzag maze with damaging floor)

Notes:

* There is the gazer in the north area of the mines. Can it be accessed without a pickaxe? If not, **add another zone here**.
* One-way connection from Lvl1, gray goblin area ([Zone 3](#zone-3-gray-goblin-area)).

### Zone 12: Mountainfolk

Connected to:

* [Zone 2](#zone-2-continuing-from-lvl1). Unbreakable portcullis, Key ID6, button on wall.
* [Zone 2](#zone-2-continuing-from-lvl1). Unbreakable(?) door, key ID5.
* [Zone 13](#zone-13-mountainfolk-storage-1). Key ID5. Breakable door.
* [Zone 14](#zone-14-mountainfolk-storage-2). Key ID5. Breakable door.
* [Zone 15](#zone-15-mountainfolk-stairs). Key ID5. Breakable door.
* [Zone 16](#zone-16-royal-bedroom). Key ID5. Breakable door.
* [Zone 17](#zone-17-vault). Password deco morono, Steeltoe doorman. Unbreakable door.

### Zone 13: Mountainfolk storage 1

Connected to:

* [Zone 12](#zone-12-mountainfolk). Key ID5. Breakable door. Apparently no one would get angry if bashed.

### Zone 14: Mountainfolk storage 2

* [Zone 12](#zone-12-mountainfolk). Key ID5. Breakable door. Apparently no one would get angry if bashed.

### Zone 15: Mountainfolk stairs

* [Zone 12](#zone-12-mountainfolk). Key ID5. Breakable door. Apparently no one would get angry if bashed.
* **Lvl 3, X 5 Y 2**, SW area close to area where you get poisoned if you step on something. [Zone 2](#zone-2-continuing-from-lvl2)
* **TODO: If you can get to this zone through zone 2 without requirements, wouldn't this become zone 2?**

### Zone 16: Royal bedroom.

* [Zone 12](#zone-12-mountainfolk). Key ID5. Breakable door. Apparently no one would get angry if bashed.

### Zone 17: Vault

* [Zone 12](#zone-12-mountainfolk). Unbreakable door. Unopenable from this side. To leave, would need to silver sapling out.

### Zone 19: One-way passage, undertow.

Is accessed by a one-way passage from Zone 2, and leads to lvl3.

* **Lvl3, X25 Y 10**. South, in water, near bones. [Zone 2](#zone-2-continuing-from-lvl2).
* Not named Zone 18 because the color of 18 is practically the same as 0 (background).

### Zone 20: Crystal ball room with skeletons

* [Zone 2](#zone-2-continuing-from-lvl1) through a lever that opens an unbreakable portcullis or falling down the viewing area.

Can be used to peer into the moonstone area.

### Zone 21: Maze room with potions

* [Zone 2](#zone-2-continuing-from-lvl1) with breakable door, Key ID6.

### Zone 22: Area reached by flying

* [Zone 2](#zone-2-continuing-from-lvl1) by flying of falling down with headlesses
* [Zone 23](#zone-23-locked-area-of-alcove) by breakable(?) door with Key ID6.

### Zone 23: Locked area of alcove

* [Zone 22](#zone-22-area-reached-by-flying) by breakable(?) door with Key ID6.

### Zone 83 Moonorb area

* [Zone 2](#zone-2-continuing-from-lvl1), by moving to a specific tile.
* Can be accessed by any area if you have a gate travel spell.

## Lvl3

### Zone 2 (continuing from Lvl2)

Status: **Incomplete**. There's still the area with key of courage that you go from the lower levels.

Connected to:

* **Lvl2, X42 Y 59** Northern area close to gray goblins.
* [Zone 24](#zone-24-alcove-behind-waterfall). What's the requirement?
* [Zone 25](#zone-25-sword-blade-area). Requires removing the vines and triggering the lever. Nothing else.
* [Zone 26](#zone-26-red-lizarman-room). breakable(?) door Key ID9.
* [Zone 27](#zone-27-lizardmen-empty-storage-room). Unbreakable door Key ID9.
* [Zone 28](#zone-28-isslek-room). Breakable door Key ID9.
* [Zone 29](#zone-29-shrine-with-lizardmen). Unbreakable door Key ID9.
* **Lvl 2, X 5 Y 3** Near goldthirst, zone [Zone 15](#zone-15-mountainfolk-stairs). **TODO: Wouldn't this make Zone 15 be Zone 2**?
* **Lvl 4, X 42 Y 2** Near trolls. **TODO fill out**.
* [Zone 30](#zone-30-storage-room-west). Breakable door, key ID8.
* [Zone 31](#zone-31-storage-room-east). Unbreakable door, key ID8.
* [Zone 33](#zone-33-corridor-with-skeles). Breakable door, no key, needs to be bashed or picked.
* [Zone 34](#zone-34-murgos-prison). Seetharee doorman, unbreakable portcullis, needs to solve quest.
* [Zone 35](#zone-35-lizardmen-sw-quarters). breakable locked door, key ID11.
* [Zone 36](#zone-36-lizardmen-se-quarters). Unbreakable locked door, key ID11.
* **Lvl4, X 23 Y 36** Near pit with lurker and weird knight area.

Other notes:
* To the East, there is a locked door, unbreakable, key ID9. Can it be circumvented by swimming to the south? Do the currents allow?
* The eastern part has a higher water level, and there are two waterfalls from it to the western, lower area. But in both cases you can circle around using the northwest path and reach the eastern area.
* There is an unbreakable locked door to the SW, which you have to either pull the lever from the east, or push the buttons in the right sequence in the west. Nevertheless, you can go around the long way if needed, so it's still the same zone.
* Cup of wonder area is accessible here. Artifact!
* There is a teleport from the lower area to the inner tunnels, in the north.
* There is a room with loot behind a secret door in the north, I'm assuming it's accessed pretty normally.
* There is a locked door, key ID11, that guards the threshold between gray and other lizardmen, but it can be circumvented by going the long way around.
* Zak has the taper. Artifact!

### Zone 24: alcove behind waterfall

Status: **Incomplete**

Connected to:
* [Zone 2](#zone-2-continuing-from-lvl2)

What's the trigger? Do I just move in?

### Zone 25: Sword blade area

* Impossible to move from it to Zone 2, since trigger is only in zone 2.
* Contains one of the artifacts.

### Zone 26: Red lizarman room

* [Zone 2](#zone-2-continuing-from-lvl2), breakable(?) door Key ID9.

### Zone 27: Lizardmen empty storage room

* [Zone 2](#zone-2-continuing-from-lvl2), unbreakable door Key ID9.

### Zone 28: Iss'lek room

* [Zone 2](#zone-2-continuing-from-lvl2), breakable(?) door Key ID9.

### Zone 29: Shrine with lizardmen

* [Zone 2](#zone-2-continuing-from-lvl2), unbreakable door Key ID9.

### Zone 30: Storage room west.

* [Zone 2](#zone-2-continuing-from-lvl2), key ID8, breakable door.

### Zone 31: Storage room east.

* [Zone 2](#zone-2-continuing-from-lvl2), key ID8, unbreakable door.

### Zone 32: Secret bandit room

* [Zone 31](#zone-31-storage-room-east), through unbreakable secret door. **TODO: Confirm - what's the trigger? Can I just open it?**

### Zone 33: Corridor with skeles

* [Zone 2](#zone-2-continuing-from-lvl2), through a locked door with no key, needs to be bashed or picked.

### Zone 34: Murgo's prison

* [Zone 2](#zone-2-continuing-from-lvl2), through freeing him (ssetharee doorman).

### Zone 35: Lizardmen, SW quarters

* [Zone 2](#zone-2-continuing-from-lvl2), key ID11, breakable door.

### Zone 36: Lizardmen, SE quarters

* [Zone 2](#zone-2-continuing-from-lvl2), key ID11, unbreakable door.

### Zone 37: Lizardmen, Quarters NE

* [Zone 2](#zone-2-continuing-from-lvl2), key ID11, unbreakable door.

### Zone 38: Lizardmen, Quarters N

* [Zone 2](#zone-2-continuing-from-lvl2), key ID11, unbreakable door.

### Zone 39: Lizardmen, Quarters NW

* [Zone 2](#zone-2-continuing-from-lvl2), key ID11, unbreakable door.

### Zone 66 (continuing from lvl4)

* **Lvl 4 X 7 Y 61** and **Lvl4 X 7 Y 50** to area with wisp
* [Zone 67](#zone-67-shadow-beasts) behind a breakable door with no key
* [Zone 68](#zone-68-key-of-courage-room) behind an unbreakable door with key ID10

### Zone 67 shadow beasts

* [Zone 66](#zone-66-continuing-from-lvl4), breakable door with no key.

Has Key ID10.

### Zone 68 Key of courage room

* [Zone 66](#zone-66-continuing-from-lvl4), unbreakable door with key ID10.

Has key of courage. Artifact!

## Lvl 4

Status: **unfinished**. Need to paint in the area to the northwest.

### Zone 2 continuing from lvl3

Status: **Unfinished**

Connected to:

* **Lvl 3 X 41 Y 2** Near fighters to the south, still [Zone 2](#zone-2-continuing-from-lvl2).
* **Lvl 3 X 25 Y 36** Close to lizardmen, still [Zone 2](#zone-2-continuing-from-lvl2)
* [Zone 40](#zone-40-rawstag-ankh-shrine), massive unbreakable door, rawstag doorman.
* [Zone 41](#zone-41-alcove-after-lever-puzzle-e), needs to have a pole or levitation (or jump?) to trigger the level.
* [Zone 43](#zone-43-bullfrog-exit-se). Requires levitation/flying or solving the bullfrog puzzle. Wand (555) is enchanted with the spell to reset the puzzle.
* **Lvl 5 X 35 Y 47** and **Lvl 5 X 28 Y 47**, north of dining hall.
* [Zone 44](#zone-44-maze-west-rodrick), unbreakable massive door with key ID13.
* **Lvl 5 X 39 Y 26** near ghouls.
* [Zone 46](#zone-46-waterfall-area), needs levitation or waterwalking+jumping.
* [Zone 47](#zone-47-waterfall-area-north-step), needs levitation or waterwalking+jumping.
* [Zone 48](#zone-48-knight-armory), dorna is the doorman. Locked massive unbreakable door with no key.
* Bullfrog exit NE **Lvl 5 X 54 Y 59** leads to tombs. Since the tombs can be accessed through zone 2, this makes the corridor zone 2, too (not 42).
<!-- * [Zone 42](#zone-42-bullfrog-exit-ne). Requires levitation/flying or solving the bullfrog puzzle. Wand (555) is enchanted with the spell to reset the puzzle. -->

Has:

* Key ID13 with Rodrick

Other notes:

### Zone 40 rawstag ankh shrine

* [Zone 2](#zone-2-continuing-from-lvl3), massive unbreakable door, can't be opened from this side.

### Zone 41 alcove after lever puzzle, E

* [Zone 2](#zone-2-continuing-from-lvl3), but can't be triggered from this area.

### Zone 42 Bullfrog exit NE (removed)

* [Zone 2](#zone-2-continuing-from-lvl3), falling down to bullfrog

### Zone 43 Bullfrog exit SE

* [Zone 2](#zone-2-continuing-from-lvl3), falling down to bullfrog

### Zone 44 Maze west rodrick

* [Zone 2](#zone-2-continuing-from-lvl3), unbreakable massive door with key ID13.
* [Zone 45](#zone-45-plate-secret-area), lever puzzle

### Zone 45 Plate secret area

* [Zone 44](#zone-44-maze-west-rodrick), but is inaccessible from here. Massive unbreakable door.

### Zone 46 waterfall area

* [Zone 2](#zone-2-continuing-from-lvl3), falling down.
* [Zone 47](#zone-47-waterfall-area-north-step), falling down.

### Zone 47 waterfall area north step

* [Zone 2](#zone-2-continuing-from-lvl3), falling down
* [Zone 46](#zone-46-waterfall-area), levitation or flying or waterwalk+jumping.

### Zone 48 knight armory

* [Zone 2](#zone-2-continuing-from-lvl3), through a lever. Door is massive, unbreakable and doesn't have a key associated.

### Zone 66 (continuing from lvl5)

* **Lvl5 X 8 Y 45** area with traps and room
* **Lvl3 X 7 Y 59** and **Lvl3 X 7 Y 52** swamp area with key of courage

## Lvl 5

Status: **unfinished** needs to fill in area to the NW where you climb up.

### Zone 2: continuing from lvl4

Connected to:

* **Lvl4 X 30 Y 47** and **Lvl4 X 34 Y 47** near rodrick. [Zone 2](#zone-2-continuing-from-lvl3)
* **Lvl4 X 41 Y 26** near outcasts. [Zone 2](#zone-2-continuing-from-lvl3)
* **Lvl4 X 56 Y 59** bullfrog exit NE.
* **Lvl6 X 43 Y 27** near lava in center. **TODO fill in**

* [Zone 49](#zone-49-zanium-mine-dispatch-corridor), needs code from Kneenibble, otherwise it's free access. Teleport is behind unbreakable portcullis.
* [Zone 51](#zone-51-nolant-tomb-vestibule), lever

* Ring is to the NNW. Artifact!
* Judy is here, and has the key of love if you show her a picture of Tom. **TODO** should this be "technically" a new zone, connected to 2?
* Tombs can be accessed by going through the ghoul area, then finding and opening the secret door, which doesn't have a lock.
* Sword hilt is found in the tombs. Artifact!
* Grave of Garamon is found here too, near a light mace.

### Zone 49 Zanium mine dispatch corridor

* [Zone 50](#zone-50-zanium-mine), from teleport

Connection to Zone 2 is dependent on the portcullis being open, so it's technically one-way.

### Zone 50 Zanium mine

* [Zone 2](#zone-2-continuing-from-lvl4), one-way teleport

### Zone 51 Nolant tomb vestibule

* [Zone 2](#zone-2-continuing-from-lvl4), unbreakable portcullis, key ID15.
* [Zone 52](#zone-52-nolant-tomb), massive door, key ID15.

Has: a trigger that makes a ghost appear, which has key ID15.

### Zone 52 Nolant tomb

* [Zone 51](#zone-51-nolant-tomb-vestibule), massive door, key ID15.

### Zone 66 (continuing from lvl6)

* **Lvl6 X 11 Y 55** lava corridor
* **Lvl4 X 8 Y 43** area with wisps

## Lvl 6

Status: **incomplete**

### Zone 2 (continuing from lvl5)

* **Lvl 5 X 43 Y 31** near the central pillar, south of dining area.
* [Zone 54](#zone-54-lava-south), ramp to the south.
* **Lvl 7 X 19 Y 13**, typical entrance near Cardon.
* [Zone 55](#zone-55-lava-north), multiple entrances and exits
* [Zone 56](#zone-56-small-alcove-on-lava-east), falling down from square room with lava walls and gold floor.
* [Zone 57](#zone-57-alcove-with-crown-near-bridge) if you jump well. **TODO** test.
* [Zone 58](#zone-58-alcove-with-shield-near-crown), levitation.

Contains:

* Book of honesty. Artifact!
* Shield of valor with Golem. Artifact!
* Wine. Artifact!

### Zone 54 lava south

* [Zone 2](#zone-2-continuing-from-lvl5), climbing up some stairs.

### Zone 55 Lava north

* [Zone 2](#zone-2-continuing-from-lvl5), multiple entrances and exits.
* [Zone 56](#zone-56-small-alcove-on-lava-east), jumping up a little.
* [Zone 57](#zone-57-alcove-with-crown-near-bridge), by a jump spell (**TODO test**) or floating.
* [Zone 58](#zone-58-alcove-with-shield-near-crown), levitating up.

### Zone 56 small alcove on lava, East

* [Zone 2](#zone-2-continuing-from-lvl5), floating up.
* [Zone 55](#zone-55-lava-north), falling down a little

### Zone 57 Alcove with crown near bridge

* [Zone 55](#zone-55-lava-north), falling down
* Possibly [Zone 2](#zone-2-continuing-from-lvl5), if you jump well. **TODO** test

### Zone 58 Alcove with shield, near crown

* [Zone 2](#zone-2-continuing-from-lvl5), levitation.
* [Zone 55](#zone-55-lava-north), falling down.

### Zone 66 (continuing from lvl7)

* **Lvl7 X 2 Y 61** secret passage behind prison
* **Lvl5 X 14 Y 55** corridor and room with traps

## Lvl 7

There's a single square on the north of the map that I think is inaccessible, so I left it unassigned. Same with the central pillar.

### Zone 2 (continuing form lvl6)

* **Lvl 6 X 18 Y 10** near mages, after bridge.

I swore I went through the backdoor to Tyball at least once, but I couldn't replicate it. What's the trigger?

Connected to:

* [Zone 59](#zone-59-guard-room-1), through an indestructible portcullis which can, at first, be opened by having a medallion or engaging in combat with everyone.
* [Zone 71](#zone-71-tyball-crib), through a secret wall **TODO: that is perhaps opened if you have a scroll of reveal?**
* [Zone 69](#zone-69-entrance-to-damaging-maze) through a locked breakable door, key ID24.
* [Zone 75](#zone-75-tombs-p1), if you previously opened the way of the secret door. **TODO if you have a scroll of reveal, will this open too?**.
* [Zone 82](#zone-82-top-of-waterfall), levitation or flying.

Items:

* Medallion that allows you to go through the guard gates.

### Zone 59 Guard room 1

* [Zone 2](#zone-2-continuing-form-lvl6), potentially by a lever. **TODO the editor doesn't say you can go back, but it might be possible**
* [Zone 60](#zone-60-spider-maze), by having talked to the goblin with the talisman or fought through the guardpost.

### Zone 60 spider maze

* **Lvl8 X12 Y33**, sectioned off area with some treasure.
* **Lvl8 X60 Y62**, area with orb rock.

* [Zone 59](#zone-59-guard-room-1), if you fought through the guards, instead of presenting the medallion. Or key ID23
* [Zone 61](#zone-61-guardroom-2), if you fight through or have medallion.
* [Zone 63](#zone-63-prison-corridor), if you opened it up from Dantes' room.
* [Zone 67](#zone-67-alcove-with-gate-travel-scroll), if you can jump or can levitate.
* [Zone 79](#zone-79-northern-lava-river), falling down.
* [Zone 75](#zone-75-tombs-p1), if you have the Kallistan crystal.

Has:

Key ID25, near a spider den
Key ID24, below a skull near cavern with dead things.


### Zone 61 Guardroom 2

* [Zone 60](#zone-60-spider-maze), by key ID23 or talking to goblins
* [Zone 62](#zone-62-prison-foyer), by key ID23

Key ID23 on troll.

Apparently you get imprisoned if you go here. You need some treasure to get the troll to open the door.

### Zone 62 Prison foyer

* [Zone 61](#zone-61-guardroom-2), key ID23.
* [Zone 63](#zone-63-prison-corridor), 

### Zone 63 Prison corridor

* [Zone 62](#zone-62-prison-foyer) through a portcullis, key ID23.
* [Zone 64](#zone-64-prison-cells-id23), several prison cells with breakable doors with key ID23.
* [Zone 65](#zone-65-prison-cells-id27), three prison cells with unbreakable doors with key ID27.

### Zone 64 Prison cells ID23

* [Zone 62](#zone-63-prison-corridor), through breakable doors with key ID23.
* [Zone 60](#zone-60-spider-maze), through a hole through Dantes room. This makes the troll guard hostile.

Fintor, griffle, kallistan, dantes

Kallistan has crystal that opens up passages in this level, but you need the password deco morono.

### Zone 65 Prison cells ID27

* [Zone 63](#zone-63-prison-corridor), through unbreakable massive doors with key ID27.

Smonden (Key ID26 which goes up to key of courage), Bolinard (Tom picture which goes to key of love), Gurstang which has the phrase for Illomo, to find the key of Truth.

### Zone 66 Up to key of courage

* [Zone 60](#zone-60-spider-maze), chasms, unbreakable door with key ID26.
* **Lvl 6 X 5 Y 61** lava corridor.

### Zone 67 Alcove with gate travel scroll

* [Zone 60](#zone-60-spider-maze), falling down.

### Zone 68 Cavern of dead things

* [Zone 60](#zone-60-spider-maze), through a breakable door with no key
* [Zone 69](#zone-69-entrance-to-damaging-maze), through a breakable door with no key

### Zone 69 entrance to damaging maze

* [Zone 68](#zone-68-cavern-of-dead-things), through a breakable door with no key
* [Zone 70](#zone-70-damaging-maze), just walk, but would need the crown.
* [Zone 78](#zone-78-passage-to-imp), if you opened the way **TODO will a reveal spell open this?**
* [Zone 81](#zone-81-ring-of-levitation-and-misc-steps), falling down or levitation.
* [Zone 79](#zone-79-northern-lava-river), falling down.

### Zone 70 Damaging maze

* Need the crown of maze navigation (found in [zone 78](#zone-78-passage-to-imp)) to travel safely, but can be memorized.
* [Zone 79](#zone-79-northern-lava-river), falling down.
* [Zone 69](#zone-69-entrance-to-damaging-maze) just walk.
* [Zone 71](#zone-71-tyball-crib)

### Zone 71 Tyball crib

* [Zone 70](#zone-70-damaging-maze), just walk
* [Zone 72](#zone-72-arielle-prison), unbreakable portcullis with key ID27
* [Zone 2](#zone-2-continuing-form-lvl6), by a secret wall.
* [Zone 73](#zone-73-corridor-to-stairs-into-main-lvl8), through an unbreakable door with key ID27.

Tyball has key ID23 and key ID27.

### Zone 72 Arial prison

* [Zone 71](#zone-71-tyball-crib), unbreakable portcullis with key ID27.

### Zone 73 corridor to stairs into main lvl8

* [Zone 71](#zone-71-tyball-crib) through an unbreakable door with key ID27.
* **Lvl 8 X 59 Y 2**, southeastern area.

### Zone 74 guard room 3

* [Zone 2](#zone-2-continuing-form-lvl6), if you have key ID23 (from west, locked massive door), talking to goblin and having talisman (east) or fighting your way through.

### Zone 75 tombs P1

* [Zone 60](#zone-60-spider-maze), if it was previously opened with the crystal **TODO** will this open with reveal?
* [Zone 2](#zone-2-continuing-form-lvl6), by a secret door to the bottom.
* [Zone 76](#zone-76-tombs-p2), by finding a secret door on NE tomb.
**TODO some rocks fall behind you, cutting off access to zone 60. Add a new intermediate zone?**

### Zone 76 tombs P2

* [Zone 75](#zone-75-tombs-p1), if you previously opened the path. **TODO** will this open with reveal?
* [Zone 77](#zone-77-puke-corridor) by finding the secret door.

### Zone 77 puke corridor

* [Zone 76](#zone-76-tombs-p2), if you found the secret path. **TODO** Will this open with reveal?
* [Zone 78](#zone-78-passage-to-imp), breakable door with key ID25.

### Zone 78 passage to imp

* [Zone 77](#zone-77-puke-corridor), breakable door with key ID25.
* [Zone 79](#zone-79-northern-lava-river), breakable door with key ID25.
* **Lvl 8 X 38 Y 47** small central region.
* **Lvl 8 X 35 Y 44** small central region, other side.
* [Zone 69](#zone-69-entrance-to-damaging-maze), through a secret door to the south.
* [Zone 80](#zone-80-alcoves-with-imp), levitating or flying.

Contains: crown of maze navigation.

### Zone 79 northern lava river

* [Zone 78](#zone-78-passage-to-imp), breakable door with key ID25.
* [Zone 81](#zone-81-ring-of-levitation-and-misc-steps), jumping up or levitation.
* [Zone 69](#zone-69-entrance-to-damaging-maze), floating up.
* [Zone 69](#zone-60-spider-maze), floating up or climbing up at the northwest area.

Floor is lava and has lots and lots and lots of fire elementals.

### Zone 80 alcoves with imp

* [Zone 78](#zone-78-passage-to-imp), falling down.

### Zone 81 Ring of levitation and misc steps

* [Zone 79](#zone-79-northern-lava-river), falling down
* [Zone 69](#zone-69-entrance-to-damaging-maze), flying or levitation

### Zone 82 Top of waterfall

Single square zone that would be funny to add something.

* [Zone 2](#zone-2-continuing-form-lvl6), falling down.

## Lvl 8

### Zone 60 (continuing from lvl7) small sectioned off mine

* **Lvl 7 X 10 Y 33**, spider maze start
* [Zone 89](#zone-89-lava-corridors-in-western-section), just walk.

### Zone 60 (continuing from lvl7) area with orb rock

* **Lvl 7 X 58 Y 54**, near plaque of evil dead
* **Lvl 7 X 56 Y 62**, near gate travel scroll
* [Zone 88](#zone-88-small-lava-eastern-section) just walk, but it has lava.

### Zone 73 (continued from lvl7)

* **Lvl 7 X 61 Y 2** near tyball's crib.
* [Zone 83](#zone-83-carasso), breakable door with no key


### Zone 78 (continued from lvl7)

* **Lvl 7 X 38 Y 50** staircase that leads to puke corridor
* **Lvl 7 X 35 Y 39** imp with crown of maze navigation.

### Zone 83 Carasso

* [Zone 73](#zone-73-continued-from-lvl7), breakable door with no key

Has key ID28

### Zone 84 Corridors to central chamber

* [Zone 73](#zone-73-continued-from-lvl7), unbreakable door with key ID28 and perhaps jumping from the southern bit.
* [Zone 85](#zone-85-lava-and-connected-regions), falling down.
* [Zone 86](#zone-86-slasher-chamber), need key of love, courage and truth, tripartite key.

### Zone 85 Lava and connected regions

* [Zone 84](#zone-84-corridors-to-central-chamber), going up
* [Zone 73](#zone-73-continued-from-lvl7), lots of ways. Ramps, flying, etc

Needs lava boots, lots of fire elementals

### Zone 86 Slasher chamber

* [Zone 84](#84-corridors-to-central-chamber), if you opened the door previously.
* [Zone 87](#zone-87-final-stretch), if you have all 8 talismans.

### Zone 88 Small lava eastern section

* [Zone 60](#zone-60-continuing-from-lvl7-area-with-orb-rock), just walking

Lava area

### Zone 89 Lava corridors in western section

* [Zone 60](#zone-60-continuing-from-lvl7-small-sectioned-off-mine), just walk

### Zone 

## Lvl 9

### Zone 87 final stretch

* No way back, only forward. Traditionally, green path.