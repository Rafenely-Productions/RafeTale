using System;
using System.Text.Json;
using Rafedream.Domain.Enums;
using Rafedream.Domain.Interfaces;
using Rafedream.Domain.Modifiers;

namespace Rafedream.Domain.Entities;

public class Feat : IEntity
{
    public Guid Id { get; set; }
    public string TechnicalName { get; set; } = string.Empty;
    public CategoryFeat Category { get; set; }
    bool SpecialData = false;
    public List<FeatPrerequisiteModifierData> Prerequisite { get; set; } = new();
    public List<ModifierData> Modifiers { get; set; } = new();

}

