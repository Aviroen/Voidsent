using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.GameData;
using StardewValley.GameData.SpecialOrders;
using StardewValley.SpecialOrders;
using StardewValley.Extensions;

namespace Voidsent.Custom
{
    internal class VSSpecialOrderBoard : SpecialOrdersBoard
    {
        private static IMonitor Monitor { get; set; }
        private static IModHelper Helper { get; set; }
        int timestampOpened;
        static int safetyTimer = 500;

        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        internal VSSpecialOrderBoard(string board_type = "") : base(board_type)
        {
            SpecialOrder.UpdateAvailableSpecialOrders(board_type, forceRefresh: false);
            boardType = board_type;
            Texture2D texture;
            if (boardType.Equals("Aviroen.VoidsentCP"))
            {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SpecialOrdersBoard");
            }
            else
            {
                texture = Game1.temporaryContent.Load<Texture2D>("LooseSprites\\SpecialOrdersBoard");
            }
            Helper.Reflection.GetField<Texture2D>(this, "billboardTexture").SetValue(texture);
        }
        public static bool OpenVSBoard(GameLocation location, string[] args, Farmer who, Point tile)
        {
            string subAction = ArgUtility.Get(args, 1);
            if (subAction == null)
            {
                return false;
            }
            else
            {
                Game1.activeClickableMenu = new VSSpecialOrderBoard();
                return true;
            }
        }
        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            if (timestampOpened + safetyTimer < Game1.currentGameTime.TotalGameTime.TotalMilliseconds)
            {
                base.receiveRightClick(x, y, playSound);
            }
            return;
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (timestampOpened + safetyTimer > Game1.currentGameTime.TotalGameTime.TotalMilliseconds)
            {
                return;
            }
            base.receiveLeftClick(x, y, playSound);
        }
        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if (leftOrder is null)
            {
                b.DrawString(Game1.dialogueFont, Helper.Translation.Get("Aviroen.VoidsentCP_NoNewOrder"), new Vector2(xPositionOnScreen + 125, yPositionOnScreen + 375), Game1.textColor);
            }
            if (rightOrder is null)
            {
                b.DrawString(Game1.dialogueFont, Helper.Translation.Get("Aviroen.VoidsentCP_NoNewOrder"), new Vector2(xPositionOnScreen + 125, yPositionOnScreen + 375), Game1.textColor);
            }
        }
        public static void UpdateAvailableVSSpecialOrders(string orderType, bool forceRefresh)
        {
            foreach (SpecialOrder order in Game1.player.team.availableSpecialOrders)
            {
                if ((order.questDuration.Value == QuestDuration.TwoDays || order.questDuration.Value == QuestDuration.ThreeDays) && !Game1.player.team.acceptedSpecialOrderTypes.Contains(order.orderType.Value))
                {
                    order.SetDuration(order.questDuration.Value);
                }
            }
            if (!forceRefresh)
            {
                foreach (SpecialOrder availableSpecialOrder in Game1.player.team.availableSpecialOrders)
                {
                    if (availableSpecialOrder.orderType.Value.Equals("Aviroen.Voidsent"))
                    {
                        return;
                    }
                }
            }
            SpecialOrder.RemoveAllSpecialOrders(orderType);
            List<string> keyQueue = new List<string>();
            foreach (KeyValuePair<string, SpecialOrderData> pair in DataLoader.SpecialOrders(Game1.content))
            {
                if (pair.Value.OrderType.Equals("Aviroen.Voidsent") && SpecialOrder.CanStartOrderNow(pair.Key, pair.Value))
                {
                    keyQueue.Add(pair.Key);
                }
            }
            List<string> keysIncludingCompleted = new List<string>(keyQueue);
            if (orderType == "")
            {
                keyQueue.RemoveAll((string id) => Game1.player.team.completedSpecialOrders.Contains(id));
            }
            Random r = Utility.CreateRandom(Game1.uniqueIDForThisGame, (double)Game1.stats.DaysPlayed * 1.3);
            for (int i = 0; i < 2; i++)
            {
                if (keyQueue.Count == 0)
                {
                    if (keysIncludingCompleted.Count == 0)
                    {
                        break;
                    }
                    keyQueue = new List<string>(keysIncludingCompleted);
                }
                string key = r.ChooseFrom(keyQueue);
                Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key, r.Next()));
                keyQueue.Remove(key);
                keysIncludingCompleted.Remove(key);
            }
        }
    }
    /*public static void UpdateAvailableRSVSpecialOrders(bool force_refresh)
		{
			if (Game1.player.team.availableSpecialOrders is not null)
			{
				foreach (SpecialOrder order in Game1.player.team.availableSpecialOrders)
				{
					if ((order.questDuration.Value == QuestDuration.TwoDays || order.questDuration.Value == QuestDuration.ThreeDays) && !Game1.player.team.acceptedSpecialOrderTypes.Contains(order.orderType.Value))
					{
						order.SetDuration(order.questDuration.Value);
					}
				}
			}

			if (force_refresh) Log.Trace("Refreshing RSV Special Orders");
			var availableOrders = Game1.player.team.availableSpecialOrders;

			Game1.player.team.acceptedSpecialOrderTypes.Remove(RSVConstants.Z_VILLAGESPECIALORDER);
			Game1.player.team.acceptedSpecialOrderTypes.Remove(RSVConstants.Z_NINJASPECIALORDER);

			for(int i=0; i< availableOrders.Count; i++)
            {
                if (availableOrders[i].orderType.Equals(RSVConstants.Z_NINJASPECIALORDER) || availableOrders[i].orderType.Equals(RSVConstants.Z_VILLAGESPECIALORDER))
                {
					Game1.player.team.availableSpecialOrders.RemoveAt(i);
					i--;
                }
            }

			Dictionary<string, SpecialOrderData> order_data = Game1.content.Load<Dictionary<string, SpecialOrderData>>("Data\\SpecialOrders");
			List<string> keys = new List<string>(order_data.Keys);

			for (int k = 0; k < keys.Count; k++)
			{
				string key = keys[k];
				if (force_refresh) Log.Trace($"Checking {key}");
				bool invalid = false;
				bool repeatable = order_data[key].Repeatable == true;
				if (repeatable && Game1.MasterPlayer.team.completedSpecialOrders.Contains(key))
				{
					if (force_refresh) Log.Trace($"Not repeatable and already done");
					invalid = true;
				}
				if (Game1.dayOfMonth >= 16 && order_data[key].Duration == QuestDuration.Month)
				{
					if (force_refresh) Log.Trace($"Month SO and after 16th");
					invalid = true;
				}
				if (!invalid && !SpecialOrder.CheckTags(order_data[key].RequiredTags))
				{
					if (force_refresh) Log.Trace($"Tags conditions not met.");
					invalid = true;
				}
				if (!invalid)
				{
					foreach (SpecialOrder specialOrder in Game1.player.team.specialOrders)
					{
						if ((string)specialOrder.questKey.Value == key)
						{
							invalid = true;
							break;
						}
					}
				}
				if (force_refresh) Log.Trace($"Order {keys[k]} is valid: {!invalid}");
				if (invalid)
				{
					keys.RemoveAt(k);
					k--;
				}
			}
			Random r = new Random((int)Game1.uniqueIDForThisGame + (int)(Game1.stats.DaysPlayed * 1.3f));
			string[] array = new string[2] { RSVConstants.Z_NINJASPECIALORDER, RSVConstants.Z_VILLAGESPECIALORDER };
			foreach (string type_to_find in array)
			{
				List<string> typed_keys = new List<string>();
				foreach (string key3 in keys)
				{
					if (order_data[key3].OrderType == type_to_find)
					{
						typed_keys.Add(key3);
					}
				}
				List<string> all_keys = new List<string>(typed_keys);

				for (int j = 0; j < typed_keys.Count; j++)
				{
					if (Game1.player.team.completedSpecialOrders.Contains(typed_keys[j]))
					{
						typed_keys.RemoveAt(j);
						j--;
					}
				}

				for (int i = 0; i < 2; i++)
				{
					if (typed_keys.Count == 0)
					{
						if (all_keys.Count == 0)
						{
							break;
						}
						typed_keys = new List<string>(all_keys);
					}
					int index = r.Next(typed_keys.Count);
					string key2 = typed_keys[index];
					Game1.player.team.availableSpecialOrders.Add(SpecialOrder.GetSpecialOrder(key2, r.Next()));
					typed_keys.Remove(key2);
					all_keys.Remove(key2);
				}
			}

			LogCurrentlyAvailableSpecialOrders();

		}
     * 
     */
}
