# PTBot 1.8

A Custom Titan AI for the Attack on Titan Tribute Game

[Download Latest Version](https://github.com/KaneMcGrath/PTBot/releases/download/1.8/PTBot.1.8.zip)

Follow updates and future mod releases on the Custom Games Mod Discord server

https://discord.gg/BgaBuhT

## About

The original goal of PTBot is to add a custom titan that behaves like a player titan.  But with recent additions it is now more of a customizable boss fight.  You are able to customize the difficulty, speed, and moves that the titan uses.

To get started, host a room on a multiplayer server or in offline mode.  Then press the quick menu hotkey (Default is "L") to bring up the quick menu where you will have access to all of the settings of this mod.

I would recommend a titan size of 1.0,2.0,3.0,4.0,5.0, or 6.0.  This mod uses sampled hitbox data from each of these titan sizes, different sizes will have this data scaled to match.  The scaling is surprisingly close but can mess up certain moves more than others(Especially Jump).

## PTBot Settings

On the first page you will find the most important settings for configuring PTBot

### Titan Name

This is the name of the titan that will show up in the kill feed when it kills you or it is killed

### Difficulty

This only affects how accurate the PTBot predictions are.  At lower difficulties It will miss alot of moves or throw them out too early or to late.  You can see this effect with the Debug Predictions setting.

### Edit Moves

This button will open the Moveset Control Window, which lets you edit each attack that PTBot can do.  You can find more info in the moveset control window section [below](https://github.com/KaneMcGrath/PTBot/edit/main/README.md#moveset-control-window)

### Pruning Level

This Setting is to help with performance, every player's movement is predicted for each sampled hitbox.  These hitboxes were sampled at a high frame rate and overlap each other.  Pruning removes a number of hitboxes from the sampled data so they don't have to be calculated.  A pruning level of 2 will keep 1 out of every 2 hitboxes, A pruning level of 3 will keep 1 out of every 3 hitboxes and so on.
Once a pruning level is input make sure to hit "Apply" to the right of it.

## Game Settings

This page contains more general settings related to hosting a room with PTBots

### Send Join Message

When a player joins your room they are sent a message with a link to the mod and what your current ping is.  This setting enables or disables sending that message.

### Endless Spawning

This will spawn a certain amount of PTBots. when one dies another will immediately spawn.  Set the amount you want spawned in the field below and click "Apply".
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

## Moveset Control Window

This window lists every attack that BTBot can use and gives you a few options to use them.  All the moves are in a scrolling list, so be sure to scroll down with the scroll bar on the side of the window, or with the mouse wheel

### Enable

This will tell the Titan whether to use or not use this attack
any attacks that are not enabled will not be calculated and save performance, so if you are having lag problems try lowering the number of enabled attacks.

### Start At

Set the start time of a move (in Seconds) to make a move skip forward in the animation and come out quicker.
Warning!  Setting StartAt too high on certain moves can cause spamming explosions which will disconnect you from the game.  These are moves like slam or slap face most of these moves are around 2 seconds in length and you should be safe setting it around 1 second.  
Try out moves in offline mode first if you are unsure.

# Source Code
This mod is forked from the RCMod 5/5/2022 Update https://github.com/rc174945/RCMod By Ricecake

Almost all of my changes are under the PTBot namespace

This mod is open source and you are welcome to include or improve any parts of it in your own mods. As long as credit is given.  I didn't use any external dependencies and everything is fairly self contained, so if you know how to mod the game, it should not be too difficult to implement.
