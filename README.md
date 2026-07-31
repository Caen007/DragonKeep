# Dragon Keep

Dragon Keep is a Valheim building mod that adds a large modular dragon pen and castle-themed construction set.

The mod was commissioned by **JamesJonesTV** and created to work together with **Elemental_Dragons**, providing a dragon pen for its dragons.

Every build piece uses detailed, high-resolution 3D construction made from individually modeled parts assembled into the structure, rather than flat picture-based walls or floors.

The individual build pieces can be used to create smaller custom dragon pens when the complete Dragon Pen is too large. The **Dragon Pen Base** and **Dragon Pen Roof** are designed to work together: the base has four ground snap points where the roof's four pillars can attach. The roof is optional, so players can also place and use the base by itself.

## Version

**1.0.0**

## Requirements

- Valheim
- BepInEx
- Jötunn
- Configuration Manager *(optional, for editing build costs in game)*

## Features

- Dedicated **Dragon Keep** category in the Hammer build menu
- Large complete Dragon Pen
- Separate roof and roofless Dragon Pen options
- Modular walls, towers, corner pieces, gate, and thrones
- Working synchronized doors with Valheim door sounds and custom main-gate sounds
- Roof door that automatically opens when a player comes within 10 metres
- Roof door automatically closes 7 seconds after the player leaves
- Building pieces are protected from normal damage and environmental wear
- One editable build-cost line for every piece

## Included Build Pieces

- Dragon Pen With Roof
- Dragon Pen Without Roof
- Dragon Pen Roof
- Dragon Pen Wall
- Dragon Pen Original High Wall
- Dragon Pen 6m Wall
- Dragon Pen Short Wall
- Dragon Pen Castle Tower
- Dragon Pen Corner Tower
- Dragon Pen Corner Piece
- Dragon Pen Small Corner Piece
- Dragon Pen Main Gate
- Dragon Pen Throne
- Dragon Pen Throne Large

Additional compatible prefabs included in the mod bundle with the `DP_` prefix are registered automatically.

## Installation

1. Install BepInEx for Valheim.
2. Install Jötunn.
3. Copy `DragonKeep.dll` into:

   `BepInEx/plugins/DragonKeep/`

4. Start Valheim.
5. Equip the Hammer and open the **Dragon Keep** category.

## Configurable Build Costs

After the game has been started once with the mod installed, the configuration file is created at:

`BepInEx/config/DragonKeep.cfg`

Every build piece has one editable build-cost line using this format:

```ini
DP_DragonPen = [Iron][400][RoundLog][400][Obsidian][400][Coins][5000]
```

Each requirement is written as `[Valheim prefab name][amount]`, with a maximum of four requirements. Invalid item names or amounts automatically fall back to that piece's default build cost.

If an older mod version created the four-slot Item/Amount settings, delete `DragonKeep.cfg` once before starting Valheim with this version. The mod will create the clean single-line configuration automatically.

Build-cost changes require a Valheim restart before they take effect.

## Default Build Costs

| Build Piece | Default Cost |
| --- | --- |
| Dragon Pen Without Roof | 400 Iron, 400 Core Wood, 400 Obsidian, 4,000 Coins |
| Dragon Pen With Roof | 400 Iron, 400 Core Wood, 400 Obsidian, 5,000 Coins |
| Dragon Pen Roof | 40 Bronze, 100 Fine Wood, 200 Crystal, 400 Resin |
| Dragon Pen 6m Wall | 20 Obsidian, 20 Stone |
| Dragon Pen Wall | 8 Obsidian, 8 Stone |
| Dragon Pen Short Wall | 2 Obsidian, 2 Stone |
| Dragon Pen Corner Piece | 10 Obsidian |
| Dragon Pen Small Corner Piece | 5 Obsidian |
| Dragon Pen Original High Wall | 20 Obsidian, 20 Stone, 20 Iron, 20 Core Wood |
| Dragon Pen Castle Tower | 100 Obsidian, 100 Stone, 10 Crystal, 210 Coins |
| Dragon Pen Corner Tower | 50 Iron, 50 Stone, 50 Core Wood, 150 Coins |
| Dragon Pen Main Gate | 50 Obsidian, 100 Iron, 100 Core Wood, 250 Coins |
| Dragon Pen Throne | Moder Trophy, Serpent Trophy, 50 Bronze, 500 Coins |
| Dragon Pen Throne Large | Moder Trophy, Fader Trophy, 200 Bronze, 2,000 Coins |

All pieces require a Workbench for placement.

## Multiplayer

Install the same mod version on the server and all players who connect to it. Keep build-cost settings consistent between the server and clients.

## Credits

Commissioned by JamesJonesTV and created by Caen007.
