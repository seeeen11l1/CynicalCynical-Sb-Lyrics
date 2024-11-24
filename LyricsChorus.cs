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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;

namespace StorybrewScripts
{
    public class LyricsChorus : StoryboardObjectGenerator
    {  
        [Configurable] public int FontSize;
        [Configurable] public Color4 Color;
        [Configurable] public string FontPath;
        [Configurable] public FontStyle FontStyle;
        [Configurable] public bool TrimTransparency;
        [Configurable] public Vector2 Padding = Vector2.Zero;
        [Configurable] public bool EffectsOnly;
        [Configurable] public float FontScale;
        [Configurable] public string LyricsDirectory;
        [Configurable] public Color4 SquareColor;
        [Configurable] public Color4 OtherSquareColor;
        int beatDuration => (int)Beatmap.GetTimingPointAt(0).BeatDuration;
        private List<LyricModel> NewLyrics = new List<LyricModel>();
        private FontGenerator Font => LoadFont("sb/lyrics/chorus", new FontDescription()
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
		    List<double> endTimeList = GetBeatmap("chorusTiming").HitObjects.Select(x => x.EndTime).ToList();
            List<double> startTimeList = GetBeatmap("chorusTiming").HitObjects.Select(x => x.StartTime).ToList();
            List<string> specificLinesList = new List<string> {"宇宙の果てまで", "私だけ連れ去って", "走馬灯が", "みられるなら", "ねえ"
            ,"あなたの横顔がいい" , "大きなその手で" ,"私だけ触れてみて", "ありのままを", "信じるから", "私を選んでみて？", "叶えて"
            , "今夜は", "月が満ちる", "宇宙の果てまで", "あなただけ連れ去るね", "走馬灯を", "信じたなら", "私の泣き顔は嫌？", 
            "魔法じゃ解けない", "夢をみてしまおうよ", "運命すら", "下らないよ", "そう", "私とふたりでいよう。"};

            var layer = GetLayer("ChorusLyricsMain");

            for(int i = 0; i < Math.Min(startTimeList.Count, textLyrics.Count); i++)
            {
                NewLyrics.Add(new LyricModel{
                    startTime = startTimeList[i],
                    endTime = endTimeList[i],
                    lyricLine = textLyrics[i]
                });
            }

            GenerateLyrics(layer, specificLinesList, 0);
        }

        public void GenerateLyrics(StoryboardLayer layer, List<string> specificLines, int endTime)
        {
            var textFont = Font;

            foreach(var line in NewLyrics)
            {
                var lineWidth = 0f;
                var lineHeight = 0f; 
                foreach(var character in line.lyricLine)
                {
                    if(specificLines[0] == line.lyricLine || specificLines[6] == line.lyricLine || 
                    specificLines[14] == line.lyricLine || specificLines[19] == line.lyricLine)
                        FontScale = 0.25f;
                    if(specificLines[1] == line.lyricLine || specificLines[7] == line.lyricLine || 
                        specificLines[15] == line.lyricLine || specificLines[20] == line.lyricLine)
                        FontScale = 0.1f;
                    if(specificLines[2] == line.lyricLine || specificLines[8] == line.lyricLine || 
                        specificLines[16] == line.lyricLine || specificLines[21] == line.lyricLine)
                        FontScale = 0.18f;
                    if(specificLines[3] == line.lyricLine || specificLines[9] == line.lyricLine || 
                        specificLines[17] == line.lyricLine || specificLines[22] == line.lyricLine)
                        FontScale = 0.15f;
                    if(specificLines[4] == line.lyricLine || specificLines[23] == line.lyricLine)
                        FontScale = 0.5f;
                    if(specificLines[5] == line.lyricLine || specificLines[10] == line.lyricLine || 
                        specificLines[18] == line.lyricLine || specificLines[24] == line.lyricLine)
                        FontScale = 0.135f;
                    if(specificLines[11] == line.lyricLine)
                        FontScale = 0.03f;
                    var texture = textFont.GetTexture(character.ToString());
                    lineWidth += texture.BaseWidth * FontScale;
                    lineHeight = texture.BaseHeight * FontScale;
                }

                var xPosition = 320f - lineWidth * 0.5f;
                var yPosition = 240f - lineHeight * 0.5f;
                var addItem = 1;

                foreach(var character in line.lyricLine)
                {
                    var texture = textFont.GetTexture(character.ToString());
                    
                    if(!texture.IsEmpty)
                    {
                        var position = new Vector2(xPosition, yPosition) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                        var lyricSprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);

                        if(specificLines[0] == line.lyricLine || specificLines[6] == line.lyricLine || 
                        specificLines[14] == line.lyricLine || specificLines[19] == line.lyricLine)
                        {
                            // Figure this out laters                     
                            var newLayer = GetLayer("BackLyrics");
                            var backLyricSprite = newLayer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                           
                            backLyricSprite.Fade(line.startTime, line.startTime, 0, 1);
                            backLyricSprite.Fade(line.endTime, line.endTime, 1, 0);

                            backLyricSprite.ScaleVec(line.startTime, new Vector2(0.3f, 1f));
                            backLyricSprite.Move(OsbEasing.OutExpo, line.startTime, line.endTime, new Vector2(280, 240), position);
                            xPosition += texture.BaseWidth * FontScale;
                        }

                        if(specificLines[1] == line.lyricLine || specificLines[7] == line.lyricLine || 
                        specificLines[15] == line.lyricLine || specificLines[20] == line.lyricLine)
                        {
                            // Add block behind
                            var randomYPos = Random(-60, 60);
                            var randomScale = Random(0.03, 0.05);
                            var squareBitmap = GetMapsetBitmap("sb/shapes/squareR.png");

                            position = new Vector2(xPosition, yPosition + randomYPos) + texture.OffsetFor(OsbOrigin.Centre) * FontScale;
                            var behindLyricLayer = GetLayer("BehindLyricBlock");
                            var squareBehindBlock = behindLyricLayer.CreateSprite("sb/shapes/squareR.png", OsbOrigin.Centre, lyricSprite.PositionAt(line.startTime));

                            lyricSprite.Move(OsbEasing.OutExpo, line.startTime, line.endTime - beatDuration, position, lyricSprite.InitialPosition);
                            squareBehindBlock.Move(OsbEasing.OutExpo, line.startTime, line.endTime - beatDuration, position, lyricSprite.InitialPosition);
                            
                            squareBehindBlock.Color(line.startTime, line.startTime, SquareColor, SquareColor);
                            squareBehindBlock.Color(line.endTime, line.endTime, SquareColor, SquareColor);
                            squareBehindBlock.Color(line.endTime + 50, line.endTime + 50, Color4.White, Color4.White);
                            squareBehindBlock.Color(line.endTime + 100, line.endTime + 100, Color4.Black, Color4.Black);

                            squareBehindBlock.Fade(line.startTime, line.startTime, 0, 1);
                            squareBehindBlock.Fade(line.endTime + 150, line.endTime + 150, 1, 0);

                            if(character.ToString() == "私" || character.ToString() == "連" || character.ToString() == "去" || character.ToString() == "触" || character.ToString() == "夢")
                            {
                                lyricSprite.Scale(line.startTime, 0.1);
                                squareBehindBlock.Scale(line.startTime, texture.BaseWidth * 0.11f / squareBitmap.Width);
                            } else{
                                lyricSprite.Scale(line.startTime, randomScale);
                                squareBehindBlock.Scale(line.startTime, texture.BaseWidth * (randomScale + 0.01f) / squareBitmap.Width);
                            }
                        }
 
                        if(specificLines[2] == line.lyricLine || specificLines[8] == line.lyricLine || 
                        specificLines[16] == line.lyricLine || specificLines[21] == line.lyricLine)
                        {
                            var yPlacement = 180;
                            var squareBitmap = GetMapsetBitmap("sb/shapes/squareR.png");

                            lyricSprite.Move(line.startTime, new Vector2(lyricSprite.InitialPosition.X, yPlacement));
                            lyricSprite.Scale(line.startTime, 0.08f);
                            lyricSprite.Move(OsbEasing.InQuad, line.endTime - beatDuration, line.endTime, 
                            new Vector2(lyricSprite.InitialPosition.X, yPlacement), new Vector2(320, yPlacement));

                            var behindLyricLayer = GetLayer("BehindLyricBlock");
                            var squareBehindBlock = behindLyricLayer.CreateSprite("sb/shapes/squareR.png", OsbOrigin.Centre, lyricSprite.PositionAt(line.startTime));
                            
                            squareBehindBlock.Color(line.startTime, OtherSquareColor);

                            squareBehindBlock.ScaleVec(OsbEasing.OutExpo, line.startTime, line.endTime, new Vector2(texture.BaseWidth * 0.05f / squareBitmap.Width, 0),
                            new Vector2(texture.BaseWidth * 0.05f / squareBitmap.Width, texture.BaseHeight * 0.08f / squareBitmap.Height));

                            squareBehindBlock.Move(OsbEasing.InQuad, line.endTime - beatDuration, line.endTime, 
                            new Vector2(lyricSprite.InitialPosition.X, yPlacement), new Vector2(320, yPlacement));

                            squareBehindBlock.Fade(line.startTime, line.startTime, 0, 0.5f);
                            squareBehindBlock.Fade(line.endTime, line.endTime, 0.5f, 0);
                        }

                        if(specificLines[3] == line.lyricLine || specificLines[9] == line.lyricLine || 
                        specificLines[17] == line.lyricLine || specificLines[22] == line.lyricLine)
                        {
                            var yPlacement = 240;
                            lyricSprite.Move(line.startTime, new Vector2(lyricSprite.InitialPosition.X, yPlacement));
                            lyricSprite.Scale(line.startTime, 0.15f);
                            lyricSprite.Move(OsbEasing.InQuad, line.endTime - beatDuration, line.endTime, 
                            new Vector2(lyricSprite.InitialPosition.X, yPlacement), new Vector2(320, yPlacement));
                        }

                        if(specificLines[4] == line.lyricLine || specificLines[23] == line.lyricLine)
                        {
                            lyricSprite.Scale(line.startTime, 0.5f);
                            lyricSprite.Move(OsbEasing.Out, line.startTime, line.endTime, new Vector2(320, 240), lyricSprite.InitialPosition);
                        }

                        if(specificLines[5] == line.lyricLine || specificLines[10] == line.lyricLine || 
                        specificLines[18] == line.lyricLine || specificLines[24] == line.lyricLine)
                        {
                            var halfCircle = 0f;
                            // Work on this
                            for(int j = 1; j < 3; j++)
                            {
                                var circleLyricSprite = layer.CreateSprite(texture.Path, OsbOrigin.Centre, position);
                                if(specificLines[5] == line.lyricLine)
                                {
                                    halfCircle = 360f / line.lyricLine.Count();
                                } else
                                {
                                    halfCircle = 360f / line.lyricLine.Count() / 2;
                                }
                                double angle = halfCircle * addItem;
                                var inRadius = 100f * (j * 1.2f);

                                circleLyricSprite.Move(line.startTime - 100, CircleRotationPositions(angle, inRadius));
                                circleLyricSprite.Rotate(line.startTime - 100, DegreeToRadians(90 + angle));
                                var turnAngle = 0f;
                                var minusSpeed = 13f;
                                var radiusOut = 10f;
                                
                                for(double i = line.startTime - 100; i < line.endTime; i += 100)
                                {
                                    circleLyricSprite.Move(i, i + 100, circleLyricSprite.PositionAt(i), CircleRotationPositions(angle + turnAngle, inRadius));
                                    circleLyricSprite.Rotate(i, i + 100, circleLyricSprite.RotationAt(i), DegreeToRadians(90 + angle + turnAngle));

                                    turnAngle -= minusSpeed;

                                    inRadius += radiusOut;

                                    radiusOut *= 0.93f;
                                    minusSpeed -= 0.5f;
                                }
                                circleLyricSprite.Scale(line.startTime, 0.135f * j);

                                circleLyricSprite.Fade(line.startTime, line.startTime, 0, 1);
                                circleLyricSprite.Fade(line.endTime - beatDuration * j, line.endTime - beatDuration * j, 1, 0);

                                xPosition += texture.BaseWidth * FontScale;
                                addItem++;
                                
                            }
                            
                        }

                        if(specificLines[11] == line.lyricLine || specificLines[12] == line.lyricLine || specificLines[13] == line.lyricLine)
                        {
                            lyricSprite.Scale(line.startTime, 0.03f);
                        }

                        if(specificLines[0] == line.lyricLine || specificLines[6] == line.lyricLine || 
                            specificLines[14] == line.lyricLine || specificLines[19] == line.lyricLine)
                            continue;
                        
                        if(specificLines[5] == line.lyricLine || specificLines[10] == line.lyricLine || 
                        specificLines[18] == line.lyricLine || specificLines[24] == line.lyricLine)
                            continue;

                        lyricSprite.Fade(line.startTime, line.startTime, 0, 1);
                        lyricSprite.Fade(line.endTime, line.endTime, 1, 0);
                    }
                    
                    xPosition += texture.BaseWidth * FontScale;
                }
            }
        }

        public double DegreeToRadians(double angle)
        {
            var radians = angle * Math.PI/180;
            return radians;
        }

        public Vector2 CircleRotationPositions(double angle, float radius)
        {
            var posX = radius * Math.Cos(DegreeToRadians(angle)) + 320;
            var posY = radius * Math.Sin(DegreeToRadians(angle)) + 240;

            return new Vector2((float)posX, (float)posY);
        }
    }

    internal class LyricModel
    {
        public double startTime;
        public double endTime;
        public string lyricLine;
    }
}
