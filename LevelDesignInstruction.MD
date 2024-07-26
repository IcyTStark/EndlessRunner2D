# Level Design Instructions

This document provides instructions on how to design and customize levels using the `ProceduralTerrainGenerator` script.

## Overview

The `ProceduralTerrainGenerator` script is responsible for generating an endless 2D terrain in Unity. It uses a Perlin noise algorithm to create hills and valleys and dynamically adds new terrain segments as the player progresses. The script also handles the placement of obstacles and power-ups on the terrain.

## Components

- **SpriteShapeController:** Controls the shape and appearance of the generated terrain.
- **Transform (Player Reference):** Tracks the player's position to determine when new terrain segments should be generated.

## Terrain Generation Settings

- **_initialLevelLength:** Initial length of the terrain.
- **_xMultiplier:** Controls the horizontal distance between terrain points.
- **_yMultiplier:** Controls the vertical variation of the terrain.
- **_curveSmoothness:** Determines the smoothness of the terrain curves.
- **_noiseStep:** Step size for Perlin noise generation.
- **_bottomDepth:** Depth of the bottom points to create a base for the terrain.

## Dynamic Generation Settings

- **_playerDistanceThreshold:** Distance threshold to trigger new terrain generation.
- **_newSegmentsPerGeneration:** Number of new terrain segments to generate at a time.

## Obstacle Controllers

- **_objectPooler:** Manages pooled objects for obstacle spawning.
- **_randomSpawnIndex:** List of indices for obstacle placement.
- **_activeObstacles:** List of currently active obstacles.
- **_obstacleSpawnCountPerGeneration:** Number of obstacles to spawn per generation.

## Power-Up Controllers

- **_randomSpawnIndexForPowerUps:** Index for power-up placement.
- **_activePowerUps:** List of currently active power-ups.
- **_powerUpSpawnCountPerGeneration:** Number of power-ups to spawn per generation.

## How to Customize

### 1. Initial Terrain Generation

The initial terrain is generated based on the `_initialLevelLength` value. You can customize the terrain's appearance by adjusting the `_xMultiplier`, `_yMultiplier`, `_curveSmoothness`, and `_noiseStep` values in the Unity Inspector.

### 2. Dynamic Terrain Generation

The script dynamically generates new terrain segments when the player gets close to the end of the current terrain. Adjust the `_playerDistanceThreshold` and `_newSegmentsPerGeneration` values to control when and how much new terrain is generated.

### 3. Obstacle Placement

Obstacles are placed at random indices within the generated terrain. The number of obstacles per generation is controlled by `_obstacleSpawnCountPerGeneration`. You can customize the obstacle prefabs and their placement logic by modifying the `GenerateObstacles` method.

### 4. Power-Up Placement

Power-ups are placed similarly to obstacles but with separate logic. The number of power-ups per generation is controlled by `_powerUpSpawnCountPerGeneration`. Customize the power-up prefabs and their placement logic in the `GeneratePowerUps` method.

### 5. Control Scheme Switching

The `GameManager` script allows you to switch between different control schemes (PC and Mobile). This can be adjusted within the Unity Editor by modifying the relevant options in the `GameManager`.

## Code Breakdown

### Initialization

The `Initialize` method sets up the initial terrain and generates obstacles. It can be called with a callback for game reset scenarios.

### Terrain Point Addition

The `AddTerrainPoint` method calculates and adds new terrain points based on Perlin noise and current settings. It also sets tangents for smooth curves.

### Terrain Segment Generation

The `CheckAndGenerateNewSegment` method monitors the player's position and triggers new terrain generation when necessary. The `GenerateNewSegment` coroutine handles the creation of new terrain points and updates bottom points.

### Obstacle and Power-Up Generation

The `GenerateObstacles` and `GeneratePowerUps` methods handle the placement of obstacles and power-ups on the terrain. They use object pooling for efficient management of game objects.

### Removing Previous Points

The `RemovePreviousPoints` method removes old terrain points to make room for new ones. This ensures the terrain remains within a manageable size.

### Debugging

The `OnDrawGizmos` method draws gizmos in the Unity Editor to visualize the end of the current terrain.
