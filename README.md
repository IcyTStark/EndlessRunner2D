# Santa's Adventure

## Overview
Santa's Adventure is an endless 2D runner game where players control Santa as he navigates a snowy landscape filled with hills and valleys. Santa can jump over and slide under obstacles such as rocks. The objective is to survive as long as possible.

## Game Mechanics

### Controls
- **Jump:** Swipe Up
- **Slide:** Swipe Down

### Gameplay
- **Endless Running:** Santa continuously runs forward on a hilly terrain.
- **Jumping:** Players can make Santa jump to avoid rocks.
- **Sliding:** Players can make Santa slide to go under floating rocks.

## Obstacles
- **Rocks:** Rocks appear at random intervals and can be jumped over or slid under.

## Power-ups
- **Shield:** Gives Temporary Inivincibility Effect.

## Scoring
Score is given based on the distance traveled.

### Key Components
1. **PlayerController:** Handles input for jumping and sliding.
2. **ObjectPooler:** Class that is responsible for pooling object and returning it to TerrainManager when asked
3. **ProceduralTerrainGeneration:** Generates points with hill and valleys using perlin noise to sprite shape controller.
4. **ScoreManager:** Tracks and updates the player's score based on distance and presents collected.
4. **GameManager:** Handles Game State and communicates to other classes

## Art and Audio

### Art Style
- 2D cartoon-style graphics with snowy theme.
