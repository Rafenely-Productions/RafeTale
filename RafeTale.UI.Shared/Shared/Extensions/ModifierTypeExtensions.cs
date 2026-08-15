// Archivo: D:\Users\ricar\repos\RafeTale\RafeTale.UI.Shared\Extensions\ModifierTypeDtoExtensions.cs
using RafeTale.Application.DTOs;

namespace RafeTale.UI.Shared.Extensions;

public static class ModifierTypeDtoExtensions
{
    // Asegúrate de que la firma devuelva "string CssClasses"
    public static (string Icon, string CssClasses, string Label) GetVisualMetadata(this ModifierTypeDto type)
    {
        return type switch
        {
            // Grupo Atributos y Estadísticas (Azul / Cyan)
            ModifierTypeDto.AttributeBonus or ModifierTypeDto.ModifyAbilityScoreMaximum =>
                ("fa-up-long", "bg-blue-950/40 border-blue-500/30 text-blue-400", "Mejora de Atributo"),

            ModifierTypeDto.SetBaseArmorClass or ModifierTypeDto.SetBaseArmorClass =>
                ("fa-shield-halved", "bg-slate-950/40 border-slate-500/30 text-slate-400", "Fórmula de Armadura"),

            ModifierTypeDto.ModifySpeed =>
                ("fa-person-running", "bg-emerald-950/40 border-emerald-500/30 text-emerald-400", "Modificador de Velocidad"),

            // Grupo Recursos y Progresión (Ámbar / Amarillo)
            ModifierTypeDto.GrantResource or ModifierTypeDto.ScaleResource or ModifierTypeDto.ModifyResourceReset =>
                ("fa-battery-full", "bg-amber-950/40 border-amber-500/30 text-amber-500", "Recurso de Clase"),

            ModifierTypeDto.GrantSubclass or ModifierTypeDto.GrantSubclassFeature =>
                ("fa-crown", "bg-yellow-950/40 border-yellow-500/30 text-yellow-400", "Rasgo de Subclase"),

            ModifierTypeDto.GrantFeat or ModifierTypeDto.SelectFeat or ModifierTypeDto.GrantFeatCategory =>
                ("fa-award", "bg-indigo-950/40 border-indigo-500/30 text-indigo-400", "Dote Otorgada"),

            // Grupo Combate y Acciones (Rojo / Naranja)
            ModifierTypeDto.AddDamageBonus or ModifierTypeDto.OnHitModifier =>
                ("fa-burst", "bg-red-950/40 border-red-500/30 text-red-400", "Bono de Daño"),

            ModifierTypeDto.ModifyAttacksPerAction or ModifierTypeDto.ModifyAttacksPerAttackAction =>
                ("fa-hand-fist", "bg-orange-950/40 border-orange-500/30 text-orange-400", "Ataques Múltiples"),

            ModifierTypeDto.GrantAdvantage =>
                ("fa-dice-d20", "bg-lime-950/40 border-lime-500/30 text-lime-400", "Ventaja Mecánica"),

            ModifierTypeDto.EnableOption or ModifierTypeDto.PassiveEffect =>
                ("fa-toggle-on", "bg-sky-950/40 border-sky-500/30 text-sky-400", "Efecto Pasivo"),

            ModifierTypeDto.ReactionEffect or ModifierTypeDto.Reaction =>
                ("fa-bolt-lightning", "bg-violet-950/40 border-violet-500/30 text-violet-400", "Efecto de Reacción"),

            // Grupo Magia y Conjuros (Púrpura)
            ModifierTypeDto.GrantSpell or ModifierTypeDto.GrantAlwaysPreparedSpell or ModifierTypeDto.AlwaysPreparedSpell or ModifierTypeDto.AlwaysPreparedSpells or ModifierTypeDto.EnableSpellcasting or ModifierTypeDto.EnableBondedMagic =>
                ("fa-wand-sparkles", "bg-purple-950/40 border-purple-500/30 text-purple-400", "Mecánica Mágica"),

            // Grupo Competencias, Idiomas y Utilidad (Teal / Rosa)
            ModifierTypeDto.GrantProficiency or ModifierTypeDto.GrantSaveProficiency or ModifierTypeDto.GrantExpertise =>
                ("fa-graduation-cap", "bg-teal-950/40 border-teal-500/30 text-teal-400", "Competencia / Pericia"),

            ModifierTypeDto.GrantLanguage or ModifierTypeDto.GrantLanguages =>
                ("fa-language", "bg-rose-950/40 border-rose-500/30 text-rose-400", "Idioma Aprendido"),

            ModifierTypeDto.GrantSense =>
                ("fa-eye", "bg-pink-950/40 border-pink-500/30 text-pink-400", "Sentido Especial"),

            // Fallback por defecto
            _ => ("fa-circle-question", "bg-slate-950/40 border-slate-700/30 text-slate-400", "Modificador")
        };
    }
}