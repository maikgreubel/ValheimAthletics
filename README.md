# Valheim Athletics Skill

based on work by spacechase0 (https://www.nexusmods.com/valheim/mods/30)

It adds a new skill to the game which increase your carry weight depending on level. Your level will raise upon moving arround upon physically demanding.

## Installation

Unzip the contents into your valheim BepInEx/plugin directory. That's it.

## Deinstallation

Remove the files ValheimAthletics.dll and the assets/German.lng, assets/English.lng and assets/skill_athletics.png.

## Remarks

As mentioned above, the basic work has been done by spacechase0. I used the already done work and changed the behaviour of gaining skill progress, which was bit "unatural".

The progress is now postponed to a defined time span of of 5 seconds. You have to overweight carry goods or pulling a wagon to gain progress in order to raise level.

With interface to Mod Config Enforcer (https://www.nexusmods.com/valheim/mods/460) a server-side configuration to clients may be enforced in order to control the 

- MinimumStamina

  The lower bound stamina value where leveling is not possible

- MinimumMoveDirectionMagnitude

  The minimum distance to move based on vector where counter is increased.

- MinimumTimeDifferenceInSeconds

  The minimum number of seconds between level raise

- SkillRaiseInterval

  The level raise interval

You may want to change these values in your configuration file "BepInEx/config/ValheimAthletics.Skill.cfg" either on server or client side.