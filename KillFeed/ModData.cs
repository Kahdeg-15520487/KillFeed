using System.Text;
using UnityEngine;
using Verse;

namespace KillFeed
{
    class ModData : Mod
    {
        public ModData(ModContentPack content) : base(content)
        {
            ModData.Settings = base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "Killfeed";
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            ModData.Settings.DoWindowContents(canvas);
        }

        public static Settings Settings;
    }

    class Settings : ModSettings
    {
        public bool DisplayWildAnimalDeath;
        public bool DisplayAllyDeath;
        public bool DisplayEnemyDeath;

        public bool UseLeftRightPos;
        public bool DisplayPositionRight;

        public int LeftOffset;
        public int TopOffset;
        private string LeftOffsetBuffer;
        private string RightOffsetBuffer;

        public int Width;
        public int Height;
        private string WidthBuffer;
        private string HeightBuffer;

        public int ExpirationTime;
        private string ExpirationTimeBuffer;

        internal void DoWindowContents(Rect canvas)
        {
            this.listing_Standard = new Listing_Standard();
            this.listing_Standard.Begin(GenUI.ContractedBy(canvas, 60f));
            this.listing_Standard.CheckboxLabeled("Display wild animal's death?", ref DisplayWildAnimalDeath, "Display wild animal's death in kill feed");
            this.listing_Standard.CheckboxLabeled("Display ally's death?", ref DisplayAllyDeath, "Display ally's death in kill feed");
            this.listing_Standard.CheckboxLabeled("Display enemy's death?", ref DisplayEnemyDeath, "Display enemy's death in kill feed");
            this.listing_Standard.GapLine(12f);
            this.listing_Standard.CheckboxLabeled("Display killfeed on up right?", ref DisplayPositionRight, "Whether or not display kill feed on up right. Will reset the offset setting below to default value.");
            this.listing_Standard.Label("if above setting is checked, custom offset will be ignored.");
            this.listing_Standard.CheckboxLabeled("Use custom offset Left/Right position?", ref UseLeftRightPos, "Whether or not display using below left/right custom offset");
            this.listing_Standard.TextFieldNumericLabeled<int>("Left offset", ref LeftOffset, ref LeftOffsetBuffer, 0);
            this.listing_Standard.TextFieldNumericLabeled<int>("Top offset", ref TopOffset, ref RightOffsetBuffer, 0);
            this.listing_Standard.GapLine(12f);
            this.listing_Standard.Label("Width and Height of the notification box");
            this.listing_Standard.TextFieldNumericLabeled<int>("Width", ref Width, ref WidthBuffer, 0);
            this.listing_Standard.TextFieldNumericLabeled<int>("Height", ref Height, ref HeightBuffer, 0);
            this.listing_Standard.GapLine(12f);
            this.listing_Standard.TextFieldNumericLabeled<int>("Killfeed's message appear duration (second)", ref ExpirationTime, ref ExpirationTimeBuffer,1);
            this.listing_Standard.End();
        }

        private void DoSlider(ref int x)
        {
            x = Mathf.RoundToInt(this.listing_Standard.Slider(x, 500, 5000));
            this.listing_Standard.GapLine(12f);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.DisplayWildAnimalDeath, "DisplayAnimalKill", false, true);
            Scribe_Values.Look<bool>(ref this.DisplayAllyDeath, "DisplayAllyKill", true, true);
            Scribe_Values.Look<bool>(ref this.DisplayEnemyDeath, "DisplayEnemyKill", true, true);
            Scribe_Values.Look<bool>(ref this.UseLeftRightPos, "UseLeftRightPos", true, true);
            Scribe_Values.Look<bool>(ref this.DisplayPositionRight, "DisplayPositionUpRight", true, true);
            Scribe_Values.Look<int>(ref this.LeftOffset, "LeftOffset", 0, true);
            Scribe_Values.Look<int>(ref this.TopOffset, "TopOffset", 0, true);
            Scribe_Values.Look<int>(ref this.Width, "Width", 100, true);
            Scribe_Values.Look<int>(ref this.Height, "Height", 32, true);
            Scribe_Values.Look<int>(ref this.ExpirationTime, "TicksBetweenRemovals", 800, true);
            base.ExposeData();
        }

        private Listing_Standard listing_Standard;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(nameof(this.DisplayAllyDeath) + this.DisplayAllyDeath);
            sb.AppendLine(nameof(this.DisplayEnemyDeath) + this.DisplayEnemyDeath);
            sb.AppendLine(nameof(this.DisplayWildAnimalDeath) + this.DisplayWildAnimalDeath);
            sb.AppendLine(nameof(this.UseLeftRightPos) + this.UseLeftRightPos);
            sb.AppendLine(nameof(this.DisplayPositionRight) + this.DisplayPositionRight);
            sb.AppendLine(nameof(this.LeftOffset) + this.LeftOffset);
            sb.AppendLine(nameof(this.TopOffset) + this.TopOffset);
            sb.AppendLine(nameof(this.Width) + this.Width);
            sb.AppendLine(nameof(this.Height) + this.Height);
            sb.AppendLine(nameof(this.ExpirationTime) + this.ExpirationTime);
            return sb.ToString();
        }
    }
}
