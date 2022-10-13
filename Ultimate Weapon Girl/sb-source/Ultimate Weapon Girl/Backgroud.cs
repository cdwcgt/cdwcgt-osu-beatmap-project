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
using System.IO.Ports;
using System.Linq;

namespace StorybrewScripts
{
    public class Backgroud : StoryboardObjectGenerator
    {
        public override void Generate()
        {
            var bg = GetLayer("background");

            var Foreground = GetLayer("background-Foreground");

            var stage = Foreground.CreateSprite("sb/stage.png", OsbOrigin.Centre);
            var starship = bg.CreateSprite("sb/starship_l.png", OsbOrigin.BottomCentre);

            starship.Move(OsbEasing.None, 0, 259963, 1024, 0, 0, 1024);
            starship.Scale(0, 3);
            starship.Fade(0, 1);
            starship.Fade(259963, 0);

            stage.Fade(0, 1);
            stage.Scale(0, 0.5);
            stage.Fade(259963, 0);
        }
    }
}
