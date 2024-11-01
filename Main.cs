using MelonLoader;
using HarmonyLib;
using Vertigo2;
using Vertigo2.Player;
using Vertigo2.Interaction;
using Vertigo2.Inventory;
using Vertigo2.Weapons;
using Valve.VR;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System;
using MyTrueGear;
using System.Threading;
using static RootMotion.FinalIK.InteractionTrigger;
using Valve.VR.InteractionSystem;
using static GhrontoBoss;
using Vertigo2BHaptics;
using static RootMotion.FinalIK.HitReaction;

namespace Vertigo2_TrueGear
{
    public static class BuildInfo
    {
        public const string Name = "Vertigo2_TrueGear"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "TrueGear Mod for Vertigo2"; // Description for the Mod.  (Set as null if none)
        public const string Author = "HuangLvYuan"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }
    


    public class Vertigo2_TrueGear : MelonMod
    {
        private static float num3 = 0;
        private static bool isHeartBeat = false;

        private static TrueGearMod _TrueGear = null;

        private static bool isSomkingDamage = false;
        private static bool isRadiationDamage = false;
        private static bool isFireDamage = false;

        private static bool canBladeShoot = true;
        private static bool canShock = true;
        private static bool canPickup = true;

        private static Timer bladeShootTimer = new Timer(BladeShootTimerCallBack,null, Timeout.Infinite, Timeout.Infinite);
        private static Timer pickupTimer = new Timer(PickupItemTimerCallBack,null, Timeout.Infinite, Timeout.Infinite);
        private static Timer handleShockTimer = new Timer(HandleShockTimerCallBack, null, Timeout.Infinite, Timeout.Infinite);

        private static bool isPause = false;
        private static bool isDeath = false;

        private static bool canShoot = true;


        public override void OnInitializeMelon() {
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Vertigo2_TrueGear));
            _TrueGear = new TrueGearMod();
            //MelonLogger.Msg("OnApplicationStart");
        }
        
        private static KeyValuePair<float, float> GetAngle1(VertigoPlayer player, HitInfo hit)
        {
            float num = 0f;
            float num2 = 0f;

            if (player != null && hit != null)
            {
                Vector3 to = new Vector3(0f, 0f, 1f);
                Vector3 vector = -hit.hitDir;
                if (hit.hitDir.y < 0.1f)
                {
                    vector = -vector;
                }
                Vector3 eulerAngles = player.head.rotation.eulerAngles;

                num = Vector3.SignedAngle(new Vector3(vector.x, 0f, vector.z), to, Vector3.up);
                num -= eulerAngles.y;
                num *= -1f;
                num = Mathf.Repeat(num, 360f);
                num2 = hit.hitPoint.y - (player.head.position.y - 0.4f);

                //MelonLogger.Msg("------------------------------------------");
                //MelonLogger.Msg(num);
                //MelonLogger.Msg($"x :{hit.hitDir.x},y :{hit.hitDir.y},z :{hit.hitDir.z}");
            }           

            return new KeyValuePair<float, float>(num, num2);
        }


        private static KeyValuePair<float, float> GetAngle2(VertigoPlayer player, HitInfo hit)
        {
            float num = 0f;
            float num2 = 0f;
            Vector3 to = new Vector3(0f, 0f, 1f);
            Vector3 vector = -hit.hitNormal;
            Vector3 eulerAngles = player.head.rotation.eulerAngles;
            num = Vector3.SignedAngle(new Vector3(vector.x, 0f, vector.z), to, Vector3.up);
            num -= eulerAngles.y;
            num *= -1f;
            num = Mathf.Repeat(num, 360f);
            num2 = hit.hitPoint.y - (player.head.position.y - 0.4f);

            //MelonLogger.Msg(num);
            //MelonLogger.Msg($"x :{hit.hitNormal.x},y :{hit.hitNormal.y},z :{hit.hitNormal.z}");

            return new KeyValuePair<float, float>(num, num2);
        }

        private static KeyValuePair<float, float> GetAngle3(VertigoPlayer player, HitInfo hit)
        {
            float num2 = 0f;

            Vector3 playerToEnemy = hit.source.transform.position - player.transform.position;
            float angle = Vector3.SignedAngle(player.transform.forward, playerToEnemy, Vector3.up);
            angle = (angle + 360) % 360; // 确保角度在0到360之间
            angle = 360f - angle;

            num2 = hit.hitPoint.y - (player.head.position.y - 0.4f);

            //MelonLogger.Msg(angle);

            return new KeyValuePair<float, float>(angle, num2);
        }



        [HarmonyPrefix,HarmonyPatch(typeof(CopterAirChase), "Crash")]
        private static void CopterAirChase_Crash_Prefix() 
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("CopterCrash");
            _TrueGear.Play("CopterCrash");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(CopterAirChase), "MissileHitCopter")]
        private static void CopterAirChase_MissileHitCopter_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("MissileHitCopter");
            _TrueGear.Play("MissileHitCopter");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Cyberjoseph), "BigStompNoDamage")]
        private static void Cyberjoseph_BigStompNoDamage_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("CyberjosephBigStompNoDamage");
            _TrueGear.Play("CyberjosephBigStompNoDamage");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Cyberjoseph), "BothFeetStomp")]
        private static void Cyberjoseph_BothFeetStomp_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("CyberjosephBothFeetStomp");
            _TrueGear.Play("CyberjosephBothFeetStomp");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Cyberjoseph), "FaceSlamNoDamage")]
        private static void Cyberjoseph_FaceSlamNoDamage_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("CyberjosephFaceSlamNoDamage");
            _TrueGear.Play("CyberjosephFaceSlamNoDamage");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Cyberjoseph), "SuperSlam")]
        private static void Cyberjoseph_SuperSlam_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("CyberjosephSuperSlam");
            _TrueGear.Play("CyberjosephSuperSlam");
        }

        


        [HarmonyPostfix, HarmonyPatch(typeof(HealthFruit), "Bite")]
        private static void HealthFruit_Bite_Postfix()
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("Healing");
            _TrueGear.Play("Healing");
        }

        

        [HarmonyPostfix, HarmonyPatch(typeof(QuadBow), "ReleaseString")]
        private static void QuadBow_ReleaseString_Postfix(VertigoHand hand)
        {
            if (hand.inputSource == SteamVR_Input_Sources.RightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPistolShoot");
                _TrueGear.Play("RightHandPistolShoot");
            }
            else if (hand.inputSource == SteamVR_Input_Sources.LeftHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPistolShoot");
                _TrueGear.Play("LeftHandPistolShoot");
            }            
        }
        
        [HarmonyPatch(typeof(Explosion), "BHapticsFX")]
        public static IEnumerable<CodeInstruction> Num3Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Ldloc_3)
                {
                   codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_3)); 
                   codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Vertigo2_TrueGear), "ProcessNum3Value")));
                   i += 2; 
               }
            }
            return codes;
        }
        private static void ProcessNum3Value(float value)
        {
            float num3 = value;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Explosion), "BHapticsFX")]
        private static void Explosion_BHapticsFX_Postfix(Explosion __instance)
        {
            if (num3 > 0f)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("Explosion");
                _TrueGear.Play("Explosion");
            }            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Ionizer), "EjectMagCoroutine")]
        private static void Ionizer_EjectMagCoroutine_Postfix(Ionizer __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }
        /*
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerFootstepFX), "Impact")]
        private static void PlayerFootstepFX_Impact_Postfix(PlayerFootstepFX __instance)
        {
            if (__instance.impactFXCounter.Ready())
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftAndRightFootStep");
                _TrueGear.Play("LeftAndRightFootStep");
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerFootstepFX), "Step")]
        private static void PlayerFootstepFX_Step_Postfix(PlayerFootstepFX __instance)
        {
            if (__instance.rightstep)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightFootStep");
                _TrueGear.Play("RightFootStep");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftFootStep");
                _TrueGear.Play("LeftFootStep");
            }
        }
        */
        [HarmonyPostfix, HarmonyPatch(typeof(AmmoBelt), "TakeAmmo")]
        private static void AmmoBelt_TakeAmmo_Postfix(AmmoBelt __instance, VertigoHand hand)
        {
            if (__instance.currentAmmo != -1)           // && __instance.ammoSlots[__instance.currentAmmo].fillAmount >= 1f
            {
                bool isRightHanded = hand.inputSource == SteamVR_Input_Sources.RightHand;
                if (isRightHanded)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightTakeAmmo");
                    _TrueGear.Play("RightTakeAmmo");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftTakeAmmo");
                    _TrueGear.Play("LeftTakeAmmo");
                }
            }                   
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "TryLoad")]
        private static void Gun_TryLoad_Postfix(Gun __instance, Ammo ammo)
        {
            if (__instance.reloadReady && ammo != null && ammo.ammoTag == __instance.ammoName)
            {
                bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
                if (isRight)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightReload");
                    _TrueGear.Play("RightReload");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftReload");
                    _TrueGear.Play("LeftReload");
                }
            }          
        }

        /*
        [HarmonyPrefix, HarmonyPatch(typeof(Gun), "Firing")]
        private static void Gun_Firing_Prefix(Gun __instance)
        {
            if (__instance.action_fire.GetStateDown(__instance.inputSource) && (!__instance.fireDelay || !__instance.modifier.consumeAmmo) && !__instance.waitingForGrabRelease && __instance.CheckForUnderwater())
            {
                if (__instance.readyToFire)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("Firing1");
                }
                else if (__instance.emptyclips.Length != 0) 
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("Firing2");
                }
                //MelonLogger.Msg("Firing");
                //MelonLogger.Msg(__instance.name);
                //MelonLogger.Msg(__instance.shooter.shot.name);
                //MelonLogger.Msg(__instance.shooter.sourceEntity.name);
                //MelonLogger.Msg(__instance.shooter.sourceEntity.friendlyFireID);
            }
        }
        */
        private static void ShootTimerCallBack(object o)
        {
            canShoot = true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(Gun), "ShootHaptics")]
        private static void Gun_ShootHaptics_Postfix(Gun __instance)
        {
            if (!canShoot)
            {
                return;
            }
            canShoot = false;
            Timer shootTimer = new Timer(ShootTimerCallBack,null,100,Timeout.Infinite);
            bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            bool otherHandHolding = __instance.heldEquippable.otherHandHolding;

            if (__instance.name.Contains("Mk") || __instance.name.Contains("Phage") || __instance.name.Contains("Nailer"))
            {
                if (otherHandHolding)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandShotgunShoot");
                    _TrueGear.Play("RightHandShotgunShoot");
                    //MelonLogger.Msg("LeftHandShotgunShoot");
                    _TrueGear.Play("LeftHandShotgunShoot");
                    return;
                }
                if (isRightHand)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandShotgunShoot");
                    _TrueGear.Play("RightHandShotgunShoot");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftHandShotgunShoot");
                    _TrueGear.Play("LeftHandShotgunShoot");
                }
            }
            else if (__instance.name.Contains("Trident") || __instance.name.Contains("AK"))
            {
                if (otherHandHolding)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                    //MelonLogger.Msg("LeftHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                    return;
                }
                if (isRightHand)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandRifleShoot");
                    _TrueGear.Play("RightHandRifleShoot");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftHandRifleShoot");
                    _TrueGear.Play("LeftHandRifleShoot");
                }
            }
            else
            {
                if (otherHandHolding)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandPistolShoot");
                    _TrueGear.Play("RightHandPistolShoot");
                    //MelonLogger.Msg("LeftHandPistolShoot");
                    _TrueGear.Play("LeftHandPistolShoot");
                    return;
                }
                if (isRightHand)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandPistolShoot");
                    _TrueGear.Play("RightHandPistolShoot");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftHandPistolShoot");
                    _TrueGear.Play("LeftHandPistolShoot");
                }
            }            
            ////MelonLogger.Msg("Gun");
            ////MelonLogger.Msg(__instance.name);
            ////MelonLogger.Msg(__instance.shooter.shot.name);
            ////MelonLogger.Msg(__instance.shooter.sourceEntity.name);
            ////MelonLogger.Msg(__instance.shooter.sourceEntity.friendlyFireID);
        }
        


        [HarmonyPostfix, HarmonyPatch(typeof(Enlighten), "Haptics")]            //激光枪
        private static void Enlighten_Haptics_Postfix(Enlighten __instance)
        {
            if (__instance.readyToFire && __instance.action_fire.GetState(__instance.inputSource))
            {
                if (!__instance.hapticsTimer)
                {
                    if (!canShoot)
                    {
                        return;
                    }
                    canShoot = false;
                    Timer shootTimer = new Timer(ShootTimerCallBack, null, 100, Timeout.Infinite);
                    bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
                    bool otherHandHolding = __instance.heldEquippable.otherHandHolding;
                    if (otherHandHolding)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("RightHandRifleShoot");
                        _TrueGear.Play("RightHandRifleShoot");
                        //MelonLogger.Msg("LeftHandRifleShoot");
                        _TrueGear.Play("LeftHandRifleShoot");
                        return;
                    }
                    if (isRightHand)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("RightHandRifleShoot");
                        _TrueGear.Play("RightHandRifleShoot");
                    }
                    else
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("LeftHandRifleShoot");
                        _TrueGear.Play("LeftHandRifleShoot");
                    }
                }                    
            }            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(HammerSickle), "Blade_OnImpact")]
        private static void HammerSickle_Blade_OnImpact_Postfix(HammerSickle __instance)
        {
            bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Blafaladaciousnesticles), "Deflect")]
        private static void Blafaladaciousnesticles_Deflect_Postfix(Blafaladaciousnesticles __instance)
        {
            bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Blafaladaciousnesticles), "OnImpact")]
        private static void Blafaladaciousnesticles_OnImpact_Postfix(Blafaladaciousnesticles __instance, float normalizedSpeed)
        {
            if (normalizedSpeed > 0f)
            {
                bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
                if (isRightHand)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightHandPickupItem");
                    _TrueGear.Play("RightHandPickupItem");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftHandPickupItem");
                    _TrueGear.Play("LeftHandPickupItem");
                }
            }            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(HammerSickle), "ThrowProjectile")]
        private static void HammerSickle_ThrowProjectile_Postfix(HammerSickle __instance)
        {
            bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(VertigoPlayer), "Hit")]
        private static void VertigoPlayer_Hit_Prefix(VertigoPlayer __instance, HitInfo hit)
        {
            if (__instance.dead)
            {
                return;
            }
            if (VertigoPlayer.godMode)
            {
                return;
            }
            if (__instance.LoadingInvulnerability)
            {
                return;
            }
            var angle1 = GetAngle1(__instance,hit);
            var angle2 = GetAngle2(__instance,hit);
            var angle3 = GetAngle3(__instance,hit);
            var angle = angle3;
            //MelonLogger.Msg(hit.success);
            //MelonLogger.Msg(hit.source.name);
            if (hit != null)
            {
                DamageType damageType = hit.damageType;
                if (damageType <= DamageType.Impact)
                {
                    if (damageType <= DamageType.Plasma)
                    {
                        if (damageType == DamageType.Generic)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"DefaultDamage1,{angle.Key},{angle.Value}");      //explosion
                            _TrueGear.PlayAngle("DefaultDamage", angle.Key,angle.Value);
                            return;
                        }
                        if (damageType == DamageType.Bullet)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"PlayerBulletDamage1,{angle3.Key},{angle3.Value}");
                            _TrueGear.PlayAngle("PlayerBulletDamage", angle3.Key,angle3.Value);
                            return;
                        }
                        if (damageType == DamageType.Plasma)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"PlayerBulletDamage2,{angle3.Key},{angle3.Value}");
                            _TrueGear.PlayAngle("PlayerBulletDamage", angle3.Key,angle3.Value);
                            return;
                        }
                    }
                    else if (damageType <= DamageType.Antimatter)
                    {
                        if (damageType == DamageType.Laser)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"PlayerBulletDamage3,{angle.Key},{angle.Value}");
                            _TrueGear.PlayAngle("PlayerBulletDamage", angle.Key,angle.Value);
                            return;

                        }
                        if (damageType == DamageType.Antimatter)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"DefaultDamage2,{angle.Key},{angle.Value}");        //explosion
                            _TrueGear.PlayAngle("DefaultDamage", angle.Key,angle.Value);
                            return;
                        }
                    }
                    else
                    {
                        if (damageType == DamageType.Explosion)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"DefaultDamage3,{angle.Key},{angle.Value}");        //explosion
                            _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
                            return;
                        }
                        if (damageType == DamageType.Impact)
                        {
                            //MelonLogger.Msg("------------------------------------------------");
                            //MelonLogger.Msg($"DefaultDamage4,{angle3.Key},{angle3.Value}");        //explosion
                            _TrueGear.PlayAngle("DefaultDamage", angle3.Key, angle3.Value);
                            return;
                        }
                    }
                }
                else if (damageType <= DamageType.Enlightenment)
                {
                    if (damageType == DamageType.Blade)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage5,{angle3.Key},{angle3.Value}");      //blade
                        _TrueGear.PlayAngle("DefaultDamage", angle3.Key,angle3.Value);
                        return;
                    }
                    if (damageType == DamageType.Bite)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage6,{angle3.Key},{angle3.Value}");          //blade
                        _TrueGear.PlayAngle("DefaultDamage", angle3.Key, angle3.Value);
                        return;
                    }
                    if (damageType == DamageType.Enlightenment)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage7,{angle.Key},{angle.Value}");            //explosion
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
                        return;
                    }
                }
                else if (damageType <= DamageType.Cold)
                {
                    if (damageType == DamageType.Heat)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage8,{angle.Key},{angle.Value}");           //LavaballDamage
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key,angle.Value);
                        return;
                    }
                    if (damageType == DamageType.Cold)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage9,{angle.Key},{angle.Value}");         //freeze
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key,angle.Value);
                        return;
                    }
                }
                else
                {
                    if (damageType == DamageType.Hyperdimensional)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage10,{angle.Key},{angle.Value}");            //explosion
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
                        return;
                    }
                    if (damageType == DamageType.Electricity)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg($"DefaultDamage11,{angle.Key},{angle.Value}");            //ElectricDamage
                        _TrueGear.PlayAngle("DefaultDamage", angle.Key,angle.Value);
                        return;
                    }
                }
            }
            if (hit != null)
            {
                if ((hit.damageType & DamageType.Fire) == DamageType.Fire)
                {
                    if (!isFireDamage)
                    {
                        isFireDamage = true;
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("FireDamage");
                        _TrueGear.Play("FireDamage");
                        Timer fireDamage = new Timer(FireDamageTimerCallBack, null, 100, Timeout.Infinite);
                    }
                    return;
                }
                if ((hit.damageType & DamageType.Poison) == DamageType.Poison)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    if (hit.source.name.Contains("Sapling_Blowgun"))
                    {
                        //MelonLogger.Msg($"PoisonDamage,{angle.Key},{angle.Value}");
                        _TrueGear.PlayAngle("PoisonDamage", angle.Key, angle.Value);
                        return;
                    }                    
                    //MelonLogger.Msg($"PoisonDamage,{angle3.Key},{angle3.Value}");
                    _TrueGear.PlayAngle("PoisonDamage", angle3.Key, angle3.Value);
                    return;
                }
                if ((hit.damageType & DamageType.Drowning) == DamageType.Drowning)
                {
                    if (!isSomkingDamage)
                    {
                        isSomkingDamage = true;
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("PoisonDamage");
                        _TrueGear.Play("PoisonDamage");              /////////////////////////////可能会一直触发      smoking
                        Timer smokingDamage = new Timer(SmokingDamageTimerCallBack, null, 100, Timeout.Infinite);
                    }
                    return;
                }
                if ((hit.damageType & DamageType.Radiation) == DamageType.Radiation)
                {
                    if (!isRadiationDamage)
                    {
                        isRadiationDamage = true;
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("PoisonDamage");
                        _TrueGear.Play("PoisonDamage");            /////////////////////////////可能会一直触发      radiation
                        Timer radiationDamage = new Timer(RadiationDamageTimerCallBack, null, 100, Timeout.Infinite);
                    }
                    return;
                }
            }
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg($"DefaultDamage12,{angle.Key},{angle.Value}");            
            _TrueGear.PlayAngle("DefaultDamage", angle.Key, angle.Value);
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(FreightElevator), "MoveTo", new Type[] { typeof(FreightElevator.Stop) })]
        private static void FreightElevator_MoveTo_Prefix(FreightElevator __instance, FreightElevator.Stop stop)
        {
            if (__instance.targetStop == __instance.StopIndex(stop))
            {
                return;
            }
            if (!__instance.moving)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("FreightElevatorStartMoving");
                _TrueGear.Play("FreightElevatorStartMoving");
            }
        }
        [HarmonyPrefix, HarmonyPatch(typeof(FreightElevator), "StopMoving")]
        private static void FreightElevator_StopMoving_Prefix(FreightElevator __instance)
        {
            if (__instance.moving)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("FreightElevatorStopMoving");
                _TrueGear.Play("FreightElevatorStopMoving");
            }
        }


        [HarmonyPrefix, HarmonyPatch(typeof(VertigoCharacterController), "DoTeleportAnim")]
        private static void VertigoCharacterController_DoTeleportAnim_HarmonyPrefix(VertigoCharacterController __instance)
        {
            if (__instance.teleporting || __instance.disableTeleportation || __instance.disableInput)
            {
                return;
            }
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("Teleport");
            _TrueGear.Play("Teleport");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(VertigoPlayer), "GroundHit")]
        private static void VertigoPlayer_GroundHit_HarmonyPrefix(VertigoPlayer __instance, VertigoCharacterController.CharacterCollision col)
        {
            if (col.hitVelocity.y < 0f && !VertigoPlayer.godMode && !VertigoCharacterController.ignoreFallDamage && !__instance.applyingFallDamage)
            {
                float num = (col.hitVelocity.y * col.hitVelocity.y / 19.62f - __instance.fallDamage_minHeight) / (__instance.fallDamage_maxHeight - __instance.fallDamage_minHeight) * __instance.startHealth;
                if (num > 0f)
                {
                    if ((!__instance.characterController.inWater || __instance.characterController.waterBody.submersion <= 0.2f) && !VertigoCharacterController.ignoreFallDamage)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("FallDamage");
                        _TrueGear.Play("FallDamage");
                    }
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HealthStation), "CheckForInjection")]
        private static void HealthStation_CheckForInjection_Postfix(HealthStation __instance)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(__instance.needleRayPos.position, __instance.needleRayPos.forward), out raycastHit, __instance.needleRayLength, __instance.injectMask) && raycastHit.collider.GetComponent<BodyHitbox>() != null)
            {
                if (__instance.state == HealthStation.PenStates.Injecting)
                {
                    return;
                }
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("Healing");
                _TrueGear.Play("Healing");
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NanitePen), "CheckForInjection")]
        private static void NanitePen_CheckForInjection_Postfix(NanitePen __instance)
        {
            if (__instance.unstickDelay)
            {
                return;
            }
            RaycastHit raycastHit;
            if (Physics.Raycast(new Ray(__instance.needleRayPos.position, __instance.needleRayPos.forward), out raycastHit, __instance.needleRayLength, __instance.injectMask) && raycastHit.collider.GetComponent<BodyHitbox>() != null)
            {
                if (__instance.state == NanitePen.PenStates.Injecting)
                {
                    return;
                }
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("Healing");
                _TrueGear.Play("Healing");
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(JumpPad), "OnTriggerEnter")]
        private static void JumpPad_OnTriggerEnter_Prefix(JumpPad __instance, Collider other)
        {
            if (__instance.playerLayer == (__instance.playerLayer | 1 << other.gameObject.layer))
            {
                VertigoPlayer componentInParent = other.GetComponentInParent<VertigoPlayer>();
                if (componentInParent.characterController.teleporting)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LaunchLing");
                    _TrueGear.Play("LaunchLing");
                }
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(LaunchLily), "OnTriggerEnter")]
        private static void LaunchLily_OnTriggerEnter_Prefix(LaunchLily __instance, Collider other)
        {
            if (__instance.resetWaiting || __instance.retracted)
            {
                return;
            }
            if (__instance.playerLayer == (__instance.playerLayer | 1 << other.gameObject.layer))
            {
                VertigoPlayer componentInParent = other.GetComponentInParent<VertigoPlayer>();
                if (componentInParent.characterController.teleporting)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LaunchLing");
                    _TrueGear.Play("LaunchLing");
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BAMM), "Update")]
        private static void BAMM_Update_Postfix(BAMM __instance)
        {
            if (__instance.player.dead && !isDeath)
            {
                isDeath = true;
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("PlayerDeath");
                _TrueGear.Play("PlayerDeath");
                //MelonLogger.Msg("StopHeartBeat");
                _TrueGear.StopHeartBeat();
                isHeartBeat = false;
                return;
            }
            else if (!__instance.player.dead)
            {
                isDeath = false;
            }
            if (isPause)
            {
                return;
            }

            if (__instance.lowHealth && !isHeartBeat)
            {
                if (__instance.lowHealthFade > 0f)
                {
                    isHeartBeat = true;
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("StartHeartBeat");
                    _TrueGear.StartHeartBeat();
                }
            }
            else if (!__instance.lowHealth && isHeartBeat)
            {
                isHeartBeat = false;
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("StopHeartBeat");
                _TrueGear.StopHeartBeat();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(VertigoInteractable), "Grab")]
        private static void VertigoInteractable_Grab_Postfix(VertigoInteractable __instance, VertigoHand grabber)
        {
            pickupTimer.Change(100,Timeout.Infinite);
            if (!canPickup)
            {
                return;
            }
            canPickup = false;
            if (grabber.inputSource == SteamVR_Input_Sources.RightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
            //MelonLogger.Msg(grabber.inputSource);
            //MelonLogger.Msg(grabber.holding);
            //MelonLogger.Msg(__instance.isBeingHeld);
            //MelonLogger.Msg(__instance.isGrabbable);
        }

        private static void PickupItemTimerCallBack(object o)
        {
            canPickup = true;
        }

        /*
        [HarmonyPrefix, HarmonyPatch(typeof(VertigoInteractable), "Drop")]
        private static void VertigoInteractable_Drop_Prefix(VertigoInteractable __instance, VertigoHand dropper)
        {
            if (dropper.inputSource == SteamVR_Input_Sources.RightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandDropItem");
                _TrueGear.Play("RightHandDropItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandDropItem");
                _TrueGear.Play("LeftHandDropItem");
            }
        }
        */
        [HarmonyPostfix, HarmonyPatch(typeof(InventoryInstance), "AddItem")]
        private static void InventoryInstance_AddItem_Postfix(InventoryInstance __instance, InventoryItem item, int slot, ref bool __result)
        {
            if (__result)
            {
                bool isRight = GameManager.Hand_Dominant == SteamVR_Input_Sources.RightHand;
                if (isRight)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftGloveSlotInputItem");
                    _TrueGear.Play("LeftGloveSlotInputItem");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightGloveSlotInputItem");
                    _TrueGear.Play("RightGloveSlotInputItem");
                }
            }            
        }

        [HarmonyPrefix, HarmonyPatch(typeof(InventoryInstance), "TakeItem")]
        private static void InventoryInstance_TakeItem_Prefix(InventoryInstance __instance, int i, Vector3 position)
        {
            if (__instance.slots[i] != null)
            {
                bool isRight = GameManager.Hand_Dominant == SteamVR_Input_Sources.RightHand;
                if (isRight)
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("LeftGloveSlotOutputItem");
                    _TrueGear.Play("LeftGloveSlotOutputItem");
                }
                else
                {
                    //MelonLogger.Msg("------------------------------------------------");
                    //MelonLogger.Msg("RightGloveSlotOutputItem");
                    _TrueGear.Play("RightGloveSlotOutputItem");
                }
            }            
        }


        private static void SmokingDamageTimerCallBack(System.Object o)
        {
            isSomkingDamage = false;
        }

        private static void RadiationDamageTimerCallBack(System.Object o)
        {
            isRadiationDamage = false;
        }

        private static void FireDamageTimerCallBack(System.Object o)
        { 
            isFireDamage = false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Blafaladaciousnesticles), "Update")]        //剑飞出来
        private static void Blafaladaciousnesticles_Update_Prefix(Blafaladaciousnesticles __instance)
        {
            
            
            if (__instance.extendFac > 0f)
            {
                float magnitude = __instance.tip.rigidbody.velocity.magnitude;
                var myMotionHapticAccumulation = __instance.motionHapticAccumulation;
                myMotionHapticAccumulation += magnitude * Time.deltaTime;
                if (myMotionHapticAccumulation > __instance.motionHapticDistance)
                {
                    bladeShootTimer.Change(200,Timeout.Infinite);
                    if (!canBladeShoot)
                    {
                        return;
                    }
                    canBladeShoot = false;
                    bool isRightHand = __instance.inputSource == SteamVR_Input_Sources.RightHand;
                    if (isRightHand)
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("RightHandRifleShoot");
                        _TrueGear.Play("RightHandRifleShoot");
                    }
                    else
                    {
                        //MelonLogger.Msg("------------------------------------------------");
                        //MelonLogger.Msg("LeftHandRifleShoot");
                        _TrueGear.Play("LeftHandRifleShoot");
                    }                    
                }
            }
            return;
        }

        private static void BladeShootTimerCallBack(object o)
        {
            canBladeShoot = true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(PauseMenu), "OnEnable")]
        private static void PauseMenu_OnEnable_Postfix(PauseMenu __instance)
        {
            //MelonLogger.Msg("------------------------------------------------");
            //MelonLogger.Msg("StopHeartBeat");
            _TrueGear.StopHeartBeat();
            isHeartBeat = false;
            isPause = true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PauseManager), "UnPause")]
        private static void PauseManager_UnPause_Postfix(PauseManager __instance)
        {
            isPause = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameManager), "LoadLevel")]
        private static void GameManager_LoadLevel_Postfix(GameManager __instance)
        {
            isPause = false;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(SteamVR_Action_Vibration), "Execute")]
        private static void SteamVR_Action_Vibration_Execute_Postfix(SteamVR_Action_Vibration __instance,float secondsFromNow, float durationSeconds, float frequency, float amplitude, SteamVR_Input_Sources inputSource)
        {
            
            if (!(secondsFromNow == 0f && (durationSeconds > 0.01f && durationSeconds != 0.05f) && frequency == 100f && (amplitude > 0.4f && frequency != 0.6f)))
            {
                return; 
            }
            handleShockTimer.Change(100,Timeout.Infinite);
            if (!canShock)
            {
                return;
            }
            canShock = false;
            if (inputSource == SteamVR_Input_Sources.RightHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else if (inputSource == SteamVR_Input_Sources.LeftHand)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
            ////MelonLogger.Msg(secondsFromNow);
            ////MelonLogger.Msg(durationSeconds);
            ////MelonLogger.Msg(frequency);
            ////MelonLogger.Msg(amplitude);            
        }

        private static void HandleShockTimerCallBack(object o)
        { 
            canShock = true;
        }


        [HarmonyPostfix, HarmonyPatch(typeof(Tailgun), "ShootHaptics")]
        private static void Tailgun_ShootHaptics_Postfix(Tailgun __instance)
        {
            if (__instance.handle_L.isBeingHeld)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandRifleShoot");
                _TrueGear.Play("LeftHandRifleShoot");
            }
            if (__instance.handle_R.isBeingHeld)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandRifleShoot");
                _TrueGear.Play("RightHandRifleShoot");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Enlighten), "EjectMag")]
        private static void Enlighten_EjectMag_Postfix(Enlighten __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AK47), "EjectMag")]
        private static void AK47_EjectMag_Postfix(AK47 __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BoltRifle), "EjectMag")]
        private static void BoltRifle_EjectMag_Postfix(BoltRifle __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MachineGun), "ejectMag")]
        private static void MachineGun_EjectMag_Postfix(MachineGun __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TridentRifle), "EjectMag")]
        private static void TridentRifle_EjectMag_Postfix(TridentRifle __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Revolver), "EjectCartridge")]
        private static void Revolver_EjectCartridge_Postfix(Revolver __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AnnihilatorPrototype), "EjectCartridge")]
        private static void AnnihilatorPrototype_EjectCartridge_Postfix(AnnihilatorPrototype __instance)
        {
            bool isRight = __instance.inputSource == SteamVR_Input_Sources.RightHand;
            if (isRight)
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("RightHandPickupItem");
                _TrueGear.Play("RightHandPickupItem");
            }
            else
            {
                //MelonLogger.Msg("------------------------------------------------");
                //MelonLogger.Msg("LeftHandPickupItem");
                _TrueGear.Play("LeftHandPickupItem");
            }
        }

    }
}