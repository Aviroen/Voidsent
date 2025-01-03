using StardewValley;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System.Xml.Serialization;
using StardewValley.Extensions;
using StardewValley.Projectiles;

namespace Voidsent.Monsters;
public class BBEG : Monster
{
    //CREATE SMOKE EFFECT FROM THE CAULDRON TO FOLLOW THE POSITION OF THE BOSS
    /*
     * make 3 separate methods corresponding to each phase
     * subdivide further for attacks
     * behaviorAtGameTick advance state if hp is below %
     * call relevant phase method
     * 8 frames per 50ms
     * don't tie behavior to animation
     */

    public BBEG()
    {
    }
    protected override void initNetFields()
    {
        base.initNetFields();
    }
    public BBEG(Vector2 position)
        : base("Aviroen.Voidsent_Konryn", position)
    {
        this.Sprite.SpriteWidth = 64;
        this.Sprite.SpriteHeight = 128;
        this.Sprite.UpdateSourceRect();
        base.ignoreDamageLOS.Value = true;
        base.isHardModeMonster = new NetBool(value: true);
        base.Slipperiness = 4;
        base.HideShadow = false;
        base.Breather = true;
        base.initNetFields();
        base.flip = Game1.random.NextBool();
        base.wildernessFarmMonster = false;
        base.objectsToDrop.Add("Aviroen.Voidsent_ITEMHERE");
        base.MaxHealth = 100000;
        base.DamageToFarmer *= 15;
        if (Game1.player.mailReceived.Contains("Aviroen.Voidsent_ExcaliburMail"))
        {
            base.Health = (int)(100000 - Game1.player.stats.Get("Aviroen.VoidsentCP_Fighters") * 2000);
        }
    }
    public void PhaseOne(GameTime time)
    {
        /*
         * 1000ms reaction speed
         * shoots projectiles (non-heat seeking) at player
         * spin animation (touch damage)
         * lunge (slime shiver and lunge)
         */
    }
    public void PhaseTwo(GameTime time)
    {
        /*
         * 500ms reaction speed
         * shoots projectiles at player
         * spin animation (touch damage)
         * lunge (slime shiver and lunge, further range)
         * hand slam animation
         */
        base.updateAnimation(time);
    }
    public void PhaseThree(GameTime time)
    {
        /*
         * 200ms reaction speed
         * shoots projectiles (non-heat seeking) at player
         * lunge (slime shiver and lunge, furthest range)
         * fist slam (extremely high damage)
         */
        base.updateAnimation(time);
    }
}
