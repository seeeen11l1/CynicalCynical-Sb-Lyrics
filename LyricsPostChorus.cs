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
using System.Drawing;
using System.IO;

namespace StorybrewScripts
{
    public class LyricsPostChorus : StoryboardObjectGenerator
    {
        [Configurable] public int FontSize;
        [Configurable] public Color4 Color;
        [Configurable] public string FontPath;
        [Configurable] public FontStyle FontStyle;
        [Configurable] public bool TrimTransparency;
        [Configurable] public Vector2 Padding = Vector2.Zero;
        [Configurable] public bool EffectsOnly;
        [Configurable] public string LyricsDirectory;
        [Configurable] public float FontScale;
        [Configurable] public Color4 SquareColor;
        int beatDuration => (int)Beatmap.GetTimingPointAt(0).BeatDuration;
        private List<LyricModelMade> NewLyrics = new List<LyricModelMade>();
        private FontGenerator font => LoadFont("sb/lyrics/post", new FontDescription()
        {
            FontSize = FontSize,
            Color = Color,
            FontPath = FontPath,
            FontStyle = FontStyle,
            TrimTransparency = TrimTransparency,
            Padding = Padding,
            EffectsOnly = EffectsOnly
        });
        public override void Generate()
        {
		    List<string> textLyrics = File.ReadAllLines(Path.Combine(ProjectPath, LyricsDirectory)).ToList();
            List<double> startTimes = GetBeatmap("postChorus").HitObjects.Select(o => o.StartTime).ToList();
            List<double> endTimes = GetBeatmap("postChorus").HitObjects.Select(o => o.EndTime).ToList();
            List<string> specificLinesList = new List<string> {"いつか", "通じ合えるその日まで", "色褪せぬ宇宙（そら）にいようよ", "いつか信じ合えたその日には"
            ,"世界の終わりをみようよ", "そして", "巡り会えたその日から", "飽きるまで恋をしようよ", "ねえ彗星みたいな日々だけど", "最期は微笑っていたいよ"
            ,"ねえ貴方の口から伝えてね", "秘密の五文字の向こうを", "色褪せぬ宇宙（そら）にいたいよ", "世界の終わりがみたいよ", "飽きるまで恋がしたいよ", "あなたの言葉、想像、未来を。", 
            "私の言葉、放課後、期待を。", "いつか奪った群青の恋を", "ふたり誓った真実の夜を"};

            for(int i = 0; i < Math.Min(startTimes.Count, textLyrics.Count); i++)
            {
                NewLyrics.Add(new LyricModelMade{
                    startTimes = startTimes[i],
                    endTimes = endTimes[i],
                    lyricLines = textLyrics[i]
                });
            }

            var layer = GetLayer("postChorusLyrics");
            GenerateLyrics(layer, specificLinesList);
        }

        public void GenerateLyrics(StoryboardLayer layer, List<string> specificLinesList)
        {
            var theFont = font;
            var behindLayer = GetLayer("BackLayer");
            var behindBitmap = GetMapsetBitmap("sb/shapes/squareR.png");
            var itemList = 0;
            foreach(var line in NewLyrics)
            {
                var lineWidth = 0f;
                var lineHeight = 0f;
                
                var lineXPos = 0f;
                var lineYPos = 0f;
                foreach(var character in line.lyricLines)
                {
                    var texture = theFont.GetTexture(character.ToString());
                    if(specificLinesList[0] == line.lyricLines || specificLinesList[5] == line.lyricLines)
                    {
                        FontScale = 0.07f;
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = texture.BaseHeight * FontScale;
                        lineXPos = 320f;
                        lineYPos = 240;
                    }

                    if(specificLinesList[1] == line.lyricLines || specificLinesList[2] == line.lyricLines 
                    || specificLinesList[3] == line.lyricLines || specificLinesList[4] == line.lyricLines 
                    || specificLinesList[12] == line.lyricLines || specificLinesList[13] == line.lyricLines)
                    {
                        FontScale = 0.03f;
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = texture.BaseHeight * FontScale;
                        lineXPos = 320f;
                        lineYPos = 400f;
                    }

                    if(specificLinesList[6] == line.lyricLines || specificLinesList[7] == line.lyricLines 
                    || specificLinesList[8] == line.lyricLines || specificLinesList[9] == line.lyricLines 
                    || specificLinesList[14] == line.lyricLines)
                    {
                        FontScale = 0.03f;
                        lineWidth = texture.BaseWidth * FontScale;
                        lineHeight += texture.BaseHeight * FontScale;
                        lineXPos = 0f;
                        lineYPos = 240f;
                    }

                    if(specificLinesList[10] == line.lyricLines)
                    {
                        FontScale = 0.03f;
                        lineWidth = texture.BaseWidth * FontScale;
                        lineHeight += texture.BaseHeight * FontScale;
                        lineXPos = 0f;
                        lineYPos = 240f;
                    }

                    if(specificLinesList[11] == line.lyricLines)
                    {

                        FontScale = 0.05f;
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = texture.BaseHeight * FontScale;
                        lineXPos = 320f;
                        lineYPos = 240f;
                    }

                    if(specificLinesList[15] == line.lyricLines  || specificLinesList[16] == line.lyricLines)
                    {
                        FontScale = 0.04f;
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = texture.BaseHeight * FontScale;
                        lineXPos = 120f;
                        lineYPos = 205f;
                    }

                    if(specificLinesList[17] == line.lyricLines  || specificLinesList[18] == line.lyricLines)
                    {
                        FontScale = 0.04f;
                        lineWidth += texture.BaseWidth * FontScale;
                        lineHeight = texture.BaseHeight * FontScale;
                        lineXPos = 460f;
                        lineYPos = 100f;
                    }

                }
                if(specificLinesList[16] == line.lyricLines || specificLinesList[18] == line.lyricLines)
                {
                    lineXPos += 40;
                    lineYPos += 70;
                }
                

                var xPosition = lineXPos - lineWidth * 0.5f;
                var yPosition = lineYPos - lineHeight * 0.5f;

                

                
                
                foreach(var character in line.lyricLines)
                {
                    var texture = theFont.GetTexture(character.ToString());
                    if(!texture.IsEmpty)
                    {
                        var position = new Vector2(xPosition, yPosition) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                        var lyricSprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);

                        if(specificLinesList[0] == line.lyricLines || specificLinesList[5] == line.lyricLines)
                        {
                            var distanceFromCenter = (320 - lyricSprite.InitialPosition.X) / 2;

                            lyricSprite.Move(OsbEasing.OutBack, line.startTimes, line.endTimes, lyricSprite.InitialPosition, 
                            new Vector2(lyricSprite.InitialPosition.X - distanceFromCenter, lyricSprite.InitialPosition.Y));

                            lyricSprite.Scale(line.startTimes, 0.07f);
                            xPosition += texture.BaseWidth * FontScale;
                        }

                        if(specificLinesList[1] == line.lyricLines || specificLinesList[2] == line.lyricLines 
                        || specificLinesList[3] == line.lyricLines || specificLinesList[4] == line.lyricLines 
                        || specificLinesList[12] == line.lyricLines || specificLinesList[13] == line.lyricLines)
                        {
                            lyricSprite.Scale(line.startTimes, 0.03f);
                            xPosition += texture.BaseWidth * FontScale;

                            var behindBlock = behindLayer.CreateSprite("sb/shapes/squareR.png", OsbOrigin.Centre, new Vector2(lineXPos, lineYPos));
                            behindBlock.Color(line.startTimes, SquareColor);


                            behindBlock.ScaleVec(line.startTimes, new Vector2(lineWidth / behindBitmap.Width, lineHeight / behindBitmap.Height));
                            behindBlock.Fade(line.startTimes, line.startTimes, 0, 1);
                            behindBlock.Fade(line.endTimes, line.endTimes, 1, 0);
                        }

                        if(specificLinesList[6] == line.lyricLines || specificLinesList[7] == line.lyricLines 
                        || specificLinesList[8] == line.lyricLines || specificLinesList[9] == line.lyricLines 
                        || specificLinesList[14] == line.lyricLines)
                        {
                            lyricSprite.Scale(line.startTimes, 0.03f);
                            yPosition += texture.BaseHeight * FontScale;
                            var behindBlock = behindLayer.CreateSprite("sb/shapes/squareR.png", OsbOrigin.Centre, new Vector2(lineXPos, lineYPos));

                            behindBlock.Color(line.startTimes, SquareColor);

                            behindBlock.ScaleVec(line.startTimes, new Vector2(lineWidth / behindBitmap.Width * 6, lineHeight / behindBitmap.Height * 1.1f));

                            behindBlock.Fade(line.startTimes, line.startTimes, 0, 1);
                            behindBlock.Fade(line.endTimes, line.endTimes, 1, 0);
                        }

                        if(specificLinesList[10] == line.lyricLines)
                        {
                            lyricSprite.Scale(line.startTimes, 0.03f);
                            yPosition += texture.BaseHeight * FontScale;
                            var behindBlock = behindLayer.CreateSprite("sb/shapes/squareR.png", OsbOrigin.Centre, new Vector2(lineXPos, lineYPos));
                            behindBlock.Color(line.startTimes, SquareColor);


                            lyricSprite.Move(OsbEasing.In, line.endTimes - beatDuration, line.endTimes, lyricSprite.InitialPosition, 
                            new Vector2(lyricSprite.InitialPosition.X, lyricSprite.InitialPosition.Y + 100));

                            behindBlock.ScaleVec(line.startTimes, new Vector2(lineWidth / behindBitmap.Width * 6, lineHeight / behindBitmap.Height * 1.1f));

                            behindBlock.Move(OsbEasing.In, line.endTimes - beatDuration, line.endTimes, behindBlock.InitialPosition, 
                            new Vector2(behindBlock.InitialPosition.X, behindBlock.InitialPosition.Y + 100));

                            behindBlock.Fade(line.startTimes, line.startTimes, 0, 1);
                            behindBlock.Fade(line.endTimes, line.endTimes, 1, 0);
                        }

                        if(specificLinesList[11] == line.lyricLines)
                        {
                            var distanceFromCenter = (320 - lyricSprite.InitialPosition.X) / 2;
                            lyricSprite.Scale(line.startTimes, 0.05f);

                            lyricSprite.Move(OsbEasing.OutExpo, line.startTimes, line.endTimes, lyricSprite.InitialPosition, 
                            new Vector2(lyricSprite.InitialPosition.X - distanceFromCenter, lyricSprite.InitialPosition.Y));

                            xPosition += texture.BaseWidth * FontScale;
                        }


                        if(specificLinesList[15] == line.lyricLines  || specificLinesList[16] == line.lyricLines)
                        {
                            lyricSprite.Scale(line.startTimes, 0.04f);
                            xPosition += texture.BaseWidth * FontScale;
                        }
                        
                        if(specificLinesList[17] == line.lyricLines  || specificLinesList[18] == line.lyricLines)
                        {
                            lyricSprite.Scale(line.startTimes, 0.04f);
                            var characterList = new List<string> {"sb/lyrics/Anim/koi/love.png", "sb/lyrics/Anim/night/night.png"};
                            var characterPos = new List<Vector2> {new Vector2(555, 100), new Vector2(595, 170)};
                            

                            if(character.ToString() == "恋" || character.ToString() == "夜")
                            {
                                var lyricAnim = layer.CreateAnimation(characterList[itemList], 3, 200, OsbLoopType.LoopForever, OsbOrigin.Centre);
                                lyricAnim.Move(line.startTimes, characterPos[itemList]);

                                lyricAnim.Scale(line.startTimes, 0.07f);

                                lyricAnim.Fade(line.startTimes, line.startTimes, 0, 1);
                                lyricAnim.Fade(line.endTimes, line.endTimes, 1, 0);

                                itemList++;
                                xPosition += texture.BaseWidth * FontScale * 2;
                                continue;
                            }
                            xPosition += texture.BaseWidth * FontScale;
                        }

                        lyricSprite.Fade(line.startTimes, line.startTimes, 0, 1);
                        lyricSprite.Fade(line.endTimes, line.endTimes, 1, 0);
            
                    }
                }
            }
           
        }
    }

    public class LyricModelMade
    {
        public double startTimes;
        public double endTimes;
        public string lyricLines;
    }
}
