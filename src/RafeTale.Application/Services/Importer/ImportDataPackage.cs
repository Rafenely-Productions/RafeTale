using RafeTale.Domain.Entities;

namespace RafeTale.Application.Services.Importer
{
    public class ImportDataPackage
    {
        public ICollection<Race> Races { get; set; } = [];
        public ICollection<SubRace> SubRaces { get; set; } = [];
        public List<ClassDefinition> ClassDefinitions { get; set; } = [];
        public List<Spell> Spells { get; set; } = [];

        public List<Character> Characters { get; set; } = [];
        public List<ClassLevelProgression> ClassLevelProgressions { get; set; } = [];
        public List<XpRules> XpRules { get; set; } = [];
        public List<Feat> Feats { get; set; } = [];
        public List<ItemTemplate> Items { get; set; } = [];
        public List<LocalizedContent> LocalizedContents { get; set; } = [];
        public List<Language> Languages { get; set; } = [];
        public List<Trait> Traits { get; set; } = [];
        public List<SpecialTrait> SpecialTraits { get; set; } = [];
        public List<Skill> SkillProficiencies { get; set; } = [];
        public List<Background> Backgrounds { get; set; } = [];
        public List<Subclass> Subclasses { get; set; } = [];
        public List<SubclassLevelProgression> SubclassLevelProgressions { get; set; } = [];
    }
}
