using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ArcheBuddy.Bot.Classes;
using System.Threading.Tasks;

namespace BasicSportFishing
{
    public class BasicSportFishing : Core
    {
        public static string GetPluginAuthor()
        {
            return "tictoc & shigato";
        }

        public static string GetPluginVersion()
        {
            return "1.2";
        }

        public static string GetPluginDescription()
        {
            return "BasicSportFishing";
        }

        CancellationTokenSource tokenSource;
        CancellationToken token;

        public void PluginRun()
        {
            ClearLogs();

            Log("++++++++++++++++++++ Plugin started ++++++++++++++++++++");

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            Task skillCancelerTask = new Task(() => SkillCanceler(token));
            skillCancelerTask.Start();

            while (true)
            {
                Thread.Sleep(10);

                WaitForTarget();

                // Face the fish
                if (angle(me.target, me) > 45 && angle(me.target, me) < 315)
                {
                    //Log(DateTime.Now.ToLongTimeString() + " Turn to Fish");            
                    TurnDirectly(me.target);
                }

                // Select the corresponding response to the fish's buffs
                // Log(DateTime.Now.ToLongTimeString() + " Check buffs");            
                if (buffTime(me.target, 5265) > 0 && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Use Skill: Stand Firm Left");
                    TurnDirectly();
                    UseSkill(21194);
                }
                if (buffTime(me.target, 5264) > 0 && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Use Skill: Stand Firm Right");
                    TurnDirectly();
                    UseSkill(21135);
                }
                if (buffTime(me.target, 5267) > 0 && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Use Skill: Give Slack");
                    TurnDirectly();
                    UseSkill(21195);
                }
                if (buffTime(me.target, 5266) > 0 && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Use Skill: Reel In");
                    TurnDirectly();
                    UseSkill(21196);
                }
                if (buffTime(me.target, 5508) > 0 && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Use Skill: Big Reel In");
                    TurnDirectly();
                    UseSkill(21290);
                }
                if (!hasBuff() && !me.isCasting)
                {
                    Log(DateTime.Now.ToLongTimeString() + " Precasting: Big Reel In");
                    TurnDirectly();
                    UseSkill(21290);
                }

            }
        }

        /// <summary>
        /// Checks if target has a buff
        /// </summary>
        /// <returns>True or False</returns>
        public bool hasBuff()
        {
            // Does target have one of these buffs?
            if (buffTime(me.target, 5265) > 0 || buffTime(me.target, 5264) > 0 || buffTime(me.target, 5267) > 0 || buffTime(me.target, 5266) > 0 || buffTime(me.target, 5508) > 0)
            {
                return true;
            }

            return false;
        }

        //Call on plugin stop
        public void PluginStop()
        {
            tokenSource.Cancel();
            Log("++++++++++++++++++++ Plugin stopped ++++++++++++++++++++");
        }

        // Wait until i do have a valid target
        public void WaitForTarget()
        {
            //Log(DateTime.Now.ToLongTimeString() + " Waiting for Target");

            // Wait when: I have no target || My target is not targeting me || My target has no "Strength Contest" buff (Strength Contest - ID: 5715)
            while (me.target == null || (me.target.target != null && me.target.target != me) || getBuff(me.target, 5715) == null)
            {
                Thread.Sleep(50);
            }
        }

        // Cancel skill if buff has changed
        public void SkillCanceler(CancellationToken tk)
        {
            // Delay before i cancel a skill (50 - 99 ms)
            // Usage: delay = r.Next(minDelay, maxDelay);
            int delay;
            int minDelay = 50;
            int maxDelay = 100;
            // Create new Random object.
            Random r = new System.Random();

            while (true)
            {
                Thread.Sleep(10);

                if (tk.IsCancellationRequested)
                {
                    return;
                }

                // Do i have a target? If not, wait for target.
                if (me.target == null)
                    continue;

                // Do i cast a skill?
                var myCastSkillId = me.castSkillId;
                if (myCastSkillId > 0)
                {
                    delay = r.Next(minDelay, maxDelay);

                    // Does the casted skill match the current buff? If not, then cancel the skill.
                    if (buffTime(me.target, 5265) > 0 && myCastSkillId != 21194)
                        CancelWithDelay(delay);
                    if (buffTime(me.target, 5264) > 0 && myCastSkillId != 21135)
                        CancelWithDelay(delay);
                    if (buffTime(me.target, 5267) > 0 && myCastSkillId != 21195)
                        CancelWithDelay(delay);
                    if (buffTime(me.target, 5266) > 0 && myCastSkillId != 21196)
                        CancelWithDelay(delay);
                    if (buffTime(me.target, 5508) > 0 && myCastSkillId != 21290)
                        CancelWithDelay(delay);

                    // Target has no valid buff, and i'm not casting Big Reel In
                   if(!hasBuff() && me.isCasting && myCastSkillId != 21290)
                    {
                        CancelWithDelay(delay);
                    }

                }
            }
        }
        public void CancelWithDelay(int delay)
        {
            // Add a small random delay before canceling the skill to make it more "human"
            Thread.Sleep(delay);
            //Log(DateTime.Now.ToLongTimeString() + " Cancel Skill");
            CancelSkill();
        }
    }
}
    
