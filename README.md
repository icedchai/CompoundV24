# Compound V24 (Temp V) plugin!
Adds 1 custom item: Compound V24!

On usage: applies random "superpower" to player. After two powers are applied, a sound effect (if specified "hallway") plays to all players globally.

**Requires [AUDIOPLAYERAPI](https://github.com/Killers0992/AudioPlayerApi) to function**

## Superpower API

Each superpower has an ability attached to it as well as innate qualities, such as damage multipliers. Superpowers must be registered into `PowerManager.Instance`.

## Available powers
- Laser Vision (hurts players who you look at)
- Controlled Superspeed (allows you to activate and deactivate at will)
  - Kill anyone you run into at superspeed!
- Uncontrolled Superspeed (allows you to activate, but not deactivate. Risks a deadly heart attack which you won't survive unless you have adrenaline)