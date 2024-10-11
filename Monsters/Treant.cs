using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using Netcode;

namespace Voidsent.Monsters
{
    public class TreantVS : RockGolem
    {
        public TreantVS()
            : base()
        { }
        public TreantVS(Vector2 position)
            : base(position)
        {
            Sprite.currentFrame = 16;
            Sprite.loop = false;
            Sprite.UpdateSourceRect();
        }
        /// <summary>Creates an instance of Stardew's RockGolem class (Wilderness/Iridium Golem subtypes), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="difficultyMod">The difficulty modifier to use. Affects health, damage, experience gained, loot items, and subtype (see remarks).</param>
        /// <param name="forceIridiumGolem">If true, an iridium golem will be generated. Otherwise, the result will unmodified from the original constructor (see remarks).</param>
        /// <remarks>
        /// As of SDV v1.6, if difficultyMod is 9 or higher and the player has the Wilderness farm type, there is a 50% chance that the base constructor (Vector2, int) will create an "Iridium Golem". Otherwise, it will create a "Wilderness Golem".
        /// </remarks>
        public TreantVS(Vector2 position, int difficultyMod, bool forceTreantVS = true)
            : base(position, difficultyMod)
        {
            if (forceTreantVS && Name != "TreantVS")
            {
                Sprite = new AnimatedSprite("Characters\\Monsters\\TreantVS");
                Name = "Treant";

                parseMonsterInfo(Name);

                IsWalkingTowardPlayer = false;
                Slipperiness = 1;
                HideShadow = true;
                jitteriness.Value = 0.0;
                DamageToFarmer += difficultyMod;
                Health += (int)(difficultyMod * difficultyMod * 2f);
                ExperienceGained += difficultyMod;
                if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.05)
                {
                    objectsToDrop.Add("749"); //fix these values to custom items
                }
                if (difficultyMod >= 5 && Game1.random.NextDouble() < 0.2)
                {
                    objectsToDrop.Add("770"); //fix these values to custom items
                }
                if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
                {
                    objectsToDrop.Add("386"); //fix these values to custom items
                }
                if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.01)
                {
                    objectsToDrop.Add("386"); //fix these values to custom items
                }
                if (difficultyMod >= 10 && Game1.random.NextDouble() < 0.001)
                {
                    objectsToDrop.Add("74"); //fix these values to custom items
                }
                Speed *= 1;
                Health += 400;
                DamageToFarmer += 10;
                ExperienceGained += 10;
                if (Game1.random.NextDouble() < 0.03)
                {
                    objectsToDrop.Add("337"); //fix these values to custom items
                }
                if (Game1.random.NextDouble() < 0.03)
                {
                    objectsToDrop.Add("337"); //fix these values to custom items
                }
                Sprite.currentFrame = 16;
                Sprite.loop = false;
                Sprite.UpdateSourceRect();
            }
        }
        public TreantVS(Vector2 position, bool alreadySpawned)
            : base(position)
        {
            if (alreadySpawned)
            {
                IsWalkingTowardPlayer = true;
                seenPlayer.Value = true;
                moveTowardPlayerThreshold.Value = 1;
            }
            else
            {
                IsWalkingTowardPlayer = false;
            }
            Sprite.loop = false;
            Slipperiness = 1;
        }
    }
}
