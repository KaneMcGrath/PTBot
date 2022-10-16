# PTBot 1.64

A Custom Titan AI for the Attack on Titan Tribute Game

[Download Latest Version](https://github.com/KaneMcGrath/PTBot/releases/download/1.64/PTBot.1.64.zip)

Follow updates and future mod releases on the Custom Games Mod Discord server

https://discord.gg/BgaBuhT

## About

PTBot is a mod that adds a custom titan that behavies like a player titan.  You can customize the difficulty and the moves that the titan uses

To get started host a room on a multiplayer server or in offline mode.  Then press the quick menu hotkey (Default is "L") to bring up the quick menu where you will have accsess to all of the settings of this mod.

I would reccomend a titan size of 1.0,2.0,3.0,4.0, or 5.0.  This mod uses sampled hitbox data form each of these titan sizes, different sizes will have this data scaled to match.  The scaling is suprisingly close but can mess up certain moves more than others(Especially Jump).

## PTBot Settings

On the first page you will find the most important settings for configuring PTBot

### Titan Name

This is the name of the titan that will show up in the kill feed when it kills you or it is killed

### Difficulty

This only affects how accurate the PTBot predictions are.  At lower difficulties It will miss alot of moves or throw them out too early or to late.  You can see this effect with the Debug Predictions setting.

### Moves

this is a list of all of the available moves, select which ones you want to use then hit "Apply" at the bottom.
these are the internal names that the titan controller uses, just know that **Attack** is Punch, **AttackII** is Slam, and **choptl** and **choptr** are slap left and right.  When you click "Apply" it will update the moveset of all spawned titans.

### Pruning Level

This Setting is to help with performance, every players movement is predicted for each sampled hitbox.  These hitboxes were sampled at a high framerate and overlap eachother.  Pruning removes a number of hitboxes from the sampled data so they dont have to be calculated.  A pruning level of 2 will keep 1 out of every 2 hitboxes, A pruning level of 3 will keep 1 out of every 3 hitboxes and so on.
Once a prunning level is input make sure to hit "Apply" to the right of it.

## Game Settings

This page contains more general settings related to hosting a room with PTBots

### Send Join Message

When a player joins your room they are sent a message with a link to the mod and what your current ping is.  This setting enables or disables sending that message.

### Endless Spawning

This will spawn a certain amount of PTBots. when one dies another will immediatly spawn.  Set the amount you want spawned in the field below and click "Apply".
Titans will only respawn when the number of total titans including other types is below the count.

### Replace Normal Titans

This will replace any titan spawned by your game with a PTBot.  You can use this for wave modes or on custom maps.

### Teleport Titans Away From Spawn

This only works on The Forest and The City.  When a titan gets too close to the spawn position, it is teleported to the other side of the map.
This will also teleport Player Titans.

### Debug Raycasts

Shows the raycast points that PTBot uses for pathing.

### Debug Targets

Shows the player a PTBot is targeting when running towards or away from a player

### Debug Predictions

Shows the Prediction of each player a PTBot is targeting for each hitbox.  you can use this to see what affect difficulty has on predictions

## Config

use this menu to save and load your settings.  Settings are stored in "PTBot_Data\PTBot\Config.txt".  At the top of this config file you can change the default Quickmenu hotkey.  you can set the hotkey to any key from this list https://docs.unity3d.com/ScriptReference/KeyCode.html

# Source Code
This mod is forked from the RCMod 5/5/2022 Update https://github.com/rc174945/RCMod By Ricecake

Almoast all of my changes are under the PTBot namespace

This mod is open source and you are welcome to include or improve any parts of it in your own mods. As long as credit is given.  I didnt use any external dependancies and everything is fairly self contained, so if you know how to mod the game, it should not be too difficult to implement.

