# Sts2ModAIAgentCommunicator

A Slay the Spire 2 mod that provides an interface for external AI agents to communicate with the game via named pipes.

## Features

- **Named Pipe Communication**: Connects to an external agent using a named pipe (`live-pipe`).
- **Combat Event Hooking**: Automatically notifies the agent about game events such as combat starting and player turn starting.
- **Remote Actions**: Supports external requests to:
  - Retrieve the player's current hand.
  - Play cards from the hand targeting specific creatures.
- **Asynchronous Pipeline**: Uses a non-blocking communication pipeline for seamless gameplay.

## Development Setup

### Prerequisites

Before you begin, ensure you have:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Godot 4.5.1 Mono](https://godotengine.org/download/archive/4.5.1-stable/) - **Download the "Windows 64-bit, .NET" version**
- Slay the Spire 2 installed via Steam

---

### Initial Configuration

#### 1. Clone the Repository
```bash
git clone https://github.com/dkalchenko/sts2_mod_template
cd sts2_mod_template
```

#### 2. Configure Your Paths

**Windows (PowerShell):**
```powershell
Copy-Item local.props.example local.props
```

**Linux/Mac:**
```bash
cp local.props.example local.props
```

#### 3. Edit `local.props`

Open `local.props` in any text editor and update with **your** paths:
```xml
<Project>
    <PropertyGroup>
        <!-- Example for default Steam installation: -->
        <STS2GamePath>C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2</STS2GamePath>

        <!-- Example Godot path: -->
        <GodotExePath>C:\Godot\Godot_v4.5.1-stable_mono_win64.exe</GodotExePath>
    </PropertyGroup>
</Project>
```
---

#### 4. Edit manifest data in `Sts2ModAIAgentCommunicator.csproj` (Optional)

Open `Sts2ModAIAgentCommunicator.csproj` and update metadata if needed:
```xml
<!-- ↓ MOD METADATA - Change these! -->
<ModName>Sts2ModAIAgentCommunicator</ModName>
<ModDisplayName>AI agent communicator</ModDisplayName>
<ModAuthor>Your Name</ModAuthor>
<ModVersion>1.0.0</ModVersion>
```
---

### Building the Mod

#### Visual Studio / Rider
Open `Sts2ModAIAgentCommunicator.csproj`

Press **Ctrl+Shift+B** or click **Build → Build Solution**

The mod will **automatically** install to:
`Slay the Spire 2/mods/Sts2ModAIAgentCommunicator/`

---

## Troubleshooting

### "Cannot find Godot executable"
- Make sure `GodotExePath` in `local.props` points to the `.exe` file
- Download the **Mono** version, not the standard version

### "Cannot find Slay the Spire 2"
- Right-click STS2 in Steam → Manage → Browse local files
- Copy the full path and paste into `STS2GamePath`

### Build succeeds but mod doesn't load
- Check that `Sts2ModAIAgentCommunicator.dll` exists in `mods/Sts2ModAIAgentCommunicator/`
- Check the game's log file for errors: `%AppData%\Roaming\SlayTheSpire2\Player.log`

### Changes don't appear in game
- Rebuild the mod (**Ctrl+Shift+B**) or with Rebuild Solution
- Restart Slay the Spire 2
