## Rando settings

- Weapons/armor replaced by similar ones of same class (could break balance and make the game easier).
- Weapons/armor replaced by any other weapon/armor.
- Randomly place weapons/armor over the levels.
- Randomize enemies (worms -> iron golems would be fun).
- Randomly place enemies over the levels.
- Randomize everyone's disposition.
- Randomize item's intrinsic cost. Maybe appraise would become a useful skill?
- Randomize fountain effects (No more retreating to the healing fountains in lvl2!)
- No starting bag. You'll have to find the automap by yourself (oof)

Anyway, I was daydreaming a few cool rando settings.

- Ranger's dream: melee weapons replaced by ranged weapons/bows.
- Penance: no weapons or money available.
- Starvation mode: Food reduced by 1/n (variable n), hidden Ylem rune for create food (In Mani Ylem?), no fishing rods.
- Russian roulette: All weapons and armor receive random bonuses/curses. (Does an npc's weapon affect its attack? Facing a bandit with firedoom axe/black sword is definitely frightening!)
- Merchant's mercy: Weapon and food are only available as barter goods. You'll have to scramble for money to get any progress done. Devious if combined with an item cost modifier.
- Invisible horrors: All monsters become dire ghosts.
- Lockdown: all doors are locked. Lockpicking or bashing only.
- Let's play a game: Place traps at random spots.
- Dislexia: Inspired by that setting in the Quest 64 rando. Randomly changes one vowel in a name into another.
- Peaceful mode: no hostiles.

## Logic idea

* Create some bitmaps with the levels. Enough so I can actually see where there's open tiles or close tiles.
* Paint over these bitmaps with some colors, manually, subdividing the levels into rooms. Ideally, each room would be composed of open tiles that can be accessed
  without any item requirements. This would include teleports and staircases/holes. I've thought about doing this programatically, but checking it later and having
  to program in all the edge cases would be quite cumbersome.
* Each color would correspond to an id. Then, by looking at the stuff between the rooms, I would manually create a link between two rooms and the item required
  to cross the "threshold". Easiest example: locked, massive door. Can't go around it. Some links can be nothing, and links can be one-way.
* This will effectively transform the whole map into a large graph with nodes. Then, by choosing a starting node (normally the entrance), map a way to the ending
  node (room with the slasher) by going to and from nodes. Perhaps add weights so the direction chosen can be controlled (i.e. favor new areas, not )
* When a path is chosen, it'll be composed of a series of steps. Then, by iterating through each step, a memory of the accessible locations is stored. The key
  item to get to the next step will be placed in any spot of the accessible locations. Perhaps weights to choose more recent locations can be added.
* After that, nonessential items are placed according to whatever logic. Perhaps weigh their usefulness according to the depth (which can also be set when
  doing the manual mapping), or just jumble them together.