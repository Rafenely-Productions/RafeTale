# RafeTale Roadmap

> Última actualización: 2026-08-15

---

## 🎯 Visión
RafeTale es una app de gestión de personajes para D&D 5e, con foco en:
- Creación de personajes guiada (wizard de 5 pasos)
- Dashboard de personaje con hechizos, habilidades y nivelación
- Importación de contenido desde Excel (content packs)
- Soporte multilenguaje

---

## 📦 Versiones

### v0.1.x — Foundation (Actual)
- [x] Creación de personajes (raza, clase, trasfondo, habilidades)
- [x] Dashboard básico con tabs
- [x] Sistema de hechizos con slots
- [x] Importación de content packs desde Excel
- [x] Codex de reglas (razas, clases, trasfondos, feats, hechizos)
- [x] Sistema de XP y nivelación
- [x] Migración a .NET 10

### v0.2.0 — Character Sheet Completa
- [ ] Ficha de personaje completa (PDF export)
- [ ] Inventario y equipamiento
- [ ] Sistema de monedas y tesoro
- [ ] Death saves y estados de condición
- [ ] Short rest / Long rest con recuperación automática

### v0.3.0 — Campaign Manager
- [ ] Creación y gestión de campañas
- [ ] Journal de sesiones con timestamps
- [ ] Vinculación de personajes a campañas
- [ ] Compartir personajes entre jugadores
- [ ] Notas del DM vinculadas a campañas

### v0.4.0 — Content Pack Avanzado
- [ ] Editor visual de content packs
- [ ] Validación de content packs en tiempo real
- [ ] Content packs oficiales pre-instalados
- [ ] Sistema de mods / homebrew

### v0.5.0 — Polish & Multiplayer
- [ ] Sincronización en la nube (opcional)
- [ ] Tema oscuro / claro completo
- [ ] Accesibilidad (screen readers, alto contraste)
- [ ] Optimización de rendimiento en soluciones grandes

### v1.0.0 — Release Estable
- [ ] Todas las features de v0.5.0 estables
- [ ] Documentación completa
- [ ] Publicación en App Store / Google Play / Microsoft Store
- [ ] Soporte para D&D 2024 rules (si aplica)

---

## 🗂️ Backlog (sin asignar a versión)

### UI/UX
- [ ] Animaciones de transición entre pantallas
- [ ] Haptic feedback en tiradas de dados
- [ ] Widget de dados en home screen
- [ ] Modo landscape para tabletas

### Domain
- [ ] Sistema de multiclassing
- [ ] Sistema de feats alternativos (Tasha's)
- [ ] Custom lineage / custom origin

### Infrastructure
- [ ] Migración de SQLite a opción de base de datos remota
- [ ] Backup automático local
- [ ] Logging estructurado (Serilog)

### Tests
- [ ] Cobertura de tests > 80%
- [ ] Tests de UI con Playwright / Appium
- [ ] Tests de integración para importación Excel

---

## 🏷️ Leyenda

| Estado | Significado |
|--------|-------------|
| [x]    | Completado |
| [-]    | En progreso |
| [ ]    | Pendiente |
| [?]    | En evaluación / necesita diseño |

---

## 📝 Notas

- Las versiones menores (v0.x.0) representan milestones funcionales.
- Los patches (v0.x.y) son bugfixes y mejoras menores.
- Este roadmap vive en el repo. Se actualiza vía PR.
