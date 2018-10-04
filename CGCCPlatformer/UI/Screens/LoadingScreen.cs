using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CGCCPlatformer.Helpers;
using CGCCPlatformer.Helpers.Graphics;
using CGCCPlatformer.UI.Common;
using CGCCPlatformer.UI.Common.Buttons;
using CGCCPlatformer.UI.DrawableText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TextBox = CGCCPlatformer.UI.Common.TextBox;
using ProgressBar = CGCCPlatformer.UI.Common.Bar.ProgressBar;

namespace CGCCPlatformer.UI.Screens
{
    public class LoadingScreen : InteractiveWindow
    {
        public string[] Message { get; }
        public Utils.EventHandler CancelHandler { get; }

        private readonly TextBox messageBox;
        private readonly ProgressBar progressBar;
        private readonly TextBox percentBox;
        private readonly TextBox timeLeftBox;
        private readonly TextBox currentTaskBox;
        private readonly TextButton cancelButton;

        private float progress;
        private readonly Stopwatch delayTimer;
        private readonly Stopwatch totalTimer;
        private readonly Queue<float> timeLeftSamples = new Queue<float>();

        public sealed override Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                messageBox.Bounds = new Rectangle(value.X, value.Y, value.Width, value.Height / 2);
                var progressWidth = (int) (value.Width * 0.5f);
                progressBar.Bounds = new Rectangle(
                    value.Center.X - progressWidth / 2, messageBox.Bounds.Bottom + 5,
                    progressWidth, 20);
                percentBox.Bounds = new Rectangle(progressBar.Bounds.X, progressBar.Bounds.Bottom + 5,
                    progressWidth, value.Bottom - (progressBar.Bounds.Bottom + 5));
                timeLeftBox.Bounds = percentBox.Bounds;
                currentTaskBox.Bounds = new Rectangle(value.X, percentBox.FilledBound.Bottom,
                    value.Width, value.Bottom - percentBox.FilledBound.Bottom);

                if (cancelButton != null)
                {
                    var cancelSize = cancelButton.Font.MeasureString("Cancel").ToPoint() + new Point(8, 0);
                    cancelButton.Bounds = new Rectangle(value.Center.X - cancelSize.X / 2,
                        currentTaskBox.FilledBound.Bottom + 5,
                        cancelSize.X, cancelSize.Y);
                }

                SetProgress(progress);
            }
        }

        public LoadingScreen(Action cancelHandler, Thread thread, SpriteFont font, params string[] message)
        {
            Message = message;
            messageBox = new TextBox(bounds, TextBox.XAlignType.Center, TextBox.YAlignType.Bottom);
            foreach (var msg in message)
                messageBox.AddString(msg);
            messageBox.LoadContent(font);

            progressBar = new ProgressBar(bounds) {Filled = 0};
            percentBox = new TextBox(bounds, TextBox.XAlignType.Left);
            percentBox.AddString("0%");
            percentBox.LoadContent(font);
            timeLeftBox = new TextBox(bounds, TextBox.XAlignType.Right);
            timeLeftBox.LoadContent(font);
            currentTaskBox = new TextBox(bounds, TextBox.XAlignType.Center);
            currentTaskBox.LoadContent(font);
            currentTaskBox.AddString("Loading...");
            if (cancelHandler != null)
            {
                CancelHandler = () =>
                {
                    Logging.WriteLine("Canceling Loading");
                    thread.Abort();
                    cancelHandler.Invoke();
                };
                cancelButton = new TextButton(new ColorText("Cancel", Color.Red, Color.Pink))
                {
                    BorderColor = Color.Red,
                    BorderHoverColor = Color.Pink,
                    Font = font
                };
                cancelButton.Click += CancelHandler;
            }

            delayTimer = new Stopwatch();
            totalTimer = new Stopwatch();

            Bounds = bounds;
            delayTimer.Stop();
        }

        public void SetTaskMessage(string currentTask, bool writeLog = true)
        {
            if (writeLog)
                Logging.WriteLine(currentTask, 2);
            currentTaskBox.ThreadSafeSetLine(0, new PlainText(currentTask));
        }

        public void SetProgress(float portion)
        {
            delayTimer.Restart();
            progress = portion;
            progressBar.Filled = (int) (progressBar.Bounds.Width * portion);
            SetProgressDisplay(Gfx.CodingClubPurple);
            //if (portion > 0.98f)
            //    Thread.Sleep(100);
        }

        private void SetProgressDisplay(Color color)
        {
            percentBox.ThreadSafeSetLine(0, new ColorText(NiceNumbers.PercentString(progress, 3), color));

            float range = 1E10f, avg = 0;
            var samples = timeLeftSamples.ToArray();
            if (samples.Length > 10)
            {
                range = samples.Max() - samples.Min();
                avg = samples.Average();
            }

            if (range - 2.5f < avg * 0.2f || range < 2)
            {
                var text = "~ " + NiceNumbers.TimeStringMultiUnit(avg,
                               range > 30 || avg > 60 * 10 ? 1 : 2);
                //if (range > 5)
                //    text += " +/-" + NiceNumbers.TimeStringMultiUnit(range);
                var colorText = new ColorText(text, color);
                timeLeftBox.ThreadSafeSetLine(0, colorText);
            }
            else
            {
                timeLeftBox.ThreadSafeSetLine(0, new PlainText(""));
            }
        }

        public override void Update(GameTime gameTime, Input input)
        {
            //messageBox.Update(gameTime);
            //progressBar.Update(gameTime);
            //percentBox.Update(gameTime);
            //currentTaskBox.Update(gameTime);

            if (cancelButton == null)
                return;
            cancelButton.Update(gameTime, input);
        }

        public override void Draw(GameTime gameTime, Point mousePos)
        {
            if (!delayTimer.IsRunning)
                delayTimer.Start();
            if (!totalTimer.IsRunning)
                totalTimer.Start();

            var speed = progress / totalTimer.ElapsedMilliseconds;
            var msLeftEstimate = (1 - progress) / speed;
            if (speed > 0 && totalTimer.ElapsedMilliseconds > 500)
                timeLeftSamples.Enqueue(msLeftEstimate / 1000);
            while (timeLeftSamples.Count > 60 * 2.5f)
                timeLeftSamples.Dequeue();

            progressBar.FillColor = Color.RoyalBlue.Interpolate(Color.Red, delayTimer.ElapsedMilliseconds / 15000f);
            if (delayTimer.ElapsedMilliseconds > 600)
            {
                //Debug.WriteLine(delayTimer.ElapsedMilliseconds);
                SetProgressDisplay(Color.White.Interpolate(Color.Red, delayTimer.ElapsedMilliseconds / 10000f));
            }

            messageBox.Draw(gameTime, mousePos);
            progressBar.Draw(gameTime, mousePos);
            percentBox.Draw(gameTime, mousePos);
            timeLeftBox.Draw(gameTime, mousePos);
            currentTaskBox.Draw(gameTime, mousePos);
            cancelButton?.Draw(gameTime, mousePos);
        }
    }
}
