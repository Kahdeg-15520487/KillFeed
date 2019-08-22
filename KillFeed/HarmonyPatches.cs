using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

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
            HarmonyInstance harmony = HarmonyInstance.Create("kahdeg.KillFeed");

            {
                Type targetType = typeof(Pawn);
                MethodInfo targetMethod = targetType.GetMethod("Kill");

                harmony.Patch(
                    targetMethod,
                    null,
                    new HarmonyMethod(typeof(HarmonyPatches), nameof(Patch_Pawn_Kill)));

                //harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
        }

        public static void Patch_Pawn_Kill(ref Thing __instance, ref DamageInfo? dinfo, ref Hediff exactCulprit)
        {
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

                KillAnnouncement announcement = new KillAnnouncement
                {
                    victim = pawn,
                    dinfo = dinfo,
                    exactCulprit = exactCulprit
                };

                //If the damage info got a value then something killed them normally.
                if (dinfo != null && dinfo.HasValue && dinfo.Value.Instigator != null)
                {
                    Thing instigator = dinfo.Value.Instigator;
                    announcement.perpetrator = instigator;

                    //Figure out the context of death.
                    bool victimIsFriendly = false;
                    if (announcement.victim.Faction != null)
                    {
                        if (announcement.victim.Faction.IsPlayer)
                        {
                            victimIsFriendly = true;
                        }
                        else
                        {
                            FactionRelation relationWithPlayer = announcement.victim.Faction.RelationWith(Faction.OfPlayer);
                            if (relationWithPlayer.kind == FactionRelationKind.Hostile && relationWithPlayer.goodwill >= 40f)
                            {
                                victimIsFriendly = true;
                            }
                        }
                    }

                    bool perpetratorIsFriendly = false;
                    bool perpetratorIsWildAnimal = false;
                    if (announcement.perpetrator.Faction != null)
                    {
                        if (announcement.perpetrator.Faction.HasName || announcement.perpetrator.Faction.Name == "New Arrivals")
                        {
                            if (announcement.perpetrator.Faction.IsPlayer)
                            {
                                //Log.Message("Perpetrator faction is player's");
                                perpetratorIsFriendly = true;
                            }
                            else
                            {
                                FactionRelation relationWithPlayer = announcement.perpetrator.Faction.RelationWith(Faction.OfPlayer);
                                //Log.Message(announcement.perpetrator.Faction.Name);
                                if (relationWithPlayer.kind == FactionRelationKind.Hostile && relationWithPlayer.goodwill >= 40f)
                                {
                                    perpetratorIsFriendly = true;
                                    //Log.Message("Perpetrator faction is hostile");
                                }
                                else
                                {
                                    //Log.Message("Perpetrator faction is ally");
                                }

                            }
                        }
                        else
                        {
                            //Log.Message("perpetrator faction has no name" + announcement.perpetrator.Faction.Name);
                        }
                    }
                    else
                    {
                        //Log.Message("perpetrator has no faction");
                        perpetratorIsWildAnimal = true;
                    }

                    if (victimIsFriendly)
                    {
                        if (!perpetratorIsFriendly)
                        {
                            announcement.type = KillAnnouncementType.Enemy;
                        }
                    }
                    else
                    {
                        if (perpetratorIsFriendly)
                        {
                            announcement.type = KillAnnouncementType.Ally;
                        }
                    }

                    if (perpetratorIsWildAnimal)
                    {
                        announcement.type = KillAnnouncementType.WildAnimal;
                    }

                    if (!Controller.Settings.DisplayAllyKill)
                    {
                        announcement.type = KillAnnouncementType.Ignore;
                    }

                    if (!Controller.Settings.DisplayEnemyKill)
                    {
                        announcement.type = KillAnnouncementType.Ignore;
                    }

                    if (!Controller.Settings.DisplayWildAnimalKill)
                    {
                        announcement.type = KillAnnouncementType.Ignore;
                    }

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
