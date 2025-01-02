using StardewValley.Characters;
using Microsoft.Xna.Framework;
using StardewValley;
using Netcode;

namespace Voidsent
{
    internal class Lizard : Horse
    {
        private readonly NetGuid lizardId = new NetGuid();
        public Guid LizardId
        {
            get
            {
                return this.lizardId.Value;
            }
            set
            {
                this.lizardId.Value = value;
            }
        }
        public Lizard()
        {
            base.willDestroyObjectsUnderfoot = false;
            base.HideShadow = true;
            base.drawOffset = new Vector2(-16f, 0f);
            this.onFootstepAction = PerformDefaultHorseFootstep;
            this.ChooseAppearance();
            this.faceDirection(3);
            base.Breather = false;
        }
        public Lizard(Guid lizardId, int xTile, int yTile)
            :this()
        {
            base.Name = "";
            this.displayName = base.Name;
            base.Position = new Vector2(xTile, yTile) * 64f;
            base.currentLocation = Game1.currentLocation;
            this.LizardId = lizardId;
        }
        public override void reloadData()
        {
         
        }
        protected override string translateName()
        {
            return base.Name.Trim();
        }
        public override bool canTalk()
        {
            return false;
        }
    }
}
