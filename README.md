# Better Bases

Mod for The Long Dark that allows players to improve their bases in multiple ways:
* Most furniture can be moved (if you have cleared it of clutter first).
* Most broken cabinets and drawers can be fixed.
* Most of the clutter on interiors can be removed.
* Some of it can also be moved!

Better Bases comes from merging two old Wulfmarius mods (HomeImprovement and BetterPlacing) with RemoveClutter, in order to have a single, big mod that allows for a lot of base customization.

## Custom definitions to add or modify objects that can be removed
You can add .json files (properly formatted) in the bb-custom-definitions (this will be created on first run) and the game will read them and apply the definitions to objects so they can be broken down.
Definitions specified here will overwrite the default ones, so if you want to change the yield for something, the tool that is required or anything else, you can do it.

You can read a detailed tutorial on how to create new item definitions or overwrite existing ones [here](./Tutorial.md)

## Installation
* Read the [installation instructions on the modlist](https://xpazeman.com/tld-mod-list/install.html)

## Changelog

### v1.1.0
* NEW: Added a system to add custom definitions so new objects can be added to the clutter pool, or modify existing ones.
* FIX: Fixed an error thrown on quitting

### v1.0.1
* FIX: Fixed error with not loading removed clutter data properly on some interiors

### v1.0.0
* Initial release of BetterBases
* Updated to work with TLD 2.16

### v0.3.0 BETA
* Added settings to toggle all of the mod's features on or off

### v0.2.1 BETA
* Changed the iteration code so it is faster while preparing the objects. Now you don't have time to cook a full turkey while waiting for some interiors to load, sorry!

### v0.2.0 BETA
Some bugfixes:
* Paper debris removal was resetting on some locations, I gave them a stern reprimand and they should stay harvested now.
* Some floating objects appeared in some scenes, this was leftovers from design that were hidden by Hinterland but the mod made them visible again. After a bit of convincing, they stay hidden now.
* Some doors and trucks weren't working properly. They are stubborn and won't budge, so until I find a better fix, doors of any kind are not removable anymore.
* The disable small items option wasn't playing nice when the player was outside, it has now been beaten into submission and now knows how to behave in such circumstances.
* Some objects were sneakily moving away from their original position with each load. I found a heavy hammer and nailed them into place. This fix might have the unintended consequence of breaking other furniture though, so if someone finds a shelf floating half a meter above the ground let me know so I can nail them down too.

### v0.1.0 BETA
Initial BETA release