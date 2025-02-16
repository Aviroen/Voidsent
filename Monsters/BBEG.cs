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
     * the main thing to understand is that u r decide what do in the Update method
and ur state is either "doing something rn" or "just got done doing something"
in this context even "stand around so player can hit me" is "doing something rn"
    you get to pick "what do next" when you enter the "just got done doing something" state
i suppose there is also the secret third state of "oh i didn't get to do what i wanted to do" that is essentialy same as "just got done doing something"
    this would be case where boss hit 50% hp left, interrupt previous doings and immediately pick next thing
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
