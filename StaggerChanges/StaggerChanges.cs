using Modding;
using System;
using UnityEngine;
using HutongGames.PlayMaker;
using SFCore.Utils;

namespace StaggerChanges
{
    public class StaggerChangesMod : Mod
    {
        private static StaggerChangesMod? _instance;

        internal static StaggerChangesMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(StaggerChangesMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public StaggerChangesMod() : base("StaggerChanges")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            On.PlayMakerFSM.OnEnable += OnFSMEnable;
            On.SpellFluke.DoDamage += OnFlukenestDamage;
            On.HutongGames.PlayMaker.Actions.StringCompare.OnEnter += OnStringCompareAction;

            Log("Initialized");
        }

        //Don't damage Oomas
        private void OnStringCompareAction(On.HutongGames.PlayMaker.Actions.StringCompare.orig_OnEnter orig, HutongGames.PlayMaker.Actions.StringCompare self)
        {
            if (self.State.Name == "Invincible?" && self.Fsm.Name == "Attack" && self.Fsm.GameObject.name == "Enemy Damager")
            {
                GameObject go = self.Fsm.FsmComponent.FindFsmGameObjectVariable("Enemy").Value.gameObject;
                self.equalEvent = (go.name.Contains("Jellyfish") && !go.name.Contains("Baby")) ? FsmEvent.GetFsmEvent("INVINCIBLE") : FsmEvent.GetFsmEvent("FINISHED");
                self.notEqualEvent = (go.name.Contains("Jellyfish") && !go.name.Contains("Baby")) ? FsmEvent.GetFsmEvent("INVINCIBLE") : null;
            }

            orig(self);
        }

        //Flukenest sends TAKE DAMAGE in addition to TOOK DAMAGE
        private void OnFlukenestDamage(On.SpellFluke.orig_DoDamage orig, SpellFluke self, GameObject obj, int upwardRecursionAmount, bool burst)
        {
            HealthManager hm = obj.GetComponent<HealthManager>();
            if (!(bool)hm)
            {
                FSMUtility.SendEventToGameObject(obj.gameObject, "TAKE DAMAGE");
            }

            else if (!hm.IsInvincible || obj.tag == "Spell Vulnerable")
            {
                FSMUtility.SendEventToGameObject(obj.gameObject, "TAKE DAMAGE");
            }

            orig(self, obj, upwardRecursionAmount, burst);
        }

        //Staggers ending require TAKE DAMAGE instead of TOOK DAMAGE
        private void OnFSMEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            //Hornet Protector
            if (self.gameObject.name == "Hornet Boss 1" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun Land", "TOOK DAMAGE");
                self.AddFsmTransition("Stun Land", "TAKE DAMAGE", "Stun Recover");
            }

            //Hornet Sentinel
            else if (self.gameObject.name == "Hornet Boss 2" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun Land", "TOOK DAMAGE");
                self.AddFsmTransition("Stun Land", "TAKE DAMAGE", "Stun Recover");
            }

            //Soul Master
            else if (self.gameObject.name == "Mage Lord" && self.FsmName == "Mage Lord")
            {
                self.RemoveFsmTransition("Stunned", "TOOK DAMAGE");
                self.AddFsmTransition("Stunned", "TAKE DAMAGE", "Stun Hit");
            }

            //Soul Tyrant
            else if (self.gameObject.name == "Dream Mage Lord" && self.FsmName == "Mage Lord")
            {
                self.RemoveFsmTransition("Stunned", "TOOK DAMAGE");
                self.AddFsmTransition("Stunned", "TAKE DAMAGE", "Stun Hit");
            }

            //Broken Vessel
            else if (self.gameObject.name == "Infected Knight" && self.FsmName == "IK Control")
            {
                self.RemoveFsmTransition("Stunned", "TOOK DAMAGE");
                self.AddFsmTransition("Stunned", "TAKE DAMAGE", "Stun Recover");
            }

            //Lost Kin
            else if (self.gameObject.name == "Lost Kin" && self.FsmName == "IK Control")
            {
                self.RemoveFsmTransition("Stunned", "TOOK DAMAGE");
                self.AddFsmTransition("Stunned", "TAKE DAMAGE", "Stun Recover");
            }

            //The Hollow Knight
            else if (self.gameObject.name == "Hollow Knight Boss" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun", "TOOK DAMAGE");
                self.AddFsmTransition("Stun", "TAKE DAMAGE", "StunHitUp");
            }

            //Pure Vessel
            else if (self.gameObject.name == "HK Prime" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun", "TOOK DAMAGE");
                self.AddFsmTransition("Stun", "TAKE DAMAGE", "StunHitUp");
            }

            //Dung Defender
            else if (self.gameObject.name == "Dung Defender" && self.FsmName == "Dung Defender")
            {
                self.RemoveFsmTransition("Stun Land", "TOOK DAMAGE");
                self.AddFsmTransition("Stun Land", "TAKE DAMAGE", "Stun Recover");
            }

            //Collector
            else if (self.gameObject.name == "Jar Collector" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun", "TOOK DAMAGE");
                self.AddFsmTransition("Stun", "TAKE DAMAGE", "Stun Hit");
            }

            //Hive Knight
            else if (self.gameObject.name == "Hive Knight" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun Land", "TOOK DAMAGE");
                self.AddFsmTransition("Stun Land", "TAKE DAMAGE", "Stun Recover");
            }

            //Grey Prince Zote
            else if (self.gameObject.name == "Grey Prince" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun", "TOOK DAMAGE");
                self.AddFsmTransition("Stun", "TAKE DAMAGE", "Jump Antic");
            }

            //Paintmaster Sheo
            else if (self.gameObject.name == "Sheo Boss" && self.FsmName == "nailmaster_sheo")
            {
                self.RemoveFsmTransition("Stun Land", "TOOK DAMAGE");
                self.AddFsmTransition("Stun Land", "TAKE DAMAGE", "Stun Recover");
            }

            //Great Nailsage Sly
            else if (self.gameObject.name == "Sly Boss" && self.FsmName == "Control")
            {
                self.RemoveFsmTransition("Stun", "TOOK DAMAGE");
                self.AddFsmTransition("Stun", "TAKE DAMAGE", "Stun Leave");
            }
        }
    }
}
