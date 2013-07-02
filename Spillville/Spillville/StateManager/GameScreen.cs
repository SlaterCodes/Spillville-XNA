using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Utilities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Spillville.StateManager
{
    public enum ScreenTransitionState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameScreen : DrawableGameComponent, LoadableGameComponent
    {
        #region Properties


        /// <summary>
        /// Normally when one screen is brought up over the top of another,
        /// the first screen will transition off to make room for the new
        /// one. This property indicates whether the screen is only a small
        /// popup, in which case screens underneath it do not need to bother
        /// transitioning off.
        /// </summary>
        public bool IsPopup
        {
            get { return isPopup; }
            protected set { isPopup = value; }
        }

        bool isPopup = false;


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get { return transitionOnTime; }
            protected set { transitionOnTime = value; }
        }

        TimeSpan transitionOnTime = TimeSpan.Zero;


        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get { return transitionOffTime; }
            protected set { transitionOffTime = value; }
        }

        TimeSpan transitionOffTime = TimeSpan.Zero;


        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get { return transitionPosition; }
            protected set { transitionPosition = value; }
        }

        float transitionPosition = 1;


        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 1 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }


        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenTransitionState ScreenState
        {
            get { return screenState; }
            protected set { screenState = value; }
        }

        ScreenTransitionState screenState = ScreenTransitionState.TransitionOn;


        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        bool isExiting = false;


        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (screenState == ScreenTransitionState.TransitionOn ||
                        screenState == ScreenTransitionState.Active);
            }
        }

        bool otherScreenHasFocus;

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent screens, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer
        {
            get { return controllingPlayer; }
            internal set { controllingPlayer = value; }
        }

        PlayerIndex? controllingPlayer;

        public event EventHandler DoneLoading;
        protected GraphicsDevice graphicsDevice { get; private set; }
        //protected float aspectRatio { get; protected set; }
        protected Vector2 viewportSize { get; private set; }
        protected SpriteBatch ScreenSpriteBatch { get; private set; }
        protected SpriteFont ScreenFont { get; private set; }
        protected SpriteFont TitleScreenFont { get; private set; }
        protected Spillville mainGame { get; private set; }
        protected bool isContentLoaded;
        //protected Texture2D backgroundTexture;

        protected InputState input {get;private set;}
        public bool IsLoaded { get; private set; }

        
        #endregion

        public GameScreen(Spillville game)
            : base(game)
        {
            mainGame = game;
            input = game.Input;
            isContentLoaded = false;
            this.DoneLoading += LoadingDone;
        }


        public override void Initialize() 
        {
            graphicsDevice = Game.GraphicsDevice;            
            viewportSize = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            ScreenSpriteBatch = new SpriteBatch(graphicsDevice);
            base.Initialize();
        }
        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        protected override void LoadContent() 
        {
            ScreenFont = Game.Content.Load<SpriteFont>(@"fonts\MenuFont");
            TitleScreenFont = Game.Content.Load<SpriteFont>(@"fonts\TitleMenuFont");
            base.LoadContent();
            IsLoaded = true;
            DoneLoading(this, EventArgs.Empty);
        }


        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.HandleInput(input);
            //this.otherScreenHasFocus = otherScreenHasFocus;

            if (isExiting)
            {
                // If the screen is going away to die, it should transition off.
                screenState = ScreenTransitionState.TransitionOff;

                TransitionPosition += .01f;

                if (!UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    //ScreenManager.RemoveScreen(this);
                    screenState = ScreenTransitionState.TransitionOff;
                }
            }
            else
            {

                TransitionPosition = MathHelper.Clamp(TransitionPosition - .005f, 0, 1000f);
            }
            /*else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, transitionOffTime, 1))
                {
                    // Still busy transitioning.
                    screenState = ScreenTransitionState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenTransitionState.Hidden;
                }
            }*/
            //else
            //{
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, transitionOnTime, -1))
                {
                    // Still busy transitioning.
                    screenState = ScreenTransitionState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    screenState = ScreenTransitionState.Active;
                }
            //}
                base.Update(gameTime);
        }

        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            transitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (transitionPosition <= 0)) ||
                ((direction > 0) && (transitionPosition >= 1)))
            {
                transitionPosition = MathHelper.Clamp(transitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }


        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputState input) { }

        public override void Draw(GameTime gameTime)
        {

            /*ScreenSpriteBatch.Begin();
            if (backgroundTexture != null)
            {
                ScreenSpriteBatch.Draw(backgroundTexture, graphicsDevice.Viewport.Bounds, Color.White);
            }
            ScreenSpriteBatch.End();
            */
            base.Draw(gameTime);
        }

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                //ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                isExiting = true;
            }
        }

        public void EnableComponent()
        {
            this.Enabled = true;
            this.Visible = true;
            isExiting = false;
        }
        public void DisableComponent()
        {
            this.Enabled = false;
            this.Visible = false;
        }

        public void LoadingDone(object sender,EventArgs args)
        {
            IsLoaded = true;
        }
    }
}
