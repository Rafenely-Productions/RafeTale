// Archivo: D:\Users\ricar\repos\DnDreams\DnDreams.UI.Shared\Extensions\ModifierTypeExtensions.cs
using DnDreams.Domain.Enums;

namespace DnDreams.UI.Shared.Extensions;

public static class ModifierTypeExtensions
{
    // Asegúrate de que la firma devuelva "string CssClasses"
    public static (string Icon, string CssClasses, string Label) GetVisualMetadata(this ModifierType type)
    {
        return type switch
        {
            // Grupo Atributos y Estadísticas (Azul / Cyan)
            ModifierType.AttributeBonus or ModifierType.ModifyAbilityScoreMaximum =>
                ("fa-up-long", "bg-blue-950/40 border-blue-500/30 text-blue-400", "Mejora de Atributo"),

            ModifierType.SetBaseArmorClass or ModifierType.SetBaseArmorClass =>
                ("fa-shield-halved", "bg-slate-950/40 border-slate-500/30 text-slate-400", "Fórmula de Armadura"),

            ModifierType.ModifySpeed =>
                ("fa-person-running", "bg-emerald-950/40 border-emerald-500/30 text-emerald-400", "Modificador de Velocidad"),

            // Grupo Recursos y Progresión (Ámbar / Amarillo)
            ModifierType.GrantResource or ModifierType.ScaleResource or ModifierType.ModifyResourceReset =>
                ("fa-battery-full", "bg-amber-950/40 border-amber-500/30 text-amber-500", "Recurso de Clase"),

            ModifierType.GrantSubclass or ModifierType.GrantSubclassFeature =>
                ("fa-crown", "bg-yellow-950/40 border-yellow-500/30 text-yellow-400", "Rasgo de Subclase"),

            ModifierType.GrantFeat or ModifierType.SelectFeat or ModifierType.GrantFeatCategory =>
                ("fa-award", "bg-indigo-950/40 border-indigo-500/30 text-indigo-400", "Dote Otorgada"),

            // Grupo Combate y Acciones (Rojo / Naranja)
            ModifierType.AddDamageBonus or ModifierType.OnHitModifier =>
                ("fa-burst", "bg-red-950/40 border-red-500/30 text-red-400", "Bono de Daño"),

            ModifierType.ModifyAttacksPerAction or ModifierType.ModifyAttacksPerAttackAction =>
                ("fa-hand-fist", "bg-orange-950/40 border-orange-500/30 text-orange-400", "Ataques Múltiples"),

            ModifierType.GrantAdvantage =>
                ("fa-dice-d20", "bg-lime-950/40 border-lime-500/30 text-lime-400", "Ventaja Mecánica"),

            ModifierType.EnableOption or ModifierType.PassiveEffect =>
                ("fa-toggle-on", "bg-sky-950/40 border-sky-500/30 text-sky-400", "Efecto Pasivo"),

            ModifierType.ReactionEffect or ModifierType.Reaction =>
                ("fa-bolt-lightning", "bg-violet-950/40 border-violet-500/30 text-violet-400", "Efecto de Reacción"),

            // Grupo Magia y Conjuros (Púrpura)
            ModifierType.GrantSpell or ModifierType.GrantAlwaysPreparedSpell or ModifierType.AlwaysPreparedSpell or ModifierType.AlwaysPreparedSpells or ModifierType.EnableSpellcasting or ModifierType.EnablePactMagic =>
                ("fa-wand-sparkles", "bg-purple-950/40 border-purple-500/30 text-purple-400", "Mecánica Mágica"),

            // Grupo Competencias, Idiomas y Utilidad (Teal / Rosa)
            ModifierType.GrantProficiency or ModifierType.GrantSaveProficiency or ModifierType.GrantExpertise =>
                ("fa-graduation-cap", "bg-teal-950/40 border-teal-500/30 text-teal-400", "Competencia / Pericia"),

            ModifierType.GrantLanguage or ModifierType.GrantLanguages =>
                ("fa-language", "bg-rose-950/40 border-rose-500/30 text-rose-400", "Idioma Aprendido"),

            ModifierType.GrantSense =>
                ("fa-eye", "bg-pink-950/40 border-pink-500/30 text-pink-400", "Sentido Especial"),

            // Fallback por defecto
            _ => ("fa-circle-question", "bg-slate-950/40 border-slate-700/30 text-slate-400", "Modificador")
        };
    }
}