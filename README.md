# Immersethebay Hackathon

 Devine Draw — VR Card-Throwing Tower Defense

Devine Draw is a fast-paced VR strategy game where players physically throw magical cards to summon allies, cast spells, and protect their tower from waves of enemies. Built with Unity, C#, and the XR Interaction Toolkit, this project blends real-time action, tactile VR interactions, and AI-driven combat to create an immersive experience.

 Game Summary

Enemies charge toward your tower.
Your only defense?
A magical deck filled with spells, summons, and abilities you activate by literally throwing cards into the battlefield.

Where the card lands determines:

 Which ally appears

 Which spell activates

 What effect triggers (AOE, buffs, heals, etc.)

Everything is real-time, physics-based, and interactive.

 Core Mechanics
 Physical Card Throwing

Grab a card using VR hand-tracking or controllers

Throw it toward enemies or strategic locations

Spawn effects based on collision point

 Spell & Summon Cards

Each card uses customizable stats:

SpellType (AOE, Heal, Buff, Special)

Range, damage, cooldown

Custom behaviors defined in SpellCard.cs

 Allies vs. Enemies

Enemy AI walks toward the tower, attacking it on arrival

Ally units automatically engage nearby enemies

Both sides use radius triggers to initiate combat

Units animate using AnimationStateController.cs

🏰 Tower Defense

If enemies reach the tower, the tower takes damage

When tower health hits 0, the player loses

Managed with TowerManager.cs and GameManager.cs

 AI Systems
Opponent AI

Pathfinding toward the tower

Auto-engages allies when in range

Handles attack loops, damage, and death cleanup

Ally AI

Automatically seeks and attacks enemies

Protects the tower by intercepting threats

Health system + regeneration + death logic

Both share a unified UnitStats system for easy balancing.

 Tech Stack

Unity 2022+

C#

XR Interaction Toolkit

Meta Quest / PCVR

https://devpost.com/software/thezou

Physics-based card throwing

Scriptable character stats
