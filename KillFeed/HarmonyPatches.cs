using System;
using System.Reflection;

using RimWorld;
using Verse;

using HarmonyLib;

namespace KillFeed
{
    /// <summary>
    /// Pretty self explanatory, contains all Harmony patches which need to be done.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {

            //Log.Message("0");
            Harmony harmony = null;
            try
            {
                //HarmonyInstance harmony = HarmonyInstance.Create("kahdeg.KillFeed");
                harmony = new Harmony("kahdeg.KillFeed");
            }
            catch (Exception ex)
            {
                Log.Message(ex.Message);
            }

            //Log.Message("1");

            if (harmony == null)
            {
                Log.Message("failed to patch");
                return;
            }

            {

                //Log.Message("2");
                Type targetType = typeof(Pawn);
                MethodInfo targetMethod = targetType.GetMethod("Kill");

                //Log.Message("3");

                harmony.Patch(
                    targetMethod,
                    null,
                    new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_Pawn_Kill)));

                //harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }

        public static void Patch_Pawn_Kill(ref Thing __instance, ref DamageInfo? dinfo, ref Hediff exactCulprit)
        {
            //Log.Message(ModData.Settings.ToString());
            //Log.Message("Pawn '" + __instance.LabelCap + "' got killed.");
            //Construct our killfeed announcement.
            if (__instance == null)
            {
                return;
            }

            Pawn pawn = __instance as Pawn;
            if (pawn != null)
            {

                //Log.Message(pawn.def.defName);
                //Log.Message(pawn.Faction.IsPlayer + "");

                KillAnnouncement announcement = new KillAnnouncement
                {
                    victim = pawn,
                    dinfo = dinfo,
                    exactCulprit = exactCulprit
                };

                //If the damage info got a value then something killed them normally.
                if (dinfo != null && dinfo.HasValue && dinfo.Value.Instigator != null)
                {
                    //Log.Message("natural death");
                    Thing instigator = dinfo.Value.Instigator;
                    announcement.perpetrator = instigator;

                    //Figure out the context of death.
                    bool victimIsFriendly = false;
                    bool victimIsNeutral = false;
                    if (announcement.victim.Faction != null)
                    {
                        if (announcement.victim.Faction.IsPlayer)
                        {
                            victimIsFriendly = true;
                        }
                        else
                        {
                            FactionRelation relationWithPlayer = announcement.victim.Faction.RelationWith(Faction.OfPlayer);
                            if (relationWithPlayer.kind == FactionRelationKind.Hostile && relationWithPlayer.baseGoodwill >= 40f)
                            {
                                victimIsFriendly = true;
                            }
                        }
                    }
                    else
                    {
                        victimIsNeutral = true;
                    }

                    //bool perpetratorIsFriendly = false;
                    //bool perpetratorIsNeutral = false;
                    //if (announcement.perpetrator.Faction != null)
                    //{
                    //    if (announcement.perpetrator.Faction.HasName || announcement.perpetrator.Faction.Name == "New Arrivals")
                    //    {
                    //        if (announcement.perpetrator.Faction.IsPlayer)
                    //        {
                    //            //Log.Message("Perpetrator faction is player's");
                    //            perpetratorIsFriendly = true;
                    //        }
                    //        else
                    //        {
                    //            FactionRelation relationWithPlayer = announcement.perpetrator.Faction.RelationWith(Faction.OfPlayer);
                    //            //Log.Message(announcement.perpetrator.Faction.Name);
                    //            if (relationWithPlayer.kind == FactionRelationKind.Hostile && relationWithPlayer.goodwill >= 40f)
                    //            {
                    //                perpetratorIsFriendly = true;
                    //                //Log.Message("Perpetrator faction is hostile");
                    //            }
                    //            else
                    //            {
                    //                //Log.Message("Perpetrator faction is ally");
                    //            }

                    //        }
                    //    }
                    //    else
                    //    {
                    //        //Log.Message("perpetrator faction has no name" + announcement.perpetrator.Faction.Name);
                    //    }
                    //}
                    //else
                    //{
                    //    //Log.Message("perpetrator has no faction");
                    //    perpetratorIsNeutral = true;
                    //}

                    //categorize announcement type
                    announcement.type = KillAnnouncementType.Ignore;

                    if (victimIsFriendly)
                    {
                        if (ModData.Settings.DisplayAllyDeath)
                        {
                            announcement.type = KillAnnouncementType.Ally;
                        }
                    }
                    else
                    {
                        if (victimIsNeutral)
                        {
                            if (ModData.Settings.DisplayWildAnimalDeath)
                            {
                                announcement.type = KillAnnouncementType.WildAnimal;
                            }
                        }
                        else
                        {
                            if (ModData.Settings.DisplayEnemyDeath)
                            {
                                announcement.type = KillAnnouncementType.Enemy;
                            }
                        }
                    }

                    //Log.Message(announcement.type.ToString());

                    //Log.Message(announcement.perpetrator.Position.ToStringSafe());

                    /*if (announcement.perpetrator.HostileTo(Faction.OfPlayer))
                    {
                        announcement.type = KillAnnouncementType.Enemy; 
                    }
                    else
                    {
                        announcement.type = KillAnnouncementType.Ally;
                    }*/
                }
                else
                {
                    //Log.Message(announcement.victim.Position.ToStringSafe());
                    announcement.type = KillAnnouncementType.Magic;
                    if (exactCulprit != null)
                    {
                        announcement.flavorText = " died from " + exactCulprit.Label;
                    }
                    else
                    {
                        announcement.flavorText = " died from magic";
                    }

                }


                if (announcement.type != KillAnnouncementType.Ignore)
                {
                    Current.Game.GetComponent<KillFeedGameComponent>().PushAnnouncement(announcement);
                }
            }
        }
    }
}
