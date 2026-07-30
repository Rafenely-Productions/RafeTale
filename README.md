# 🎲 DnDreams

> **Tu compañero digital para Dungeons & Dragons 5e.**
> Gestión de personajes, grimorio interactivo, biblioteca de reglas y tiradas de dados — todo en una app .NET MAUI con la estética de una mesa de rol.

<p align="center">
  <img src="docs/screenshots/home.png" alt="DnDreams Home" width="300">
</p>

---

## ✨ Features

| Feature | Estado | Descripción |
|---------|--------|-------------|
| 🧙 **Creación de personajes** | ✅ | Wizard de 5 pasos: raza, clase, trasfondo, habilidades, confirmación |
| 📊 **Dashboard de personaje** | ✅ | Stats, hechizos, skills, features — todo en un dashboard oscuro y elegante |
| 📖 **Biblioteca D&D** | ✅ | Clases, razas, hechizos, dotes, trasfondos y tabla de XP — con lazy loading |
| 🎲 **Tiradas de dados** | ✅ | D4, D6, D8, D10, D12, D20, D100 con animaciones |
| ⬆️ **Subida de nivel** | ✅ | Wizard de level up con HP, ASI/Feat y selección de hechizos |
| 🌍 **Localización** | ✅ | Español (es-MX) base, arquitectura lista para inglés |
| 📦 **Base de datos local** | ✅ | SQLite con EF Core, seed desde Excel (`DnDreams_v2.xlsx`) |
| ☁️ **Sincronización** | 🚧 | Roadmap: API self-hosted en laptop Linux |

---

## 🏗️ Arquitectura

```
DnDreams/
├── 📁 DnDreams.Domain/          # Entidades puras, enums, value objects
│   └── Entities/
│       ├── Character.cs
│       ├── Spell.cs
│       ├── ClassDefinition.cs
│       └── ...
│
├── 📁 DnDreams.Application/     # Lógica de negocio, DTOs, interfaces
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│       ├── LevelUpService.cs
│       ├── SpellBudget.cs
│       └── LocalizationService.cs
│
├── 📁 DnDreams.Infrastructure/  # Persistencia, extractores, seeders
│   ├── Persistence/
│   │   ├── DnDreamsDbContext.cs
│   │   └── DependencyInjection.cs
│   └── Extractors/
│       └── ExcelImportService.cs
│
├── 📁 DnDreams.UI.Shared/       # Componentes Blazor compartidos
│   ├── Components/              # Componentes reutilizables
│   │   ├── Library/
│   │   ├── Character/
│   │   └── UI/
│   ├── Pages/                   # Páginas principales
│   └── Shared/                  # Layouts, MainLayout, NavMenu
│
└── 📁 DnDreams.MAUI/            # Shell de la app, configuración, assets
    ├── MauiProgram.cs
    ├── App.xaml.cs
    └── Platforms/               # Código específico por plataforma
```

**Patrón:** Clean Architecture + CQRS-like (queries y commands separados en servicios)

---

## 🚀 Cómo correr

### Requisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) con workload **.NET MAUI**
- Windows 10/11, Android, o iOS (Mac necesario para iOS)

### Pasos

```bash
# 1. Clonar
git clone https://github.com/rafenely/DnDreams.git
cd DnDreams

# 2. Restaurar paquetes
dotnet restore

# 3. Correr en Windows (WinUI 3)
dotnet build DnDreams.MAUI/DnDreams.MAUI.csproj -f net9.0-windows10.0.19041.0
dotnet run --project DnDreams.MAUI/DnDreams.MAUI.csproj -f net9.0-windows10.0.19041.0

# O en Android (necesitas emulador o dispositivo conectado)
dotnet build DnDreams.MAUI/DnDreams.MAUI.csproj -f net9.0-android
dotnet run --project DnDreams.MAUI/DnDreams.MAUI.csproj -f net9.0-android
```

### Primera vez
La app detecta si la base de datos SQLite está vacía y automáticamente importa los datos desde `DnDreams_v2.xlsx` (incluido como asset). La segunda vez en adelante, usa la DB local directamente.

---

## 🧪 Tests

```bash
cd DnDreams.Tests
dotnet test
```

| Suite | Tests | Cobertura |
|-------|-------|-----------|
| Domain (Character) | 9 | Modifiers, stats, proficiency, skills |
| Application (SpellBudget) | 10 | Validación de cantrips, spells, niveles |

**Stack de testing:** xUnit + FluentAssertions + NSubstitute

---

## 🎨 Design System

- **Paleta:** Parchment (`#f5f5dc`), Gold (`#d4af37`), Blood Red (`#8b0000`), Slate (`#0f172a`)
- **Tipografía:** Open Sans (UI), serif (descripciones de D&D), monospace (stats y números)
- **Framework CSS:** Tailwind CSS vía CDN en Blazor components
- **Iconos:** Font Awesome 6

---

## 🗺️ Roadmap

- [x] MVP: Crear personaje, dashboard, biblioteca, tiradas
- [x] Refactorización: Clean Architecture, tests unitarios
- [x] Desacoplamiento de UI: Library, CharacterDashboard, LevelUpWizard
- [ ] README + documentación
- [ ] Modo oscuro/claro toggle
- [ ] Exportar personaje a PDF
- [ ] Sincronización en la nube (laptop Linux como servidor)
- [ ] Generador de encuentros
- [ ] Modo "Mesa de DM" para tablets

---

## 🛠️ Stack Técnico

| Capa | Tecnología |
|------|------------|
| **Frontend** | Blazor Hybrid (.NET MAUI) |
| **Backend/Local** | SQLite + Entity Framework Core 9 |
| **Importación** | ClosedXML (Excel → SQLite) |
| **Localización** | `IStringLocalizer` + archivos `.resx` |
| **Testing** | xUnit, FluentAssertions, NSubstitute |
| **UI** | Tailwind CSS, Font Awesome, CSS variables |

---

## 📸 Screenshots

> *Próximamente: GIFs de creación de personaje, dashboard, tirada de dados y biblioteca.*

---

## 📝 Licencia

MIT — Úsalo, modifícalo, compártelo. Que rueden los dados. 🎲

---

<p align="center">
  <i>"No es solo una app. Es tu grimorio digital."</i>
</p>
