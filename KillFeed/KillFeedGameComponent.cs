using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace KillFeed
{
    /// <summary>
    /// Game component responsible for rendering the killfeed.
    /// </summary>
    public class KillFeedGameComponent : GameComponent
    {
        /// <summary>
        /// Constant for ticksLeftUntilRemove.
        /// </summary>
        //todo make this a setting
        public static int defaultTicksBetweenRemovals = 10;

        public static float defaultRowHeight = 32f;

        /// <summary>
        /// Current ongoing game.
        /// </summary>
        public Game game;

        /// <summary>
        /// The feed itself.
        /// </summary>
        public List<KillAnnouncement> feed = new List<KillAnnouncement>();
        /// <summary>
        /// How many ticks left until the bottom most announcement will be removed.
        /// </summary>
        public int ticksLeftUntilRemove = 0;

        public KillFeedGameComponent()
        {

        }

        public KillFeedGameComponent(Game game)
        {
            this.game = game;
        }

        private KillAnnouncement GetMouseOverKillAnnouncement()
        {
            foreach (KillAnnouncement announcement in feed)
            {
                if (Mouse.IsOver(announcement.currentRowRect))
                {
                    //Log.Message(announcement.victim.LabelShort);
                    return announcement;
                }
            }
            return null;
        }

        public override void GameComponentOnGUI()
        {
            Event e = Event.current;
            KillAnnouncement ka;
            if (e.isMouse && e.type == EventType.MouseUp)
            {
                switch (e.button)
                {
                    case 0:
                        //Log.Message("Left Click");
                        ka = GetMouseOverKillAnnouncement();
                        if (ka != null)
                        {
                            CameraJumper.TryJump(new RimWorld.Planet.GlobalTargetInfo(ka.victim));
                        }
                        break;
                    case 1:
                        //Log.Message("Right Click");
                        ka = GetMouseOverKillAnnouncement();
                        if (ka != null)
                        {
                            feed.Remove(GetMouseOverKillAnnouncement());
                        }
                        break;
                    case 2:
                        //Log.Message("Middle Click");
                        break;
                    default:
                        break;
                }
            }

            //Do logic.


            if (ticksLeftUntilRemove > 0)
            {
                ticksLeftUntilRemove--;

                if (ticksLeftUntilRemove <= 0)
                {
                    PopAnnouncement();
                }
            }

            if (ticksLeftUntilRemove <= 0 && feed.Count > 0)
            {
                ticksLeftUntilRemove = defaultTicksBetweenRemovals;
            }

            //Render the feed.
            Vector2 screen = new Vector2(UI.screenWidth, UI.screenHeight);
            float quarterWidth = screen.x / 4f;
            float quarterHeight = screen.y;

            int left = ModData.Settings.LeftOffset;
            int top = ModData.Settings.TopOffset;
            int rowHeight = ModData.Settings.Height == 0 ? (int)defaultRowHeight : ModData.Settings.Height;
            int rowWidth = ModData.Settings.Width == 0 ? (int)quarterWidth : ModData.Settings.Width;

            if (!ModData.Settings.UseLeftRightPos)
            {
                if (ModData.Settings.DisplayPositionRight)
                {
                    left = (int)(screen.x - rowWidth);
                    top = 0;
                }
                else
                {
                    left = 0;
                    top = 0;
                }
            }

            Rect rect = new Rect(left, top + rowHeight, quarterWidth, quarterHeight);
            float row = 0f;

            foreach (KillAnnouncement announcement in feed)
            {
                Rect rowRect = new Rect(rect);
                rowRect.y += row;
                rowRect.height = rowHeight;
                rowRect.width = rowWidth;

                announcement.OnGUI(rowRect);
                row += rowRect.height;
            }
        }

        /// <summary>
        /// Pushes announcement to the top of the feed.
        /// </summary>
        /// <param name="announcement">Announcement to push.</param>
        public void PushAnnouncement(KillAnnouncement announcement)
        {
            //Log.Message(announcement.type.ToString());
            feed.Insert(0, announcement);
            int rowHeight = ModData.Settings.Height;
            //Pop last announcement if it would exceed the vertical screen size.
            float bottomY = rowHeight + (float)feed.Count * rowHeight;

            if (bottomY >= (float)UI.screenHeight - rowHeight)
            {
                PopAnnouncement();
            }
        }

        /// <summary>
        /// Removes last announcement.
        /// </summary>
        public void PopAnnouncement()
        {
            if (feed.Count > 0)
            {
                feed.RemoveAll((m) => m.Expired);
            }
        }
    }
}
