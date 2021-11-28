# truename

replicating the mechanics and game play of 🧙‍♀️✨magic🧙‍♂️

## Plan

three phases to the implementation plan:

- 🔳 rules engine / unit tests (in progress)
- 🔲 client that accepts game state represented by json and resolves any pending interactions
- 🔲 user oriented game client

## Getting Started

`truename` is build using v6 of `dotnet`.

### Commands

From the root directory:

#### Build

`dotnet build src`

#### Run Tests

`dotnet test src`

#### Run the Console Test Harness

`dotnet run --project src/truename.Console`


## Resources

* https://scryfall.com/docs/syntax - search magic cards
* https://github.com/magefree/mage - a feature complete implementation of magic in java

### Wiki
* https://mtg.fandom.com/wiki/Object 
* https://mtg.fandom.com/wiki/Layer

# Layer System: Interesting Cards + Interactions
* [Urza's Saga](https://scryfall.com/card/mh2/259/urzas-saga)
* [Thespian's Stage](https://scryfall.com/card/2xm/327/thespians-stage)
* [Blood Moon](https://scryfall.com/card/2xm/118/blood-moon)

While Blood Moon turns either Saga or Stage into a `Basic Mountain` the way layers and granted abilities work means they do not lose their notable abilities. Ideal for testing complex interactions in the layer system.

# Turns
* [Turns.md](reference\turn-notes.md)