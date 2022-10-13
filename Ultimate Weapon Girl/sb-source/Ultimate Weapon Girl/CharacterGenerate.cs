using OpenTK;
using OpenTK.Graphics;
using StorybrewCommon.Mapset;
using StorybrewCommon.Scripting;
using StorybrewCommon.Storyboarding;
using StorybrewCommon.Storyboarding.Util;
using StorybrewCommon.Subtitles;
using StorybrewCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;

namespace StorybrewScripts
{
    public class CharacterGenerate : StoryboardObjectGenerator
    {
        public override void Generate()
        {
            var layer = GetLayer("Character-Foreground");

            Sora sora = new Sora(layer, Beatmap);

            sora.NextAction(NAction.damage, 30000);

            sora.NextAction(NAction.ok, 35000);

            sora.NextAction(NAction.attack, 40000);

            sora.Current.Sprite.Move(60000, 640, 380);
        }

        public class Character
        {
            public virtual OsbSprite Sprite { get; set; }

            public Beatmap Beatmap { get; set; }

            /// <summary>
            /// 角色是否在原来的反向
            /// </summary>
            public bool IsForward { get; set; } = true;

            public bool Disabled { get; set; } = false;

            /// <summary>
            /// 图片是否是原比例
            /// </summary>
            public bool VecOri { get; set; } = true;

            public NAction Type;

            public Character(NAction type, StoryboardLayer layer, Beatmap beatmap, string spritPath, int startTime = 0, int startX = 640, int startY = 320)
            {
                Beatmap = beatmap;
            
                Sprite = layer.CreateSprite(spritPath, OsbOrigin.BottomCentre, new Vector2(startX, startY));

                Type = type;

                //Sprite.Rotate
            }

            public void Flip(int startTime, int endTime, int times = 1)
            {
                int intervalTime = (endTime - startTime) / times;

                endTime = startTime + intervalTime;

                for (int i = 0; i < times; i++)
                {
                    Sprite.ScaleVec(startTime, endTime, 1, 1, 0, 1);

                    Sprite.FlipH(endTime);

                    IsForward = !IsForward;

                    Sprite.ScaleVec(endTime, endTime += intervalTime, 0, 1, 1, 1);

                    startTime = endTime + intervalTime;

                    endTime = startTime + intervalTime;
                }
            }

            public void FlipIn(int startTime, int endTime)
            {
                Sprite.Fade(startTime, 1);

                if (!IsForward)
                {
                    Sprite.FlipH(endTime);
                    IsForward = true;
                }

                Sprite.ScaleVec(startTime, endTime, 0, 1, 1, 1);

                Disabled = false;
                VecOri = true;
            }

            public void FlipOut(int startTime,int endTime)
            {
                Sprite.ScaleVec(startTime, endTime, 1, 1, 0, 1);
                Sprite.Fade(endTime, 0);

                Disabled = true;
                VecOri = false;
            }
        }

        public class Sora
        {
            public NAction NowAction { get; }

            private Character[] spritesList;

            public Character Current { get; set; }

            /// <summary>
            /// 每拍的时间
            /// </summary>
            public int beatDuration;

            public Sora(StoryboardLayer layer, Beatmap beatmap)
            {
                beatDuration = (int)beatmap.TimingPoints.FirstOrDefault().BeatDuration;

                spritesList = new Character[]
                {
                    new Character(NAction.normal,layer,beatmap, "sb/character/cuties/normal-sora.png", startX:360, startY:385),
                    new Character(NAction.ok,layer,beatmap, "sb/character/cuties/offical-ok-sora.png", startX:360, startY:385),
                    new Character(NAction.attack,layer,beatmap, "sb/character/cuties/offical-attack-sora.png", startX:360, startY:385),
                    new Character(NAction.damage,layer,beatmap, "sb/character/cuties/offical-damage-sora.png", startX:360, startY:385),
                };

                Current = spritesList.Single(s => s.Type == NAction.normal);

                Current.Sprite.Fade(0, 1);

                
            }

            public void NextAction(NAction action, int time)
            {
                if (Current.Type == action)
                {
                    throw new Exception("已经是这个动作了");
                }

                Vector2 currentPositon = Current.Sprite.PositionAt(time);

                Current.FlipOut(time, time + beatDuration / 4);
                Current = spritesList.Single(s => s.Type == action);

                Current.Sprite.Move(time + beatDuration / 4, currentPositon);
                Current.FlipIn(time + beatDuration / 4, time + beatDuration / 2);
            }
        }

        public enum NAction
        {
            normal,
            ok,
            attack,
            damage,
        }
    }
}
