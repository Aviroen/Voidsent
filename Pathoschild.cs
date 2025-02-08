using StardewModdingAPI;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using System.Reflection.Metadata;

namespace Voidsent
{
    /*The MIT License (MIT)

Copyright 2016–2021 Jesse Plamondon-Willard (Pathoschild)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
     */
    internal class Pathoschild
    {
        private static IModRegistry ModRegistry { get; set; }
        private static string ModID { get; set; }
        public const string Bookshelf = "Bookshelf";
        public const string Kitchen = "Kitchen";
        public const string Dresser = "Dresser";
        public const string Dining = "Dining";
        public const string Cabinet = "Cabinet";
        public const string TV = "TV";
        public const string Clock = "Clock";
        public const string Coat = "Coat";
        public const string Misc = "Misc";
        public static void Initialize(IModRegistry registry, string modID)
        {
            ModRegistry = registry;
            ModID = modID;
        }
        public static bool OnCentralAction(GameLocation location, string[] args, Farmer who, Point tile)
        {
           // if (location.NameOrUniqueName is not $"Aviroen.VoidsentCP_")
                //return false;

            string subAction = ArgUtility.Get(args, 1);
            switch (subAction)
            {
                case Bookshelf:
                    return BookshelfBool();
                case Kitchen:
                    return KitchenBool();
                case Dresser:
                    return DresserBool();
                case Dining:
                    return DiningBool();
                case Cabinet:
                    return CabinetBool();
                case TV:
                    return TVBool();
                case Clock:
                    return ClockBool();
                case Coat:
                    return CoatBool();
                case Misc:
                    return MiscBool();

                default:
                    return false;
            }
        }
        public static bool BookshelfBool()
        {
            string message = GetBookshelfMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetBookshelfMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Bookshelf"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetBookshelfMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool KitchenBool()
        {
            string message = GetKitchenMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetKitchenMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Kitchen"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetKitchenMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool DresserBool()
        {
            string message = GetDresserMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetDresserMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Dresser"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetDresserMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool DiningBool()
        {
            string message = GetDiningMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetDiningMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Dining"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetDiningMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool CabinetBool()
        {
            string message = GetCabinetMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetCabinetMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Cabinet"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetCabinetMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool TVBool()
        {
            string message = GetTVMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetTVMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/TV"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetTVMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool ClockBool()
        {
            string message = GetClockMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetClockMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Clock"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetClockMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool CoatBool()
        {
            string message = GetCoatMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetCoatMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Coat"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetCoatMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
        public static bool MiscBool()
        {
            string message = GetMiscMessage();

            if (!string.IsNullOrWhiteSpace(message))
            {
                Game1.drawDialogueNoTyping(message);
                return true;
            }

            return false;
        }
        public static string GetMiscMessage()
        {
            HashSet<string> seenMessages = new();

            // get available options
            List<string> options = new();
            foreach ((string id, List<string>? dialogues) in Game1.content.Load<Dictionary<string, List<string>?>>($"Mods/{ModID}/Misc"))
            {
                options.AddRange(dialogues ?? []);
            }
            options.RemoveAll(option => string.IsNullOrWhiteSpace(option) || seenMessages.Contains(option));

            // if we've seen them all, reset
            if (options.Count == 0 && seenMessages.Count > 0)
            {
                seenMessages.Clear();
                return GetMiscMessage();
            }

            // choose one
            string selected = Game1.random.ChooseFrom(options);
            seenMessages.Add(selected);
            return selected;
        }
    }
}
