using Discord;

namespace Interaction.Utilities {
    //Constants used for emojis.
    // 
    //Find more at: https://emojipedia.org/symbols/

    #region Play & Navigation
    public static partial class Emojis {
        public static readonly Emoji Forward = new Emoji("▶️");
        public static readonly Emoji Fast_Forward = new Emoji("⏩");
        public static readonly Emoji Skip_Forward = new Emoji("⏭️");

        public static readonly Emoji Reverse = new Emoji("◀️");
        public static readonly Emoji Fast_Reverse = new Emoji("⏪");
        public static readonly Emoji Skip_Reverse = new Emoji("⏮️");

        public static readonly Emoji Shuffle = new Emoji("🔀");
        public static readonly Emoji Repeat = new Emoji("🔁");
        public static readonly Emoji Repeat_Single = new Emoji("🔂");

        public static readonly Emoji Play_Or_Pause = new Emoji("⏯️");
        public static readonly Emoji Pause = new Emoji("⏸️");
        public static readonly Emoji Stop = new Emoji("⏹️");
        public static readonly Emoji Record = new Emoji("⏺️");
        public static readonly Emoji Eject = new Emoji("⏏️");
    }
    #endregion

    #region Warnings and Checks
    public static partial class Emojis {
        public static readonly Emoji Warning = new Emoji("⚠️");
        public static readonly Emoji No_Entry = new Emoji("⛔");
        public static readonly Emoji Prohibited = new Emoji("🚫");
        public static readonly Emoji NSFW = new Emoji("🔞");
        public static readonly Emoji Check_Mark_Button = new Emoji("✅");
        public static readonly Emoji Check_Box_Button = new Emoji("☑️");
        public static readonly Emoji Check_Mark = new Emoji("✔️");
        public static readonly Emoji Cross_Mark = new Emoji("❌");
        public static readonly Emoji Ok = new Emoji("🆗");
    }
    #endregion

    #region Keys & Numbers
    public static partial class Emojis {
        public static readonly Emoji Keycap_Number_Sign = new Emoji("#️⃣");
        public static readonly Emoji Keycap_Asterisk = new Emoji("*️⃣");
        public static readonly Emoji Keycap_Zero = new Emoji("0️⃣");
        public static readonly Emoji Keycap_One = new Emoji("1️⃣");
        public static readonly Emoji Keycap_Two = new Emoji("2️⃣");
        public static readonly Emoji Keycap_Three = new Emoji("3️⃣");
        public static readonly Emoji Keycap_Four = new Emoji("4️⃣");
        public static readonly Emoji Keycap_Five = new Emoji("5️⃣");
        public static readonly Emoji Keycap_Six = new Emoji("6️⃣");
        public static readonly Emoji Keycap_Seven = new Emoji("7️⃣");
        public static readonly Emoji Keycap_Eight = new Emoji("8️⃣");
        public static readonly Emoji Keycap_Nine = new Emoji("9️⃣");
        public static readonly Emoji Keycap_Ten = new Emoji("🔟");
        public static readonly Emoji Input_Number = new Emoji("🔢");
        public static readonly Emoji Input_Symbols = new Emoji("🔣");
    }
    #endregion

    #region Letters
    public static partial class Emojis {
        public static readonly Emoji Keycap_Letter_A = new Emoji("🅰");
        public static readonly Emoji Keycap_Letter_B = new Emoji("🅱");
        public static readonly Emoji Keycap_Letter_C = new Emoji("🅲");
        public static readonly Emoji Keycap_Letter_D = new Emoji("🅳");
        public static readonly Emoji Keycap_Letter_E = new Emoji("🅴");
        public static readonly Emoji Keycap_Letter_F = new Emoji("🅵");
        public static readonly Emoji Keycap_Letter_G = new Emoji("🅶");
        public static readonly Emoji Keycap_Letter_H = new Emoji("🅷");
        public static readonly Emoji Keycap_Letter_I = new Emoji("🅸");
        public static readonly Emoji Keycap_Letter_J = new Emoji("🅹");
        public static readonly Emoji Keycap_Letter_K = new Emoji("🅺");
        public static readonly Emoji Keycap_Letter_L = new Emoji("🅻");
        public static readonly Emoji Keycap_Letter_M = new Emoji("🅼");
        public static readonly Emoji Keycap_Letter_N = new Emoji("🅽");
        public static readonly Emoji Keycap_Letter_O = new Emoji("🅾");
        public static readonly Emoji Keycap_Letter_P = new Emoji("🅿");
        public static readonly Emoji Keycap_Letter_Q = new Emoji("🆀");
        public static readonly Emoji Keycap_Letter_R = new Emoji("🆁");
        public static readonly Emoji Keycap_Letter_S = new Emoji("🆂");
        public static readonly Emoji Keycap_Letter_T = new Emoji("🆃");
        public static readonly Emoji Keycap_Letter_U = new Emoji("🆄");
        public static readonly Emoji Keycap_Letter_V = new Emoji("🆅");
        public static readonly Emoji Keycap_Letter_W = new Emoji("🆆");
        public static readonly Emoji Keycap_Letter_X = new Emoji("🆇");
        public static readonly Emoji Keycap_Letter_Y = new Emoji("🆈");
        public static readonly Emoji Keycap_Letter_Z = new Emoji("🆉");
    }
    #endregion

    #region Other
    public static partial class Emojis {
        public static readonly Emoji Recycle = new Emoji("♻️");
        public static readonly Emoji Information = new Emoji("ℹ️");
        public static readonly Emoji Question_Mark = new Emoji("❓");
        public static readonly Emoji Skull = new Emoji("💀");

        public static readonly Emoji School = new Emoji("🏫");
        public static readonly Emoji Rainbow = new Emoji("🌈");
    }
    #endregion

    #region Math
    public static partial class Emojis {
        public static readonly Emoji Divide = new Emoji("➗");
        public static readonly Emoji Minus = new Emoji("➖");
        public static readonly Emoji Multiply = new Emoji("✖️");
        public static readonly Emoji Plus = new Emoji("➕");
    }
    #endregion

    #region Items
    public static partial class Emojis {
        public static readonly Emoji Backpack = new Emoji("🎒");
        public static readonly Emoji Ticket = new Emoji("🎫");
        public static readonly Emoji Notebook = new Emoji("📓");
        public static readonly Emoji Glasses = new Emoji("👓");
        public static readonly Emoji Clipboard = new Emoji("📋");
        public static readonly Emoji Telephone = new Emoji("☎️");
        public static readonly Emoji TelephoneReceiver = new Emoji("📞");
        public static readonly Emoji Keyboard = new Emoji("⌨️");
    }
    #endregion

    #region Awards
    public static partial class Emojis {
        public static readonly Emoji MedalGold = new Emoji("🥇");
        public static readonly Emoji MedalSilver = new Emoji("🥈");
        public static readonly Emoji MedalBronze = new Emoji("🥉");
        public static readonly Emoji Trophy = new Emoji("🏆");
        public static readonly Emoji MedalSports = new Emoji("🏅");
        public static readonly Emoji Crown = new Emoji("👑");
        public static readonly Emoji HundredPoints = new Emoji("💯");
    }
    #endregion

    #region Medical
    public static partial class Emojis {
        public static readonly Emoji BloodDrop = new Emoji("🩸");
        public static readonly Emoji Pill = new Emoji("💊");
        public static readonly Emoji Syringe = new Emoji("💉");
        public static readonly Emoji Hospital = new Emoji("🏥");
        public static readonly Emoji Microbe = new Emoji("🦠");
        public static readonly Emoji Mosquito = new Emoji("🦟");
        public static readonly Emoji Stethoscope = new Emoji("🩺");
        public static readonly Emoji Thermometer = new Emoji("🌡️");
        public static readonly Emoji FaceMask = new Emoji("😷");
        public static readonly Emoji Ambulance = new Emoji("🚑");
        public static readonly Emoji FaceVomiting = new Emoji("🤮");
        public static readonly Emoji NauseatedFace = new Emoji("🤢");
        public static readonly Emoji SneezingFace = new Emoji("🤧");
        public static readonly Emoji FaceSick = new Emoji("🤒");
    }
    #endregion

    #region Military, Violence & Crime
    public static partial class Emojis {
        public static readonly Emoji CrossedSwords = new Emoji("⚔️");
        public static readonly Emoji Pistol = new Emoji("🔫");
        public static readonly Emoji Bomb = new Emoji("💣");
        public static readonly Emoji Guard = new Emoji("💂");
        public static readonly Emoji PirateFlag = new Emoji("🏴‍☠️");
        public static readonly Emoji Detective = new Emoji("🕵️");
        public static readonly Emoji Dagger = new Emoji("🗡️");
        public static readonly Emoji Shield = new Emoji("🛡️");
        public static readonly Emoji Dove = new Emoji("🕊️");
    }
    #endregion

    #region Signs & Buttons
    public static partial class Emojis {
        public static readonly Emoji Radioactive = new Emoji("☢️");
        public static readonly Emoji Biohazard = new Emoji("☣️");
        public static readonly Emoji SignStop = new Emoji("🛑");
        public static readonly Emoji ButtonNew = new Emoji("🆕");
        public static readonly Emoji ButtonCool = new Emoji("🆒");
        public static readonly Emoji ButtonID = new Emoji("🆔");
        public static readonly Emoji ButtonFree = new Emoji("🆓");
        public static readonly Emoji ButtonSOS = new Emoji("🆘");
        public static readonly Emoji ButtonOK = new Emoji("🆗");
    }
    #endregion

    #region Utility
    public static partial class Emojis {
        public static readonly Emoji Wrench = new Emoji("🔧");
        public static readonly Emoji Toolbox = new Emoji("🧰");
        public static readonly Emoji HammerAndWrench = new Emoji("🛠️");
        public static readonly Emoji HammerAndPick = new Emoji("⚒️");
        public static readonly Emoji Pick = new Emoji("⛏️");
        public static readonly Emoji Hammer = new Emoji("🔨");
        public static readonly Emoji Gear = new Emoji("⚙️");

    }
    #endregion

    #region Time & Sleep
    public static partial class Emojis {
        public static readonly Emoji AlarmClock = new Emoji("⏰");
        public static readonly Emoji HourglassNotDone = new Emoji("⏳");
        public static readonly Emoji HourglassDone = new Emoji("⌛");
        public static readonly Emoji Stopwatch = new Emoji("⏱️");
        public static readonly Emoji Compass = new Emoji("🧭");
        public static readonly Emoji Calendar = new Emoji("📅");
        public static readonly Emoji Zzz = new Emoji("💤");
        public static readonly Emoji SleepingFace = new Emoji("😴");
        public static readonly Emoji SleepyFace = new Emoji("😪");
        public static readonly Emoji Bed = new Emoji("🛏️");
    }
    #endregion

    #region Economics
    public static partial class Emojis {
        public static readonly Emoji Dollar = new Emoji("💲");
        public static readonly Emoji CurrencyExchange = new Emoji("💱");
        public static readonly Emoji MoneyBag = new Emoji("💰");
        public static readonly Emoji MoneyFace = new Emoji("🤑");
        public static readonly Emoji MoneyWithWings = new Emoji("💸");
        public static readonly Emoji Bank = new Emoji("🏦");
        public static readonly Emoji ChartIncreasing = new Emoji("📈");
        public static readonly Emoji ChartDecreasing = new Emoji("📉");
        public static readonly Emoji ChartBar = new Emoji("📊");
        public static readonly Emoji Rocket = new Emoji("🚀");
        public static readonly Emoji GemStone = new Emoji("💎");
        public static readonly Emoji Briefcase = new Emoji("💼");
        public static readonly Emoji OfficeBuilding = new Emoji("🏢");
    }
    #endregion

    #region Shopping & Work
    public static partial class Emojis {
        public static readonly Emoji ShoppingBags = new Emoji("🛍️");
        public static readonly Emoji Label = new Emoji("🏷️");
        public static readonly Emoji Receipt = new Emoji("🧾");
        public static readonly Emoji Purse = new Emoji("👛");
        public static readonly Emoji CreditCard = new Emoji("💳");
        public static readonly Emoji Cart = new Emoji("🛒");
        public static readonly Emoji DepartmentStore = new Emoji("🏬");
    }
    #endregion

    #region Gambling & Games
    public static partial class Emojis {
        public static readonly Emoji EightBall = new Emoji("🎱");
        public static readonly Emoji CrystalBall = new Emoji("🔮");
        public static readonly Emoji Joker = new Emoji("🃏");
        public static readonly Emoji Puzzle = new Emoji("🧩");
        public static readonly Emoji VideoGame = new Emoji("🎮");
        public static readonly Emoji SlotMachine = new Emoji("🎰");
        public static readonly Emoji GameDie = new Emoji("🎲");
        public static readonly Emoji CloverFour = new Emoji("🍀");
        public static readonly Emoji CloverThree = new Emoji("☘️");
    }
    #endregion

    #region Authority & Restriction
    public static partial class Emojis {
        public static readonly Emoji Eye = new Emoji("👁️");
        public static readonly Emoji Key = new Emoji("🔑");
        public static readonly Emoji Locked = new Emoji("🔒");
        public static readonly Emoji Unlocked = new Emoji("🔓");
        public static readonly Emoji KeyOld = new Emoji("🗝️");
    }
    #endregion

    #region General categories & actions
    public static partial class Emojis {
        public static readonly Emoji Newspaper = new Emoji("📰");
        public static readonly Emoji MagnifyingGlass = new Emoji("🔍");
        public static readonly Emoji PerformingArts = new Emoji("🎭");
        public static readonly Emoji ForkAndKnifeWithPlate = new Emoji("🍽️");
        public static readonly Emoji WorldMap = new Emoji("🗺️");
        public static readonly Emoji Car = new Emoji("🚗");
    }
    #endregion

    #region Gestures
    public static partial class Emojis {
        public static readonly Emoji SmallPenis = new Emoji("🤏");
        public static readonly Emoji OKHand = new Emoji("👌");
        public static readonly Emoji ThumbsUp = new Emoji("👍");
        public static readonly Emoji WritingHand = new Emoji("✍️");
        public static readonly Emoji WavingHand = new Emoji("👋");
        public static readonly Emoji Sparkles = new Emoji("✨");
        public static readonly Emoji Dizzy = new Emoji("💫");
        public static readonly Emoji StarGlowing = new Emoji("🌟");
        public static readonly Emoji Star = new Emoji("⭐");
        public static readonly Emoji Fire = new Emoji("🔥");
        public static readonly Emoji HighVoltage = new Emoji("⚡");
        public static readonly Emoji ThinkingFace = new Emoji("🤔");
    }
    #endregion

    public static partial class Emojis {

    }
}