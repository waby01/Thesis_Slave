# Thesis Slave: ACC is a Myth

## Game Overview
Thesis Slave is a 2D turn-based deckbuilder prototype about the comedic academic struggles of a final-year university student facing their thesis supervisor. The primary core mechanic is managing a hand of cards to reduce the supervisor's "Revision Bar" to zero while maintaining the player's "Mental Health" indicator above zero. 

The player draws cards each turn, queues actions using Stamina points, and resolves outcomes before facing the supervisor's counter-attack. The win condition is achieved by fully clearing the Revision Bar (getting ACC), while the lose condition occurs when the player's Mental Health drops to 0 (getting burnt out).

## How to Run
1. Clone or download this repository.
2. Open the project using Unity.
3. Open the main scene located at `Assets/Scenes/MainMenu.unity`.
4. Press **Play** in the Unity Editor or run the standalone build.

- **Engine & version used:** Unity 6 (6000.0.75f1)
- **Build location:** `/Build/` folder (contains `ThesisSlave.exe`)

## Technical Decisions
- **Finite State Machine (FSM):** Structured the core game loop in `BattleManager.cs` using states (`Start`, `PlayerTurn`, `ExecutingPlayerActions`, `EnemyTurn`, `Win`, `Lose`) to ensure clean state isolation and predictability.
- **Action Queue System:** Card plays are queued into an execution list upon clicking, resolving sequentially during the execution state rather than instantly, enhancing game feel and turn clarity.
- **UI Interaction Locking & Protection:** Implemented conditional interaction flags (`CanClickEndTurn` and `IsAnyPanelActive`) to lock card inputs and disable the "End Turn" button whenever turn panels, pause menus, or overlay panels are active.
- **Singleton Audio Architecture:** Built a persistent `AudioManagers.cs` handling global sound effects and music override states across panel transitions and scene loads.
- **ScriptableObject Data Driven:** Defined card stats and enemy intents via ScriptableObjects (`CardData`, `EnemyData`) to decouple gameplay logic from visual rendering (`SlotCard`).

## What I Would Do With More Time
- **Card Reward & Drafting System:** Implement a reward screen between battle stages to allow players to draft new cards into their deck.
- **Deck Customization / Card Upgrade:** Add mechanics for players to remove obsolete cards or upgrade existing ones to shape their deck synergy.
- **Enhanced Visual Feedback:** Add card play animations, screen shakes on heavy attacks, and dynamic UI particles.
- **Run Progression Map:** Implement a node-based map system to give players choices between different supervisors or academic events.

## Known Issues
- Rapidly clicking cards during scene load transitions might occasionally cause slight UI refresh delays on low-end hardware.
- UI layout scaling is optimized for 16:9 resolutions (1920x1080); other aspect ratios might experience minor visual padding overlaps.