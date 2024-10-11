namespace Voidsent.Monsters
{
    public interface ICustomDamage
    {
        /// <summary>
        /// A value that will preserve and/or replace this monster's hardcoded damage values.
        /// </summary>
        int CustomDamage { get; set; }
    }
}
