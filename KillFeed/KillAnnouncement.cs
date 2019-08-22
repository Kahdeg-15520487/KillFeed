using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace KillFeed
{
    public enum KillAnnouncementType
    {
        WildAnimal,
        Ally,
        Enemy,
        Ignore
    }

    /// <summary>
    /// Announces a kill in the killfeed.
    /// </summary>
    public class KillAnnouncement
    {
        public Thing victim;
        public Thing perpetrator;
        public string flavorText;
        public KillAnnouncementType type;
        public DamageInfo? dinfo;
        public Hediff exactCulprit;
        public Rect currentRowRect;

        public static float colorAlpha = 0.6f;
        public static Color colorAlly = new Color(0.5f, 0.84f, 0.91f, colorAlpha);
        public static Color colorEnemy = new Color(0.91f, 0.5f, 0.5f, colorAlpha);
        public static Color colorNeutral = new Color(0.5f, 0.5f, 0.5f, colorAlpha);

        public KillAnnouncement()
        {
            type = KillAnnouncementType.WildAnimal;
        }

        public void OnGUI(Rect inRect)
        {
            Rect ininRect = inRect.ContractedBy(2f);
            currentRowRect = inRect;

            float height = ininRect.height;

            switch (type)
            {
                case KillAnnouncementType.WildAnimal:
                    {
                        Widgets.DrawBoxSolid(ininRect, colorNeutral);
                    }
                    break;

                case KillAnnouncementType.Ally:
                    {
                        Widgets.DrawBoxSolid(ininRect, colorAlly);
                    }
                    break;

                case KillAnnouncementType.Enemy:
                    {
                        Widgets.DrawBoxSolid(ininRect, colorEnemy);
                    }
                    break;
                case KillAnnouncementType.Ignore:
                    return;
            }

            Text.Anchor = TextAnchor.MiddleLeft;

            //Perpetrator if applicable.
            float currentXPosition = ininRect.x;

            if (perpetrator != null)
            {
                Rect portraitRect = new Rect(ininRect);
                portraitRect.x = currentXPosition;
                portraitRect.width = portraitRect.height;

                Widgets.ThingIcon(portraitRect, perpetrator);

                currentXPosition += portraitRect.width;
                currentXPosition += 2f;

                Vector2 nameSize = Text.CalcSize(perpetrator.LabelShort);
                Rect nameRect = new Rect(portraitRect);
                nameRect.x = currentXPosition;
                nameRect.width = nameSize.x;
                Widgets.Label(nameRect, perpetrator.LabelShort);


                currentXPosition += nameRect.width;
            }

            //Weapon if applicable
            if (dinfo.HasValue && dinfo.Value.Weapon != null)
            {
                currentXPosition += 2f;

                Rect portraitRect = new Rect(ininRect);
                portraitRect.x = currentXPosition;
                portraitRect.width = portraitRect.height;

                if (dinfo.Value.Weapon.uiIcon != null)
                {
                    try
                    {
                        Widgets.ThingIcon(portraitRect, dinfo.Value.Weapon);
                    }
                    catch (Exception)
                    {
                        dinfo.Value.Weapon.label = "melee";
                    }
                }

                currentXPosition += portraitRect.width;
                currentXPosition += 2f;

                Vector2 nameSize = Text.CalcSize(dinfo.Value.Weapon.label);
                Rect nameRect = new Rect(portraitRect);
                nameRect.x = currentXPosition;
                nameRect.width = nameSize.x;
                Widgets.Label(nameRect, dinfo.Value.Weapon.label);

                currentXPosition += nameRect.width;
            }

            //Victim
            {
                currentXPosition += 2f;

                if (victim != null)
                {

                    Rect portraitRect = new Rect(ininRect);
                    portraitRect.x = currentXPosition;
                    portraitRect.width = portraitRect.height;

                    Widgets.ThingIcon(portraitRect, victim);

                    currentXPosition += portraitRect.width;
                    currentXPosition += 2f;



                    Vector2 nameSize = Text.CalcSize(victim.LabelShort);
                    Rect nameRect = new Rect(portraitRect);
                    nameRect.x = currentXPosition;
                    nameRect.width = nameSize.x;
                    Widgets.Label(nameRect, victim.LabelShort);

                    currentXPosition += nameRect.width;
                }
                else
                {
                    Log.Message("victim null");
                }
            }

            //Flavor text.
            if (perpetrator == null && !flavorText.NullOrEmpty())
            {
                Vector2 flavorTextSize = Text.CalcSize(flavorText);
                Rect flavorTextRect = new Rect(ininRect);
                flavorTextRect.x = currentXPosition;
                flavorTextRect.width = flavorTextSize.x;
                Widgets.Label(flavorTextRect, flavorText);

                currentXPosition += flavorTextSize.x;
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
