
# Notes on building Turns

## Rules for Turn Structure

https://mtg.fandom.com/wiki/Turn_structure

## Magic Rules Tips

> [...]  If two different effects are trying to give different players an additional turn, which wins? In Magic, the most recently generated extra turn will be taken first. While taking turns does not use the stack, it may be useful to think of turns as if they did. Like spells and abilities on the stack, the top most one resolves and happens first. Normally in a two player game, the ‘turn stack’ simply alternates between the two players. When someone generates an extra turn, this turn goes on top of the stack, it will disrupt to normal pattern and this turn will be taken next. So if two different players generate extra turns, the last effect to resolve and create an extra turn will ‘win’ and be taken first. Then the additional turn that was generated first will be taken. Once that turn is over the game will return to its normal pattern.

[Original Blog Post](https://blogs.magicjudges.org/rulestips/2011/08/how-to-deal-with-extra-turns/)

## Basic Workflow

Maybe don't explicitly check for these triggers and prefer the triggers call Game Object methods to accomlish what's needed.

#### Note:

All "skip" effects should be implemented with _Replacement Effects_.

[Rule 614.1(b)/614.10](https://mtg.fandom.com/wiki/Replacement_effect)
> 614.1. Some continuous effects are replacement effects. Like prevention effects (see rule 615), replacement effects apply continuously as events happen—they aren’t locked in ahead of time. Such effects watch for a particular event that would happen and completely or partially replace that event with a different event. They act like “shields” around whatever they’re affecting. 

> 614.1b Effects that use the word “skip” are replacement effects. These replacement effects use the word “skip” to indicate what events, steps, phases, or turns will be replaced with nothing.

> 614.10. An effect that causes a player to skip an event, step, phase, or turn is a replacement effect. “Skip [something]” is the same as “Instead of doing [something], do nothing.” Once a step, phase, or turn has started, it can no longer be skipped—any skip effects will wait until the next occurrence.

> 614.10a Anything scheduled for a skipped step, phase, or turn won’t happen. Anything scheduled for the “next” occurrence of something waits for the first occurrence that isn’t skipped. If two effects each cause a player to skip their next occurrence, that player must skip the next two; one effect will be satisfied in skipping the first occurrence, while the other will remain until another occurrence can be skipped.

> 614.10b Some effects cause a player to skip a step, phase, or turn, then take another action. That action is considered to be the first thing that happens during the next step, phase, or turn to actually occur.


## Test Cases

### Time Walk

Take an extra turn after this one.

### Stasis

Players skip their untap step.

### Response // Resurgence (Resurgence half, Sorcery)

After this main phase, there is an additional combat phase followed by an additional main phase.

### Teferi, Time Raveler / Time Walk

- Player 2 has Teferi, Time Raveler w/+1 Static Effect active
- Player 1 creates Time Walk Effect in MainPhase1
- Player 2 (responds?) creates a Time Walk Effect
