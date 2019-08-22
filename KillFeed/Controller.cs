using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace KillFeed
{
    class Controller : Mod
    {
        public Controller(ModContentPack content) : base(content)
        {
            Controller.Settings = base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "Killfeed";
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Controller.Settings.DoWindowContents(canvas);
        }

        public static Settings Settings;
    }

    class Settings : ModSettings
    {
        public bool DisplayWildAnimalKill;
        public bool DisplayAllyKill;
        public bool DisplayEnemyKill;

        public bool UseLeftRightPos;
        public bool DisplayPositionRight;
        public int LeftOffset;
        public int TopOffset;
        private string LeftOffsetBuffer;
        private string RightOffsetBuffer;

        public int TicksBetweenRemovals;
        private string TicksBetweenRemovalsBuffer;

        internal void DoWindowContents(Rect canvas)
        {
            this.listing_Standard = new Listing_Standard();
            this.listing_Standard.Begin(GenUI.ContractedBy(canvas, 60f));
            this.listing_Standard.CheckboxLabeled("Display wild animal's kill?", ref DisplayWildAnimalKill, "Display animal kill's in kill feed");
            this.listing_Standard.CheckboxLabeled("Display ally's kill?", ref DisplayAllyKill, "Display ally's kill in kill feed");
            this.listing_Standard.CheckboxLabeled("Display enemy's kill?", ref DisplayEnemyKill, "Display enemy's kill in kill feed");
            this.listing_Standard.GapLine(12f);
            this.listing_Standard.CheckboxLabeled("Use preset Left/Right position?", ref UseLeftRightPos, "Whether or not display using below left/right preset");
            this.listing_Standard.CheckboxLabeled("Display killfeed on up right?", ref DisplayPositionRight, "Whether or not display kill feed on up right. Will reset the offset setting below to default value.");
            this.listing_Standard.TextFieldNumericLabeled<int>("Left offset", ref LeftOffset, ref LeftOffsetBuffer);
            this.listing_Standard.TextFieldNumericLabeled<int>("Top offset", ref TopOffset, ref RightOffsetBuffer);
            this.listing_Standard.GapLine(12f);
            this.listing_Standard.TextFieldNumericLabeled<int>("Killfeed's message appear duration (milisecond)", ref TicksBetweenRemovals, ref TicksBetweenRemovalsBuffer);
            //this.DoSlider(ref TicksBetweenRemovals);
            this.listing_Standard.End();
        }

        private void DoSlider(ref int x)
        {
            x = Mathf.RoundToInt(this.listing_Standard.Slider(x, 500, 5000));
            this.listing_Standard.GapLine(12f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.DisplayWildAnimalKill, "DisplayAnimalKill", false, true);
            Scribe_Values.Look<bool>(ref this.DisplayAllyKill, "DisplayAllyKill", true, true);
            Scribe_Values.Look<bool>(ref this.DisplayEnemyKill, "DisplayEnemyKill", true, true);
            Scribe_Values.Look<bool>(ref this.UseLeftRightPos, "UseLeftRightPos", true, true);
            Scribe_Values.Look<bool>(ref this.DisplayPositionRight, "DisplayPositionUpRight", true, true);
            Scribe_Values.Look<int>(ref this.LeftOffset, "LeftOffset", 0, true);
            Scribe_Values.Look<int>(ref this.TopOffset, "TopOffset", 0, true);
            Scribe_Values.Look<int>(ref this.TicksBetweenRemovals, "TicksBetweenRemovals", 800, true);
        }

        private Listing_Standard listing_Standard;
    }
}
