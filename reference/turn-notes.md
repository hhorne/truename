
# Notes on building Turns

## Rules for Turn Structure

https://mtg.fandom.com/wiki/Turn_structure

## Basic Workflow

Maybe don't explicitly check for these triggers and prefer the triggers call Game Object methods to accomlish what's needed.

Check _Triggers_ for _Skip Turn_
Do _Turn_ unless _Skip Turn_ found
Check _Triggers_ for _Skip Phase_
Do _Phase_ unless _Skip Phase_ found
Check _Triggers_ for _Skip Step_
Do [Step] unless [Skip Step] found
Check [Triggers] for [Extra Step]
Repeat for each [Step] in [Phase]
Check [Triggers] for [Extra Phase]
Repeat for each [Phase] in [Turn]
EndOfTurn check [Triggers] for [Extra Turn]

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
