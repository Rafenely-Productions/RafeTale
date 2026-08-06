using System;
using System.Text.Json;
using RafeTale.Domain.Enums;
using RafeTale.Domain.Interfaces;
using RafeTale.Domain.Modifiers;

namespace RafeTale.Domain.Entities;

public class Feat : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public CategoryFeat Category { get; set; }
    bool SpecialData = false;
    public List<FeatPrerequisiteModifierData> Prerequisite { get; set; } = new();
    public List<ModifierData> Modifiers { get; set; } = new();

}

