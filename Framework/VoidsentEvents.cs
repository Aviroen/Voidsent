using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Voidsent.Framework;
public class VoidsentEvents : ICustomEventScript
{
    private List<TemporaryAnimatedSprite> tempSprites = new List<TemporaryAnimatedSprite>();
    private Vector2 centerScreen;
    //look at carolinetea
    /*
    private currentCustomEventScript(string name, delegate action)
    {
        @event.currentCustomEventScript = new VoidsentEvent(new Vector2(-64000f, -64000f), @event);
        // optional: might as well do fade to clear here
        Game1.globalFadeToClear(null, 0.01f);
    }
    */
    public static void Introduction()
    {

    }
    public virtual void draw(SpriteBatch b)
    {

    }

    public void drawAboveAlwaysFront(SpriteBatch b)
    {

    }

    public bool update(GameTime time, Event e)
    {
        return false;
    }
}
