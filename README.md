# Ninth Circle
**Unity Game Development Project for COMP 376 at Concordia University**

A first-person action game developed as a team project over the course of several months. Ninth Circle was created in under a month as a Final Project submission, featuring complex boss battles, weapon systems, and magical talismans.

## 🎮 Game Overview

Ninth Circle is a challenging first-person action game where players battle through waves of enemies and face off against powerful bosses. The game features a comprehensive weapon system, magical talismans, and dynamic environmental hazards.

### Full Playthrough Video with Commentary
[![Ninth Circle Video](https://img.youtube.com/vi/nqd3PVFwXIo/0.jpg)](https://www.youtube.com/watch?v=nqd3PVFwXIo)

## 👥 Development Team

- [tooLateForTown](https://github.com/tooLateForTown/) - General Programmer, Player Movement
- [mckenzietsean](https://github.com/mckenzietsean) - General Programmer, Art Design and UI
- [ssmith89gg](https://github.com/ssmith89gg) - General Programmer, Level Design
- [nightnimbus](https://github.com/nightnimbus) - General Programmer, Art Design and Sound Design
- [aachernigel](https://github.com/aachernigel) - General Programmer, Enemy Design
- **[AkilTDixon](https://github.com/AkilTDixon) - General Programmer, Weapon Design & Boss Systems**

## 🎯 My Contributions (Assets/Akil/)

All code and systems I developed are located in the `Assets/Akil/` folder. Here's a comprehensive breakdown of my contributions:

### 🏗️ Core Systems Architecture

#### **Player Management System** (`PlayerInfoScript.cs`)
- **Singleton pattern implementation** for global player state management
- **Comprehensive keybind system** with customizable controls
- **Game mode selection** (Easy/Normal difficulty)
- **Player death and damage functionality** with visual feedback
- **Health and ammo management** across all weapon types
- **Talisman system integration** with cooldown management

#### **Game Balance & Progression**
- **Dynamic difficulty scaling** across all gameplay elements
- **Weapon upgrade system** with multiple enhancement paths
- **Enemy spawn balancing** and wave progression
- **Experience and progression mechanics**

### ⚔️ Weapon System (`Assets/Akil/Scripts/`)

#### **Base Weapon Architecture** (`WeaponScript.cs`)
- **Modular weapon system** with inheritance-based design
- **Dynamic damage and fire rate modifiers** with additive/multiplicative scaling
- **Ammunition management** with clip-based reloading
- **Upgrade system** supporting multiple enhancement types
- **Animation integration** with weapon-specific behaviors

#### **Individual Weapon Implementations**

**Rifle System** (`RifleScript.cs`)
- **Raycast-based shooting** with precise hit detection
- **Multishot upgrade** - fires additional projectiles
- **Explosive rounds upgrade** - area-of-effect damage with physics
- **Dynamic muzzle flash effects** synchronized with fire rate
- **Mesh switching** for visual upgrade representation

**RPG System** (`RPGScript.cs`)
- **Projectile-based rocket launcher** with physics simulation
- **Multishot capability** for devastating area damage
- **Explosion mechanics** with force application

**Shotgun System** (`ShotgunScript.cs`)
- **Spread-based shooting** with multiple pellets
- **Charged shot upgrade** - increased damage and range
- **Penetrating rounds** - bullets pass through multiple enemies

**Melee System** (`MeleeScript.cs`)
- **Close-combat weapon** with knockback mechanics
- **Ammo replenishment upgrade** - restores ammunition on hit
- **Knockback enhancement** - increased force application

### 🏆 Boss Battle System

#### **Kaene (First Boss)** (`Boss/Kaene/KaeneScript.cs`)
- **Multi-phase boss design** with distinct attack patterns
- **Orb spawning mechanics** with strategic positioning
- **Channeling system** with visual aura effects
- **Triple projectile attack** with spread patterns
- **Area-of-effect ultimate** with knockback physics
- **Minion spawning** during combat phases
- **Frost debuff system** affecting player movement
- **Dynamic health scaling** based on player max HP

#### **Endenor (Second Boss)** (`Boss/Endenor/EndenorScript.cs`)
- **Advanced AI behavior** with dodge mechanics
- **Teleportation system** with afterimage effects
- **Wind phase mechanics** with environmental hazards
- **Stun system** with escalating thresholds
- **NavMesh integration** for intelligent pathfinding
- **Multi-hit vulnerability** during stunned states
- **Dynamic defense scaling** with grapple debuff system


*** Wind Phase Transition ***

https://github.com/user-attachments/assets/2ed861ec-8446-4e95-8c71-1ebf963f6eab




#### **Boss Support Systems**
- **Projectile Turret** (`ProjectileTurret.cs`) - Automated defense systems
- **Afterimage Effects** (`Afterimages.cs`) - Visual trail system for boss movement
- **Wind Rotation** (`RotateWind.cs`) - Environmental effect management

### 🔮 Talisman System (`Assets/Akil/Scripts/Talismans/`)

#### **Base Talisman Architecture** (`TalismanBase.cs`)
- **Inheritance-based design** for consistent behavior
- **Cooldown management** with visual UI feedback
- **Rarity system** affecting power and cooldown
- **Input handling** with customizable keybinds

#### **Individual Talisman Implementations**

**Shield Talisman** (`ShieldTalisman.cs`)
- **Damage absorption** with visual shield effects
- **Regeneration mechanics** over time
- **UI integration** showing shield status

**Heal Talisman** (`HealTalisman.cs`)
- **Passive health regeneration** system
- **Scalable healing rates** based on rarity

**Charge Talisman** (`ChargeTalisman.cs`)
- **Invulnerability period** with forced forward movement
- **Damage on impact** with knockback effects
- **Particle system effects** for visual feedback
- **Speed modification** during charge state

**Grapple Talisman** (`GrappleTalisman.cs`)
- **Raycast-based targeting** system
- **Enemy debuffing** - reduces target defense
- **Dual interaction modes** - pull enemy or pull player
- **Physics-based movement** with smooth transitions

### 🌪️ Environmental Systems

#### **Hazard Controller** (`HazardController.cs`)
- **Dynamic hazard spawning** based on level progression
- **NavMesh integration** for safe spawn positioning
- **Multiple hazard types** (lightning, meteors)
- **Visual indicators** before hazard activation

#### **Wave Spawning System** (`SpawnScript.cs`)
- **Complex wave management** with enemy type distribution
- **Multi-level progression** with increasing difficulty
- **Gate-based spawning** with visual indicators
- **Boss and miniboss integration** at specific wave intervals
- **Dynamic enemy composition** based on level and wave

### 🎨 Visual Systems

#### **Particle Effects & VFX**
- **Boss-specific particle systems** for all major attacks
- **Weapon muzzle flashes** with dynamic timing
- **Environmental effects** for hazards and abilities
- **UI particle systems** for talisman activations

#### **Animation Integration**
- **Weapon-specific animations** for all attack types
- **Boss animation sequencing** with state management
- **Player feedback animations** for damage and abilities

### 🎮 User Interface & Experience

#### **Tutorial System** (`Assets/Akil/Scripts/Tutorial/`)
- **Interactive tutorial** with step-by-step guidance
- **Flashing arrow indicators** (`FlashingArrow.cs`)
- **Text controller system** (`TextController.cs`) for dynamic instructions

#### **Game State Management**
- **Game over functionality** with proper cleanup
- **Scene loading system** (`LoadScene.cs`)
- **Pause menu integration** across all systems

### 🔧 Technical Implementation Details

#### **Code Architecture**
- **Event-driven design** using Unity Events for system communication
- **Singleton patterns** for global state management
- **Component-based architecture** for modular functionality
- **Interface implementations** (IDamageable) for consistent damage handling

#### **Performance Optimizations**
- **Object pooling** for frequently spawned objects
- **Efficient raycast systems** for weapon targeting
- **Optimized particle systems** with proper cleanup
- **Memory management** for temporary effects and projectiles

#### **Game Balance Features**
- **Scalable difficulty** affecting damage, health, and spawn rates
- **Progressive enemy scaling** across levels
- **Weapon upgrade balancing** with meaningful choices
- **Boss phase transitions** with appropriate difficulty curves

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 or later
- FMOD Studio (for audio integration)

### Installation
1. Clone the repository
2. Open the project in Unity
3. Ensure all assets are properly imported
4. Build and run from the main scene

### Key Scenes
- `Assets/Victor/Scenes/MainMenu.unity` - Main menu and game selection
- `Assets/Akil/Scenes/Tutorial.unity` - Interactive tutorial (my contribution)
- `Assets/Play Scene/PlayTest.unity` - Main gameplay scene

## 🎯 Game Features

- **4 Unique Weapon Types** with multiple upgrade paths
- **3 Epic Boss Battles** with complex AI and multi-phase mechanics
- **4 Magical Talismans** with distinct abilities and rarities
- **Dynamic Environmental Hazards** that scale with progression
- **Comprehensive Tutorial System** for new players
- **Customizable Controls** with full keybind support
- **Multiple Difficulty Modes** for different skill levels

## 📁 Project Structure

```
Assets/Akil/                    # My complete contribution
├── Scripts/                    # All C# scripts I developed
│   ├── Boss/                   # Boss AI and mechanics
│   ├── Talismans/              # Magical ability system
│   └── Tutorial/               # Interactive tutorial system
├── Prefab/                     # Game objects and prefabs
├── Materials/                  # Custom materials and effects
├── Scenes/                     # Tutorial scene
└── Textures/                   # Custom textures and UI elements
```

## 🏆 Technical Achievements

- **42 C# Scripts** developed from scratch
- **Complex AI Systems** with state machines and behavior trees
- **Modular Weapon Architecture** supporting easy expansion
- **Event-Driven Communication** between game systems
- **Comprehensive Tutorial System** with interactive guidance

