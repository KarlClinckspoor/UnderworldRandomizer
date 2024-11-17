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

### Zone 2: main Level 1 areas

Status: **Unfinished!** need to populate lvl2. If I go from the backside, can I get the door with the lever open?

Key IDs found:
* ID1 (in backpack close to bragit)
* ID2 (1 on SE, close to acid slug, another on spiral stairs close to access to lvl2, NW)

Connected to:
* [Zone 3](#zone-3-gray-goblin-area), gray goblin area, by gray goblin doorman (Eb). Unbreakable portcullis.
* **Lvl 2, coords X 35 Y 20** (access in alcove, jumping gap SW of central pillar). Ultimately zone 2 wraps around to this area so, despite the lock with key ID2, it can be circumvented.
* **Lvl 2, coords X 7  Y 38** (spiral staircase, NWW of central pillar)
* [Zone 5](#zone-5-little-storage-area-with-mongbat), Key ID2. Breakable?
* [Zone 6](#zone-6-green-goblin-area), green goblin doorman (Drog).
* [Zone 8](#zone-8-skeleroom) Key ID2. Breakable?
* [Zone 9](#zone-9-pillar-height-puzzle-control-room-and-treasure). Key ID2.
* [Zone 10](#zone-10-close-to-outcasts). Door near water, breakable. Door near bragit, ID1. Levitating up viewing area.

Other notes:

* Key ID1 can make the area quicker to travel, but isn't necessary to unlock the doors. The water below the shrine can be considered a softlock in a way, since bashing open the door becomes a necessity (and bashing doors isn't "accepted"). However, if you complete the jumping puzzle, you can continue, so this was considered a single zone.

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

### Lvl2 Other notes

* The moonorb and gate travel scroll area - what leads here?

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

## Lvl 4

### Zone 2 continuing from lvl3

Connected to:

* **Lvl 3 X 41 Y 2** Near fighters to the south, still [Zone 2](#zone-2-continuing-from-lvl2).
* **Lvl 3 X 25 Y 36** Close to lizardmen, still [Zone 2](#zone-2-continuing-from-lvl2)