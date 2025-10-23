# ChromaDash
First Unity 2D Project

```csharp
🗂️ A. 코어 시스템 (Core Systems & Managers)  
[Core]  
 |  
 |- 📂 Managers (Singletons)  
 |   |- GameManager.cs  
 |   |- InputManager.cs  
 |   |- EffectManager.cs  
 |   |- PoolManager.cs  
 |   └─ AudioManager.cs  
 |  
 |- 📡 Event Hub  
 |   └─ GameEvents.cs  
 |  
 └─ 🛠️ Definitions  
     └─ GameEnums.cs  
  
⚙️ B. 상태 기계 엔진 (HFSM Core Engine)  
[StateMachineCore]  
 |  
 |- 📜 IState.cs (Interface)  
 |  
 |- ⚙️ StateMachine.cs  
 |   └─ 📄 Transition.cs (Data Class)  
 |  
 └─ 🏃 PlayerState.cs (Abstract Base Class)  
     (└─ Implements: IState)  
  
🏃 C. 플레이어 (Player & HFSM Implementation)  
[Player]  
 |  
 └─ 🎮 PlayerController.cs (Context)  
     |  
     |- ⚙️ StateMachine (Instance - Root)  
     |  
     |- ⏳ TimeGauge.cs (Component)  
     |  
     └─  Hierarchy (PlayerStates Namespace)  
         |  
         └─ 📜 PlayerState.cs (Abstract Base Class)  
             |  
             |- ⚙️ StateMachine (Instance - Sub-State Machine)  
             |  
             └─ Implements 📜 IState  
                 |  
                 ├─ 👟 PlayerGroundedState (Super-State)  
                 │   └─ (Manages Sub-States like PlayerRunningState)  
                 |  
                 ├─ 🦅 PlayerAirborneState (Super-State)  
                 │   ├─ PlayerJumpingState (Sub-State)  
                 │   └─ PlayerFallingState (Sub-State)  
                 |  
                 └─ (Other States: PlayerDeadState, etc.)  
  
🌍 D. 월드 (World & Procedural Generation)  
[World]  
 |  
 |- 🗺️ LevelGenerator.cs  
 |   └─ (Spawns) 📦 MapSegment.cs  
 |  
 └─ 📦 MapSegment.cs  
     └─ (Contains) 🧱 Platform.cs  
  
🎨 E. UI (Event Subscribers)  
[UI]  
 |  
 |- (Subscribes to 📡 GameEvents)  
 |   ├─ 📊 TimeGaugeUI.cs  
 |   └─ 💯 ScoreUI.cs  
 |  
 └─ (참고: UI 스크립트들은 Core나 Player를 직접 참조하지 않음)  
 ```