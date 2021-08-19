using HUD;
using RWCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ManyMoreFixes
{
    internal static class MiscChangesHK
    {
        public static void ApplySmallChanges()
        {
            string userDataPath = Custom.RootFolderDirectory() + "UserData";
            string sandBoxPath = userDataPath + Path.DirectorySeparatorChar + "Sandbox";
            // Creature UserData folder
            if (!File.Exists(userDataPath))
            {
                Directory.CreateDirectory(userDataPath);
                Debug.Log("Avoided crash because UserData folder was missing");
            }

            if (!File.Exists(sandBoxPath))
            {
                Directory.CreateDirectory(sandBoxPath);
                Debug.Log("Avoided crash because Sandbox folder was missing");
            }


            // ABOVECLOUDS
            //On.AboveCloudsView.ctor += AboveCloudsView_ctor;

            // FLIESWORD
            //On.FliesWorldAI.Update += FliesWorldAI_Update;

            // HITTHISOBJECT
            // Scavengers won't hit you or other creatures if they didn't mean it.
            On.Weapon.HitThisObject += Weapon_HitThisObject;

            // ABSTRACTROOM
            // Graphics quality option in Config Machine Options menu. 
            On.AbstractRoom.RealizeRoom += AbstractRoom_RealizeRoom;
            On.AbstractRoom.Update += AbstractRoom_Update;

            // SCAVENGER
            // Anti-Snatch-Spears.
            On.Scavenger.PickUpAndPlaceInInventory += Scavenger_PickUpAndPlaceInInventory;
            On.ScavengerAI.CollectScore_1 += ScavengerAI_CollectScore_1;

            // MAINLOOPPROCESS
            // Improper calculation of timestacker if game is lagging.
            //On.MainLoopProcess.RawUpdate += MainLoopProcess_RawUpdate;

            // ROOMCAMERA
            // Major performance improvements in Pebbles' room. 
            On.RoomCamera.ChangeBothPalettes += RoomCamera_ChangeBothPalettes;

            // SHELTERDOOR
            // All hostile creatures in shelter will stay sleeping at the start of the cycle 
            // until a little while after the shelter doors are fully open.
            //On.ShelterDoor.Update += ShelterDoor_Update;

            // SHORTCUTGRAPHICS
            // Shortcut colors for creatures that were missing them.
            On.ShortcutGraphics.ShortCutColor += ShortcutGraphics_ShortCutColor;

            // SNAILAI
            // Snails won't camp inside pipe entrances anymore.
            On.SnailAI.Update += SnailAI_Update;
            On.SnailAI.TileIdleScore += SnailAI_TileIdleScore;

            // HUD - DIALOGBOX
            // Text is properly aligned in dialog boxes.
            //On.HUD.DialogBox.NewMessage_1 += DialogBox_NewMessage_1;
            //On.HUD.DialogBox.Update += DialogBox_Update;
            //On.HUD.DialogBox.Draw += DialogBox_Draw;
            //On.HUD.DialogBox.InitNextMessage += DialogBox_InitNextMessage;

            // Load unkown placed object crashes the game
            On.PlacedObject.FromString += PlacedObject_FromString;
        }


        private static void PlacedObject_FromString(On.PlacedObject.orig_FromString orig, PlacedObject self, string[] s)
        {
            try
            {
                orig(self, s);
            }
            catch (Exception e)
            {
                Debug.LogError("[MMF] Error loading placed objects " + e);
            }
        }

        /*
        private static void FliesWorldAI_Update(On.FliesWorldAI.orig_Update orig, FliesWorldAI self)
        {
            MiscChangesHK.fliesToRespawnLast = null;
            object value;
            if ((value = typeof(FliesWorldAI).GetField("fliesToRespawn", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(self)) is int)
            {
                int value2 = (int)value;
                MiscChangesHK.fliesToRespawnLast = new int?(value2);
                typeof(FliesWorldAI).GetField("fliesToRespawn", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(self, 0);
            }
            else
            {
                Debug.Log("MMF: Reflection failed on FliesWorldAI.flies (int)");
            }
            self.fliesToRespawn
            orig.Invoke(self);
            if (MiscChangesHK.fliesToRespawnLast != null)
            {
                int? num = MiscChangesHK.fliesToRespawnLast;
                int num2 = 0;
                if ((num.GetValueOrDefault() > num2 & num != null) && self.world.rainCycle.TimeUntilRain > 800)
                {
                    typeof(FliesWorldAI).GetMethod("RespawnFlyInWorld", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(self, new object[0]);
                    MiscChangesHK.fliesToRespawnLast--;
                    typeof(FliesWorldAI).GetField("fliesToRespawn", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(self, MiscChangesHK.fliesToRespawnLast);
                }
            }
        }
        */

        private static bool Weapon_HitThisObject(On.Weapon.orig_HitThisObject orig, Weapon self, PhysicalObject obj)
        {
            Scavenger scavenger;
            if ((obj is Player || obj is Scavenger) && (scavenger = (self.thrownBy as Scavenger)) != null)
            {
                Tracker.CreatureRepresentation mostAttractivePrey = scavenger.AI.preyTracker.MostAttractivePrey;
                if (mostAttractivePrey != null && mostAttractivePrey.representedCreature.realizedCreature != null)
                {
                    if (obj as Creature != mostAttractivePrey.representedCreature.realizedCreature)
                    {
                        return false;
                    }
                    string str = (obj is Player) ? "the player." : "another scavenger.";
                    Debug.Log("Scavenger really wanted to hit " + str);
                }
            }
            return orig(self, obj);
        }

        private static void AbstractRoom_RealizeRoom(On.AbstractRoom.orig_RealizeRoom orig, AbstractRoom self, World world, RainWorldGame game)
        {
            if (MMFMod.config.quality == MMFMod.Quality.LOW)
            {
                self.singleRealizedRoom = true;
            }
            orig(self, world, game);
        }

        private static void AbstractRoom_Update(On.AbstractRoom.orig_Update orig, AbstractRoom self, int timePassed)
        {
            orig(self, timePassed);
            if (self != null && self.realizedRoom != null && MMFMod.config.quality == MMFMod.Quality.LOW)
            {
                self.singleRealizedRoom = true;
            }
        }

        // Anti-snatch spear
        private static void Scavenger_PickUpAndPlaceInInventory(On.Scavenger.orig_PickUpAndPlaceInInventory orig, Scavenger self, PhysicalObject obj)
        {
            if (obj is Spear spear && spear != null && (int)spear.mode == 6)
            {
                return;
            }
            orig(self, obj);
        }

        // Anti-snatch spear
        private static int ScavengerAI_CollectScore_1(On.ScavengerAI.orig_CollectScore_1 orig, ScavengerAI self, PhysicalObject obj, bool weaponFiltered)
        {
            if (obj is Spear && (int)(obj as Spear).mode == 6)
            {
                return 0;
            }
            return orig(self, obj, weaponFiltered);
        }

        /*
        private static void MainLoopProcess_RawUpdate(On.MainLoopProcess.orig_RawUpdate orig, MainLoopProcess self, float dt)
        {

                float num = (float)self.myTimeStacker;
                num += dt * (float)self.framesPerSecond;
                if (num > 1f)
                {
                    self.Update();
                    num -= 1f;
                    if (num >= 1f)
                    {
                        num = 0f;
                    }
                }
                self.GrafUpdate(num);
                //typeof(MainLoopProcess).GetField("myTimeStacker", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(self, num);
                self.myTimeStacker = num;
                return;

        }
        */

        private static void RoomCamera_ChangeBothPalettes(On.RoomCamera.orig_ChangeBothPalettes orig, RoomCamera self, int palA, int palB, float blend)
        {
            if (self.paletteA == palA && self.paletteB == palB && self.paletteBlend == blend)
            {
                return;
            }
            orig(self, palA, palB, blend);
        }
        /*
        private static void ShelterDoor_Update(On.ShelterDoor.orig_Update orig, ShelterDoor self, bool eu)
        {
            //object value;

            float num = self.closedFac;
            float num2;
            if (self.Broken)
            {
                num2 = 0f;
            }
            else
            {
                num2 = Mathf.Clamp(num, 0f, 1f);
            }
            if (num2 == 0f)
            {
                MiscChangesHK.openTime++;
            }
            else
            {
                MiscChangesHK.openTime = 0;
            }
            if (MiscChangesHK.openTime < 120)
            {
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    bool flag = false;
                    if (self.room.abstractRoom.creatures[i].state.socialMemory != null)
                    {
                        for (int j = 0; j < self.room.game.Players.Count; j++)
                        {
                            SocialMemory.Relationship orInitiateRelationship = self.room.abstractRoom.creatures[i].state.socialMemory.GetOrInitiateRelationship(self.room.game.Players[j].ID);
                            if (orInitiateRelationship.like >= 0.5f || orInitiateRelationship.tempLike > 0.5f)
                            {
                                flag = true;
                            }
                        }
                    }
                    if (!flag && self.room.abstractRoom.creatures[i].realizedCreature != null && (int)self.room.abstractRoom.creatures[i].creatureTemplate.type != 1)
                    {
                        self.room.abstractRoom.creatures[i].realizedCreature.stun = Math.Max(self.room.abstractRoom.creatures[i].realizedCreature.stun, 20);
                    }
                }
            }
            orig(self, eu);
        }
        */
        /*
        private static int openTime = 0;
        private static void ShelterDoor_Update(On.ShelterDoor.orig_Update orig, ShelterDoor self, bool eu)
        {
            orig(self, eu);

            if (self.Closed == 0f)
                openTime += 1;
            else
                openTime = 0;
            if (openTime < 120)
            {
                for (int i = 0; i < self.room.abstractRoom.creatures.Count; i++)
                {
                    if (ShelterDoor.CoordInsideShelterRange(self.room.abstractRoom.creatures[i].pos.Tile, this.isAncient))
                    {
                        bool likesPlayer = false;
                        if (this.room.abstractRoom.creatures[i].state.socialMemory != null)
                        {
                            for (int j = 0; j < this.room.game.Players.Count; j++)
                            {
                                if (this.room.game.GetStorySession.saveStateNumber == 5 && (this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.Scavenger || this.room.abstractRoom.creatures[i].creatureTemplate.type == CreatureTemplate.Type.ScavengerElite))
                                {
                                    likesPlayer = false;
                                }
                                else
                                {
                                    SocialMemory.Relationship orInitiateRelationship = room.abstractRoom.creatures[i].state.socialMemory.GetOrInitiateRelationship(room.game.Players[j].ID);
                                    if (orInitiateRelationship.like >= 0.5f || orInitiateRelationship.tempLike > 0.5f)
                                    {
                                        likesPlayer = true;
                                    }
                                }
                            }
                        }
                        if (!likesPlayer && this.room.abstractRoom.creatures[i].realizedCreature != null && this.room.abstractRoom.creatures[i].creatureTemplate.type != CreatureTemplate.Type.Slugcat)
                        {
                            this.room.abstractRoom.creatures[i].realizedCreature.stun = Math.Max(this.room.abstractRoom.creatures[i].realizedCreature.stun, 20);
                        }
                    }
                }
            }
        }
        */
        private static Color ShortcutGraphics_ShortCutColor(On.ShortcutGraphics.orig_ShortCutColor orig, ShortcutGraphics self, Creature crit,
            RWCustom.IntVector2 pos)
        {
            if ((int)crit.Template.type == 26)
            {
                crit.Template.shortcutSegments = 3;
                return Color.blue;
            }
            if ((int)crit.Template.type == 27)
            {
                crit.Template.shortcutSegments = 2;
                return new Color(0.7f, 0.7f, 0.4f);
            }
            return orig(self, crit, pos);
        }

        static int shuffleDestinationDelay = 0;
        private static void SnailAI_Update(On.SnailAI.orig_Update orig, SnailAI self)
        {
            orig(self);
            MiscChangesHK.shuffleDestinationDelay--;
            if (self.snail.room.GetTile(new IntVector2(self.creature.pos.x, self.creature.pos.y)).shortCut != 0 && MiscChangesHK.shuffleDestinationDelay <= 0)
            {
                MiscChangesHK.shuffleDestinationDelay = 200;
                self.creature.abstractAI.SetDestination(new WorldCoordinate(self.snail.room.abstractRoom.index,
                    UnityEngine.Random.Range(0, self.snail.room.TileWidth), UnityEngine.Random.Range(0, self.snail.room.TileHeight), -1));
            }
            if (MiscChangesHK.shuffleDestinationDelay > 0)
            {
                self.move = true;
            }
        }

        private static float SnailAI_TileIdleScore(On.SnailAI.orig_TileIdleScore orig, SnailAI self, WorldCoordinate pos)
        {
            if (!pos.TileDefined || !self.pathFinder.CoordinateReachable(pos) || self.snail.room.GetTile(pos).shortCut != 0)
            {
                return float.MinValue;
            }
            return orig(self, pos);
        }

        /*
        private static void DialogBox_Draw(On.HUD.DialogBox.orig_Draw orig, HUD.DialogBox self, float timeStacker)
        {
            orig(self, timeStacker);

        }

        private static void DialogBox_InitNextMessage(On.HUD.DialogBox.orig_InitNextMessage orig, DialogBox self)
        {
            orig(self);
            self.label.text = self.CurrentMessage.text;
            self.label.text = "";
        }



        public static bool IsOccupyingShortcut(SnailAI snailAI)
        {
            return snailAI.snail.room.GetTile(new IntVector2(snailAI.creature.pos.x, snailAI.creature.pos.y)).shortCut != 0;
        }
        */
    }
}
