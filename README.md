# Dragon Keep

Dragon Keep is a Valheim building mod that adds a large modular dragon pen and castle-themed construction set.

The mod was created to work together with **Elemental_Dragons** by **JamesJonesTV**, providing a dragon pen for its dragons.

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
- Working synchronized doors with custom sounds
- Roof door that automatically opens when a player comes within 10 metres
- Roof door automatically closes 7 seconds after the player leaves
- Building pieces are protected from normal damage and environmental wear
- Four configurable material requirement slots for every piece

## Included Build Pieces

- Dragon Pen
- Dragon Pen no roof
- Dragon Pen roof
- Dragon Pen Wall
- Dragon Pen Original High Wall
- Dragon Pen 6m Wall
- Dragon Pen Short Wall
- Dragon Pen Castle Tower
- Dragon Pen Corner Tower
- Dragon Pen Corner Piece
- Dragon Pen Small Corner Piece
- Dragon Pen Main Gate
- Dragon Throne
- Dragon Throne Large

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

Every build piece has four configurable requirement slots:

- Requirement 1 Item / Amount
- Requirement 2 Item / Amount
- Requirement 3 Item / Amount
- Requirement 4 Item / Amount

Use the Valheim prefab name for the item, such as:

- `Wood`
- `Stone`
- `Iron`
- `Copper`

Set a requirement amount to `0` to disable that slot. Blank item names are also ignored.

Example:

```ini
Requirement 1 Item = Wood
Requirement 1 Amount = 50
Requirement 2 Item = Stone
Requirement 2 Amount = 40
Requirement 3 Item =
Requirement 3 Amount = 0
Requirement 4 Item =
Requirement 4 Amount = 0
```

Build-cost changes require a Valheim restart before they take effect.

## Default Build Costs

| Build Piece | Default Cost |
| --- | --- |
| Dragon Pen | 100 Iron, 200 Wood, 50 Resin |
| Dragon Pen no roof | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen roof | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Wall | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Original High Wall | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen 6m Wall | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Short Wall | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Castle Tower | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Corner Tower | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Corner Piece | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Small Corner Piece | 50 Iron, 100 Wood, 40 Resin |
| Dragon Pen Main Gate | 50 Iron, 100 Wood, 40 Resin |
| Dragon Throne | 50 Iron, 100 Wood, 40 Resin |
| Dragon Throne Large | 250 Stone, 100 Iron, 40 Resin |

All pieces require a Workbench for placement.

## Multiplayer

Install the same mod version on the server and all players who connect to it. Keep build-cost settings consistent between the server and clients.

## Credits

Created by Caen007.
