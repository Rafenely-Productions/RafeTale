# 🎲 RafeTale

> **by Rafenely Studios**
> 
> A cross-platform tabletop RPG rules engine. Create your own systems, import community content packs, and manage characters with an immersive digital interface.

<p align="center">
  <img src="docs/screenshots/home.png" alt="RafeTale" width="300">
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
RafeTale/
├── 📁 RafeTale.Domain/          # Pure entities, enums, value objects
├── 📁 RafeTale.Application/     # Business logic, DTOs, interfaces
├── 📁 RafeTale.Infrastructure/  # Persistence, extractors, seeders
├── 📁 RafeTale.UI.Shared/       # Shared Blazor components
└── 📁 RafeTale.MAUI/            # App shell, config, assets
```

**Pattern:** Clean Architecture + CQRS-like (queries and commands separated in services)

### Generic DTO Services

A key pattern in the Application layer is the generic contract:

```csharp
public interface IService<TDto, TEntity>
    where TDto : class where TEntity : class
{
    Task<TDto> ArmDto(TEntity entity);
    TDto ArmDto(TEntity entity, Dictionary<LocProperty, Dictionary<Guid, string>>? localizedWords = null);
    Task<List<TDto>> GetAllAsync(Expression<Func<TEntity, bool>>? filter, Action<IncludeAggregator<TEntity>>? includes = null);
    Task<TDto> GetByIdAsync(Guid id, Action<IncludeAggregator<TEntity>>? includes = null);
}
```

Implemented for: `Character`, `ClassDefinition`, `Race`, `Subclass`, `Spell`, `Language`, `Background`, `Feat`.

### Dependency Injection

Configured in `RafeTale.MAUI/MauiProgram.cs`:

- `RafeTaleDbContext` scoped to a local SQLite path.
- `IAppInitializer` singleton for startup seeding.
- Read services (`ICharacterQueryService`, `ILibraryDataService`, etc.) and write services (`ILevelUpService`, `ILevelingService`, etc.).
- Default culture forced to `es-MX`.

### Core Domain Model

| Entity | Responsibility |
|--------|----------------|
| `Character` | Player character; computed attributes from `Stats` + `CharacterModifiers`. |
| `Race` / `SubRace` | Playable lineages; may grant traits and features. |
| `ClassDefinition` / `Subclass` | Playable classes and specializations. |
| `ClassLevelProgression` | Level-by-level class benefits (HP dice, features, spell slots, ASI flags). |
| `Background` | Character origin; grants skills/equipment/features. |
| `Feat` | Optional advancement choices. |
| `Feature` | Reusable rule elements referenced by classes, races, feats, backgrounds. |
| `Spell` | Magic abilities linked to classes via spell lists. |
| `CharacterModifier` / `ActiveModifiers` | Dynamic or temporary bonuses. |
| `XpRules` | XP thresholds per level. |
| `LocalizedContent` | Translatable strings keyed by entity and property. |

---

## 🚀 How to Run

### Requirements
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) with **.NET MAUI** workload
- Windows 10/11, Android, or iOS (Mac required for iOS)

### Steps

```bash
# 1. Clone
git clone https://github.com/rafenely-productions/rafetale.git
cd RafeTale

# 2. Restore packages
dotnet restore

# 3. Run on Windows (WinUI 3)
dotnet build RafeTale.MAUI/RafeTale.MAUI.csproj -f net9.0-windows10.0.19041.0
dotnet run --project RafeTale.MAUI/RafeTale.MAUI.csproj -f net9.0-windows10.0.19041.0

# Or on Android (needs emulator or connected device)
dotnet build RafeTale.MAUI/RafeTale.MAUI.csproj -f net9.0-android
dotnet run --project RafeTale.MAUI/RafeTale.MAUI.csproj -f net9.0-android
```

### First Time
The app ships with bundled offline data:

- `RafeTale.MAUI/Resources/Raw/RafeTale_SRD_v1.xlsx` — bundled SRD-style content pack.
- `RafeTale.MAUI/Resources/Raw/dndreams.db3` — pre-seeded SQLite database.

On first launch, `AppInitializer` checks the local DB and optionally imports the default pack. Subsequent launches use the local DB directly.

---

## 🎮 User Flows

### Character Creation (5-step wizard)
1. **Select Race** — choose race and subrace.
2. **Select Class** — choose class and subclass.
3. **Select Background** — choose origin background.
4. **Abilities** — assign base ability scores.
5. **Confirm** — finalize and persist.

### Character Dashboard
- Header with HP, level, XP, AC, attributes.
- Tabs: Features, Spells, Skills.

### Level Up Wizard
- HP selection (roll or average).
- ASI / Feat selection.
- Spell selection.

### Rules Library
Browse classes, races, backgrounds, feats, spells, and the XP table with lazy loading and inspectors.

### Dice Roller
Roll D4, D6, D8, D10, D12, D20, D100 with animated history.

---

## 📦 Content Pack Format

Content packs are Excel workbooks parsed by `ExcelDataExtractor` using ClosedXML. Expected sheets (subject to extractor implementation):

| Sheet | Maps to Entity |
|-------|----------------|
| Races | `Race`, `SubRace` |
| Classes | `ClassDefinition`, `Subclass`, `ClassLevelProgression`, `SubclassLevelProgression` |
| Backgrounds | `Background` |
| Feats | `Feat` |
| Spells | `Spell` |
| Features | `Feature` |
| Items | `ItemTemplate` |
| Languages | `Language` |
| Traits | `Trait`, `SpecialTrait` |
| XP Rules | `XpRules` |
| LocalizedContent | `LocalizedContent` |

The `SandboxPage.razor` screen can import custom packs at runtime.

---

## 🧪 Tests

```bash
cd RafeTale.Tests
dotnet test
```

| Suite | Responsibility |
|-------|----------------|
| `Domain/Entities/CharacterTests` | Stats, modifiers, proficiency, skills. |
| `Application/Helpers/SpellBudgetTests` | Cantrip, spell, and level validation. |
| `Application/Services/LevelUpServiceTests` | Level-up flow validation. |

**Testing stack:** xUnit + FluentAssertions + NSubstitute

---

## 🧰 Development Conventions

### Layer Rules
- `Domain` has no external dependencies.
- `Application` references only `Domain`.
- `Infrastructure` references `Application` and `Domain`.
- `UI.Shared` references `Application` and `Domain`.
- `MAUI` references all projects for DI registration.

### Localization
- Use `IStringLocalizer<AppStrings>` for UI labels.
- Use `IStringLocalizer<LibraryStrings>` for rules/library labels.
- Base culture is `es-MX`.

### Naming Legacy
The project was previously named **DnDreams**. Some legacy names remain (e.g., `DnDreamsDbContext.cs`, `dndreams.db3`, migration snapshot). Prefer `RafeTale` for new code.

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
