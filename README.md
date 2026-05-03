# Maze Hunter

> A solo project for a 3d game course at VTC Academy

This is a third-person action game built in Unity 6.0. In the game, the player has to navigate through a procedurally generated maze in order to collect all objectives as fast as possible, all while fighting off incoming hordes of zombies.

---

## Gameplay

Preview GIF:
<figure>
  <img src="https://github.com/taan41/Maze-Hunter/blob/main/Assets/Readme/gameplay_preview.gif" width="600" alt="Preview gif">
</figure>

[Gameplay preview on Youtube](https://youtu.be/aGZjZ5nt5Hc)
[Maze generation process on Youtube](https://youtu.be/m9A2ahXin3M)

---

## Features

- **Procedural maze generation:** Depth-first algorithm with configurable grid size, path branching chances, and room priority weighting
- **Dual weapon system:** Melee (sword) and ranged (gun) combat with mechanics like ammo, damage amp. based on combo/hit body part, and VFX such as sword trails, muzzle flashes,...
- **Monster AI:** NavMesh-based AI that detects the player by range and line-of-sight, with close integration of Animator for smooth behavior transitions
- **Minimap:** Dynamic minimap that reveals map objectives as player explores
- **Score system:** Scored by time elapsed, objectives collected, kills, and completion bonus; configurable via ScriptableObject, with top 10 entries persisted locally as JSON
- **Settings menu:** Persistent audio/graphics settings saved via PlayerPrefs

---
## Tech Stack

- **Engine:** Unity 6.0 (URP)
- **Language:** C#
- **Architecture:** Partial classes for player/monster, ScriptableObjects for game rules, singletons for managers, object pooling for monsters/game fx
