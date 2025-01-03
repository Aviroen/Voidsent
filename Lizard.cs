using StardewValley.Characters;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace Voidsent
{
    internal class Lizard : Horse
    {
        public Lizard()
        {
            ateCarrotToday = false;
            base.willDestroyObjectsUnderfoot = false;
            base.HideShadow = true;
            base.drawOffset = new Vector2(-16f, 0f);
            this.onFootstepAction = PerformDefaultHorseFootstep;
            this.faceDirection(3);
            base.Breather = false;
            base.initNetFields();
        }
        public override void ChooseAppearance(LocalizedContentManager? content = null)
        {
            if (this.Sprite == null)
            {
                this.Sprite = new AnimatedSprite("Animals\\Aviroen.VoidsentCP_Komodo", 0, 32, 32);
                this.Sprite.textureUsesFlippedRightForLeft = true;
                this.Sprite.loop = true;
            }
        }

        public new bool TryFindStable(out GameLocation location, out Stable stable)
        {
            GameLocation? foundLocation = null;
            Stable? foundStable = null;
            Utility.ForEachLocation(delegate (GameLocation curLocation)
            {
                foreach (Building building in curLocation.buildings)
                {
                    if (building is Stable stable2 && stable2.HorseId == this.HorseId && !stable2.isUnderConstruction())
                    {
                        foundLocation = curLocation;
                        foundStable = stable2;
                        if (curLocation.IsActiveLocation())
                        {
                            return false;
                        }
                    }
                }
                return true;
            });
            location = foundLocation!;
            stable = foundStable!;
            return stable != null;
        }
    }
}
