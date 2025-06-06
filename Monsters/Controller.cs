using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;

namespace Voidsent.Monsters;
//https://gist.github.com/Mushymato/fb0cbe103f2aab19667c9a216142315e
/*
 * but yes u can see that 2 actions ("Waiting", "Oh yeah!") are loaded
 * each last 4s
 * and whenever we r not in countdown, try to pick the next action to do
 * on successful pick the Perform delegate is called
 * Perform should really be called something like ActStart, with corresponding ActEnd and prob an Update for stuff like projectile
 */

public record BossAct(
    string Name,
    TimeSpan Duration,
    int Priority,
    Func<BossBase, bool> Check,
    Action<BossBase, GameTime> Perform
);

public class BossBase : Monster
{
    protected BossAct? CurrAct = null;
    protected TimeSpan CurrActTimer = TimeSpan.Zero;
    protected List<BossAct> BossActs = [];

    public BossBase() { }

    public BossBase(string name, Vector2 position)
        : base(name, position) { }

    public virtual void LoadActs()
    {
        BossActs.Add(
            new(
                "Waiting",
                TimeSpan.FromSeconds(4),
                0,
                (mons) => Random.Shared.NextDouble() < 0.2,
                (mons, time) => Console.WriteLine("Waiting")
            )
        );
        BossActs.Add(
            new(
                "Oh yeah!",
                TimeSpan.FromSeconds(4),
                0,
                (mons) => Random.Shared.NextDouble() < 0.6,
                (mons, time) => Console.WriteLine("Oh yeah!")
            )
        );
    }

    public override void reloadSprite(bool onlyAppearance = false)
    {
        // placeholder, angry roger
        Sprite = new AnimatedSprite("Characters\\Monsters\\Angry Roger", 0, 64, 64);
    }

    public override void draw(SpriteBatch b)
    {
        // placeholder, simply draw the monster
        b.Draw(
            Sprite.Texture,
            getLocalPosition(Game1.viewport) + new Vector2(32f, 21 + yOffset),
            Sprite.SourceRect,
            Color.White,
            0f,
            new Vector2(8f, 16f),
            Math.Max(0.2f, scale.Value) * 4f,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            1f - position.X / 10000f
        );
    }

    private void CheckCurrAct(BossAct? newAct, GameTime time)
    {
        if (newAct == null || newAct == CurrAct)
            return;
        CurrAct = newAct;
        CurrActTimer = newAct.Duration;
        CurrAct.Perform(this, time);
        return;
    }

    public override void behaviorAtGameTick(GameTime time)
    {
        base.behaviorAtGameTick(time);
        // Enacting the next Act
        if (CurrAct != null)
        {
            CheckCurrAct(BossActs.FirstOrDefault((act) => act.Priority > CurrAct.Priority && act.Check(this)), time);
        }
        // Picking the next Act
        if (CurrActTimer > TimeSpan.Zero)
        {
            CurrActTimer -= time.ElapsedGameTime;
            if (CurrActTimer <= TimeSpan.Zero)
            {
                CurrAct = null;
                CurrActTimer = TimeSpan.Zero;
            }
            return;
        }
        CheckCurrAct(BossActs.FirstOrDefault((act) => act.Check(this)), time);
    }
}