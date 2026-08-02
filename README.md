# 🎲 Rafedream

> **by Rafenely Studios**
> 
> A cross-platform tabletop RPG rules engine. Create your own systems, import community content packs, and manage characters with an immersive digital interface.

<p align="center">
  <img src="docs/screenshots/home.png" alt="Rafedream" width="300">
</p>

---

## ✨ Features

| Feature | Status | Description |
|---------|--------|-------------|
| 🧙 **Character Creation** | ✅ | 5-step wizard: lineage, path, background, abilities, confirmation |
| 📊 **Character Dashboard** | ✅ | Stats, spells, skills, features — all in a dark, immersive dashboard |
| 📖 **Rules Library** | ✅ | Classes, lineages, spells, feats, backgrounds and XP table — with lazy loading |
| 🎲 **Dice Roller** | ✅ | D4, D6, D8, D10, D12, D20, D100 with animations |
| ⬆️ **Level Up** | ✅ | Level-up wizard with HP, ASI/Feat and spell selection |
| 🌍 **Localization** | ✅ | Spanish (es-MX) base, architecture ready for English |
| 📦 **Local Database** | ✅ | SQLite with EF Core, seed from Excel packs |
| ☁️ **Sync** | 🚧 | Roadmap: self-hosted API |

---

## 🏗️ Architecture

```
Rafedream/
├── 📁 Rafedream.Domain/          # Pure entities, enums, value objects
├── 📁 Rafedream.Application/     # Business logic, DTOs, interfaces
├── 📁 Rafedream.Infrastructure/  # Persistence, extractors, seeders
├── 📁 Rafedream.UI.Shared/       # Shared Blazor components
└── 📁 Rafedream.MAUI/            # App shell, config, assets
```

**Pattern:** Clean Architecture + CQRS-like (queries and commands separated in services)

---

## 🚀 How to Run

### Requirements
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with **.NET MAUI** workload
- Windows 10/11, Android, or iOS (Mac required for iOS)

### Steps

```bash
# 1. Clone
git clone https://github.com/rafenely-productions/rafedream.git
cd Rafedream

# 2. Restore packages
dotnet restore

# 3. Run on Windows (WinUI 3)
dotnet build Rafedream.MAUI/Rafedream.MAUI.csproj -f net9.0-windows10.0.19041.0
dotnet run --project Rafedream.MAUI/Rafedream.MAUI.csproj -f net9.0-windows10.0.19041.0

# Or on Android (needs emulator or connected device)
dotnet build Rafedream.MAUI/Rafedream.MAUI.csproj -f net9.0-android
dotnet run --project Rafedream.MAUI/Rafedream.MAUI.csproj -f net9.0-android
```

### First Time
The app detects if the SQLite database is empty and optionally imports data from an Excel content pack (not included). On subsequent launches, it uses the local DB directly.

---

## 🧪 Tests

```bash
cd Rafedream.Tests
dotnet test
```

| Suite | Tests | Coverage |
|-------|-------|----------|
| Domain (Character) | 10 | Modifiers, stats, proficiency, skills |
| Application (SpellBudget) | 10 | Cantrip, spell, and level validation |

**Testing stack:** xUnit + FluentAssertions + NSubstitute

---

## 🎨 Design System

- **Palette:** Parchment (`#f5f5dc`), Gold (`#d4af37`), Blood Red (`#8b0000`), Slate (`#0f172a`)
- **Typography:** Inter (UI), Cinzel (headers), monospace (stats and numbers)
- **CSS Framework:** Tailwind CSS via CDN in Blazor components
- **Icons:** Font Awesome 6, Bootstrap Icons

---

## 🗺️ Roadmap

- [x] MVP: Character creation, dashboard, library, dice rolling
- [x] Refactoring: Clean Architecture, unit tests
- [x] UI decoupling: Library, CharacterDashboard, LevelUpWizard
- [ ] README + documentation
- [ ] Dark/light mode toggle
- [ ] Export character to PDF
- [ ] Cloud sync (self-hosted server)
- [ ] Encounter builder
- [ ] Game Master tablet mode

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Blazor Hybrid (.NET MAUI) |
| **Backend/Local** | SQLite + Entity Framework Core 9 |
| **Import** | ClosedXML (Excel → SQLite) |
| **Localization** | `IStringLocalizer` + `.resx` files |
| **Testing** | xUnit, FluentAssertions, NSubstitute |
| **UI** | Tailwind CSS, Font Awesome, CSS variables |

---

## 📸 Screenshots

> *Coming soon: GIFs of character creation, dashboard, dice rolling and library.*

---

## 📝 License

MIT — Use it, modify it, share it. May your rolls be ever in your favor. 🎲

---

<p align="center">
  <i>"Not just an app. Your digital grimoire."</i>
</p>
