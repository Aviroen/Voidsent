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
    public float randomStackOffset;

    [XmlIgnore]
    public NetEvent1Field<Vector2, NetVector2> attackedEvent = new NetEvent1Field<Vector2, NetVector2>();

    [XmlElement("leftDrift")]
    public readonly NetBool leftDrift = new NetBool();

    [XmlElement("specialNumber")]
    public readonly NetInt specialNumber = new NetInt();

    protected override void initNetFields()
    {
        base.initNetFields();
        base.NetFields.AddField();
        this.attackedEvent.onEvent += OnAttacked;
    }
    public BBEG(Vector2 position)
        : base("Aviroen.Voidsent_Konryn", position)
    {
        if (Game1.random.NextBool())
        {
            this.leftDrift.Value = true;
        }
        base.Slipperiness = 4;
        base.flip = Game1.random.NextDouble() < 0.45;
        base.HideShadow = false;
    }
    public BBEG(Vector2 position, GameLocation location)
        : base("Aviroen.Voidsent_Konryn", position)
    {
        this.randomStackOffset = Utility.RandomFloat(0f, 100f);
        base.flip = Game1.random.NextBool();
        this.specialNumber.Value = Game1.random.Next(100);
        if (Game1.MasterPlayer.mailReceived.Contains("Aviroen.Voidsent_ExcaliburMail"))
        {
            base.hasSpecialItem.Value = true;
            base.Health = 100000;
            base.DamageToFarmer *= 15;
        }
    }
    public void PhaseOne()
    {

    }
    public void PhaseTwo()
    {

    }
    public void PhaseThree()
    {

    }
    public virtual void OnAttacked(Vector2 trajectory)
    {
        if (Game1.IsMasterGame)
        {
            if (trajectory.LengthSquared() == 0f)
            {
                trajectory = new Vector2(04, -1f);
            }
            else
            {
                trajectory.Normalize();
            }
            trajectory *= 16f;
            BasicProjectile projectile = new BasicProjectile(base.DamageToFarmer / 3 * 2, 13, 3, 0, (float)Math.PI / 16f, trajectory.X, trajectory.Y, base.Position, null, null, null, explode: true, damagesMonsters: false, base.currentLocation, this);
            projectile.height.Value = 24f;
            projectile.color.Value = this.color.Value;
            projectile.ignoreMeleeAttacks.Value = true;
            projectile.hostTimeUntilAttackable = 0.1f;
            if (Game1.random.NextBool())
            {
                projectile.debuff.Value = "13";
            }
            base.currentLocation.projectiles.Add(projectile);
        }
    }
    public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
    {
        int actualDamage = Math.Max(1, damage - base.resilience.Value);
        if (Game1.random.NextDouble() < base.missChance.Value - base.missChance.Value * addedPrecision)
        {
            actualDamage = -1;
        }
        else
        {
            if (Game1.random.NextDouble() < 0.025)
            {
                if (!base.focusedOnFarmers)
                {
                    base.DamageToFarmer += base.DamageToFarmer / 2;
                    base.shake(1000);
                }
                base.focusedOnFarmers = true;
            }
            base.Slipperiness = 5;
            base.Health -= actualDamage;
            base.setTrajectory(xTrajectory, yTrajectory);
            base.currentLocation.playSound("clank");
            this.readyToJump = -1;
            if (base.Health <= 0)
            {
                base.currentLocation.playSound("");
                Game1.stats.MonstersKilled++;
            }
        }
        return actualDamage;
    }
}
