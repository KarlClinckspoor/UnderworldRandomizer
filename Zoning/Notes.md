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
* **Lvl 2, coords X 35 Y 20** (access in alcove, jumping gap SW of central pillar)
* **Lvl 2, coords X 7  Y 38** (spiral staircase, NWW of central pillar)
* [Zone 5](#zone-5-little-storage-area-with-mongbat), Key ID2. Breakable?
* [Zone 6](#zone-6-green-goblin-area), green goblin doorman (Drog).
* [Zone 8](#zone-8-skeleroom) Key ID2. Breakable?
* [Zone 9](#zone-9-pillar-height-puzzle-control-room-and-treasure). Key ID2.
* [Zone 10](#zone-10-close-to-outcasts). Door near water, breakable. Door near bragit, ID1. Levitating up viewing area.

Other notes:

* Key ID1 can make the area quicker to travel, but isn't necessary to unlock the doors.


### Zone 3: Gray goblin area

Status: **Unfinished!** Need to populate lvl2. Is secret door breakable?

Key IDs:
* ID3. With gray goblin Retichall.

Connected to:
* [Zone 2](#zone-2-main-level-1-areas). Button on wall close to portcullis.
* [Zone 4](#zone-4-gray-goblin-storage). Gray goblin storage. Key ID3. Angers goblins.
* [Zone 2](#zone-2-main-level-1-areas). Puzzle area through locked secret door, key ID3. Breakable?
* **Lvl 2, coords X 47 Y 53**

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

Other notes:
* Color is very similar to zone 2, but it's a slightly different shade of green.