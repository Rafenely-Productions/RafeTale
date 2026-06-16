using DnDreams.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDreams.Application.Services.Importer
{
    public class ImportDataPackage
    {
        public List<Race> Races { get; set; } = new();
        public List<SubRace> SubRaces { get; set; } = new();
        public List<ClassDefinition> ClassDefinitions { get; set; } = new();
        public List<Spell> Spells { get; set; } = new();

        public List<Character> Characters { get; set; } = new();
        public List<ClassLevelProgression> ClassLevelProgressions { get; set; } = new();
        public List<XpRules> XpRules { get; set; } = new();
        public List<Feat> Feats { get; set; } = new();
        public List<ItemTemplate> Items { get; set; } = new();
        public List<LocalizedContent> LocalizedContents { get; set; } = new();
        public List<Language> Languages { get; set; } = new();
        public List<Trait> Traits { get; set; } = new();
        public List<SpecialTrait> SpecialTraits { get; set; } = new();
        public List<Skill> SkillProficiencies { get; set; } = new();
        public List<Background> Backgrounds { get; set; } = new();
        public List<Subclass> Subclasses { get; set; } = new();
        public List<SubclassLevelProgression> SubclassLevelProgressions { get; set; } = new();
    }
}
