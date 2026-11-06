# ITU-CS-464-LAB-06-BSCS25017

**Name:** Muhammad Abdullah   .   **Roll number:** BSCS25017   .   **Engine:** Unity 6

A 5-level arena game built Enemies, Obstacles & Power-ups. The player is a rolling ball. Every level has its own layout, objective and difficulty, and difficulty changes from Level 1 to Level 5.

## Levels at a glance

| # | Scene | Name | Objective | Main threats | Timer | Lives | Power-ups |
|---|---|---|---|---|---|---|---|
| 1 | `Level01` | Warm-Up Yard | Collect 8 coins | 1 slow patrolling guard, 3 spike strips | none | 3 | speed |
| 2 | `Level02` | Spike Run | Reach the goal before time runs out | 3 crushers, 6 spike strips | 40 s | 3 | speed |
| 3 | `Level03` | Guard House | Collect 10 coins without getting caught | 2 guards that patrol and chase | none | 3 | shield |
| 4 | `Level04` | Crusher Grid | Survive until the timer ends (coins are bonus) | 9 crushers firing in a wave, spike ring | 40 s | 3 | shield, magnet (appear mid-run) |
| 5 | `Level05` | The Gauntlet | Collect 12 coins to unlock the exit, then reach it | 2 fast guards, crusher bridge over spikes, chaser waves | 90 s | 2 | shield, speed |

## How each level differs

1. **Warm-Up Yard** is an open 20x20 arena. It is like a tutorial: roll, collect coins, avoid one slow guard and a few spikes.
2. **Spike Run** swaps coins for a race. An S-shaped corridor leads to a goal pad, with spike strips and crushers across the lane. A speed boost halfway helps with the clock.
3. **Guard House** is a cross-shaped map: a central hub with four side rooms, one coin pair in each. Guards chase you when they see you, and door walls block their view so you can hide. A touch costs one heart, then the guard retreats to its waypoint and pauses.
4. **Crusher Grid** changes the goal to survival. A 3x3 grid of crushers covers the floor and fires on a diagonal wave, with spikes around the edge. There is no safe spot, so you hop from pad to pad. The shield and magnet appear during the run, and coins are optional bonuses.
5. **The Gauntlet** combines everything in three rooms. Room A has guards, Room B a crusher bridge over spikes, and Room C waves of chasers. The exit pad stays red until all 12 coins are collected, and you only have 2 lives. It is like the crux of the whole build.

## Difficulty progression

- **Guards:** 1 slow guard, none, 2 chasing guards, none, 2 faster chasing guards.
- **Crushers:** none, 3, none, 9, 3.
- **Timer:** none, 40 s, none, 40 s, 90 s.
- **Lives:** 3, 3, 3, 3, 2.
- **Objective type:** collect, race, evade, survive, then collect-and-escape.

## Scripts (`Assets/Scripts`)

| Script | Purpose | Added in |
|---|---|---|
| `PlayerController`, `Health`, `GameManager`, `Coin` | Movement, hearts and i-frames, score, lives, win/lose, coins | Level 1 |
| `EnemyPatrol`, `Hazard`, `PowerUp` | Waypoint patrol, damaging triggers (spikes, crushers), speed and shield pickups | Level 1 |
| `GoalZone`, timer in `GameManager` | Goal pad and countdown | Level 2 |
| `EnemyGuard` | Patrol, chase and return state machine with line of sight and hit-then-retreat | Level 3 |
| `Magnet`, `TimedPickups`, survive mode in `GameManager` | Coin magnet, pickups that appear mid-run, win by surviving | Level 4 |
| `Chaser`, `WaveSpawner`, `ActivateOnEnter`, locked exit in `GameManager` | Wave enemies, room-triggered spawner, exit that needs all coins | Level 5 |

`Hazard` gained a `startDelay` for staggering crushers (Level 4), and `GameManager` grew a little with each level while keeping the earlier levels working.

## Controls

- **Move:** WASD or arrow keys (legacy Input Manager).
- **Restart / Next Level:** buttons on the win and lose panels.

## Project layout

```
Assets/
  Scenes/     Level01 ... Level05
  Scripts/
  Prefabs/    Coin, Crusher, Chaser
  Materials/
```
