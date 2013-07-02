using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Spillville.MainGame;
using Spillville.MainGame.World;
using Spillville.Utilities;
using Spillville.Models.Boats;
using Spillville.Models.ParticlesSystem;
using Spillville.MainGame.HUD;
using Spillville.MainGame.Levels;

namespace Spillville.Models.Animals
{
    public abstract class Animal : IDrawableModel
    {
        public float health { get; protected set; }
        public bool Dead { get; protected set; }
        public bool Clean { get; protected set; }
        public TimeSpan CleaningTime { get; protected set; }
        private TimeSpan StartCleanTime;
        private bool forward;
        private float customeRotation;
        public bool healthWarning = false;

        public TimeSpan EntryTime;
        public bool Deployed;

        public BoundingBox boundingBox { get; protected set; }
        public Model ModelObject { get; set; }

        protected Vector3 _modelPosition;
        public Vector3 ModelPosition { get { return _modelPosition; } set { _modelPosition = value; } }
        public Matrix[] boneTransforms { get; private set; }
        public Vector3 EmissiveColor { get; set; }
        public float ModelScale { get; protected set; }
        public Vector3 ModelRotation { get; set; }
        public GridTile CurrentTile { get; protected set; }

        public bool CoveredInOil { get { return true;/*OilSpill.HasOil(this.ModelPosition);*/ } }
        private Texture2D _RedBox;
        protected Texture2D _GreenBox;
        private Vector3 _screenCoords;

        protected string animalType = "Animal";

        protected uint timeCounter;

        public int SinkConstant;

        public bool IsAnimated { get; protected set; }

        public virtual void Initialize(Vector2 position)
        {
            //Model = model;
            //ModelObject = model;
            timeCounter = 0;
            Deployed = false;
            boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
            ModelPosition = new Vector3(position.X, -15, position.Y);
            DoesCollision = true;
            IsBoundingBoxUpToDate = false;
            //ModelSpeed = 0f;
            //ModelMaxSpeed = 10f;
            Dead = false;
            forward = true;
            health = 50f;

            //this.Selectable = false;

            Clean = true;
            StartCleanTime = TimeSpan.Zero;

            CurrentTile = WorldGrid.GridTileAtLocation(position);
            CurrentTile.OccupiedAnimal = this;
            

            _screenCoords = Vector3.Zero;
            if (health > 0)
            {
                _RedBox = HealthBar.CreateBox(50, 10, Color.Red, Color.Black);
                _GreenBox = HealthBar.CreateBox(50, 10, Color.LightGreen, Color.Black);
            }
            else
            {
                _RedBox = HealthBar.CreateBox(1, 1, Color.Red, Color.Black);
                _GreenBox = HealthBar.CreateBox(1, 1, Color.LightGreen, Color.Black);
            }

            //base.Initialize(true);
        }

        public virtual void Update(GameTime gameTime)
        {
            //if (!Dead && !Clean && StartCleanTime != TimeSpan.Zero && gameTime.TotalGameTime - StartCleanTime > CleaningTime)
            //{
            //    Clean = true;
            //}

            //TODO communicate with the reputation field
           // Wiggle();
            //base.Update(gameTime);
            _modelPosition.Y = MathHelper.Clamp(WaterShader.GetWaveHeight(_modelPosition.Z) + _modelPosition.Y, -25, -5);
            if (Dead)
            {
                var particlesPosition = this.ModelPosition;
                particlesPosition.Y += 100;

                for (int i = 0; i < 5; i++)
                    ParticlesSystemsCollection.ParticlesSystems[2].AddParticle(particlesPosition, Vector3.Down);

                SendMeAway();
            }    
        }


        public void DrawHealthBar(SpriteBatch spriteBatch)
        {
            RasterizerState prev = spriteBatch.GraphicsDevice.RasterizerState;
            
            _screenCoords = spriteBatch.GraphicsDevice.Viewport.Project(this.ModelPosition, Camera.Projection, Camera.View, Matrix.Identity);
            _screenCoords.X -= (float)(25);
            _screenCoords.Y -= 30;
            spriteBatch.Begin();
            spriteBatch.Draw(_RedBox,
                new Vector2(_screenCoords.X, _screenCoords.Y),
                Color.White);
            if (health > 0)
            {
                spriteBatch.Draw(_GreenBox,
                    new Vector2(_screenCoords.X, _screenCoords.Y),
                    Color.Wheat);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.RasterizerState = prev;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawHealthBar(spriteBatch);
        }

        public virtual bool DoesCollision { get; protected set; }

        public virtual bool IsBoundingBoxUpToDate { get; protected set; }

    	public virtual void UpdateBoundingBox()
        {
            boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
            IsBoundingBoxUpToDate = true;
        }

    	public bool IsSelectable
    	{
    		get { return false; }
    	}



        public void Wiggle()
        {
            //this.CustomRotationMatrix = Matrix.CreateRotationX(customeRotation);

            if (forward)
            {
                customeRotation = customeRotation + 0.01f;
                if (customeRotation > 1.0f)
                    forward = false;
            }
            else
            {
                customeRotation = customeRotation - 0.01f;
                if (customeRotation < -1.0f)
                    forward = true;
            }

        }

        public void Hurt(float amount)
        {
            health-=amount;
            Clean = false;
            if (!Tutorials.TutorialSaveDolphinHelped)
            {
                Tutorials.TutorialSaveDolphinHelped = true;
                BulletinContainer.CallBulletin("Tutorial.SaveAnimal");
            }
        }

        public void Heal(float amount)
        {
            if(health<50)
                health += amount;

            if (health >= 50)
            {
                health = 50;
                Clean = true;

                var particlesPosition = this.ModelPosition;
                particlesPosition.Y += 100;

                for(int i=0; i<20; i++)
                    ParticlesSystemsCollection.ParticlesSystems[1].AddParticle(particlesPosition, Vector3.Down);

                SendMeAway();
            }
         
            if (health > 1)
                _GreenBox = HealthBar.CreateBox((int)health, 10, Color.LightGreen, Color.Black);
            else
                _GreenBox = HealthBar.CreateBox(1, 10, Color.Red, Color.Black);

         
        }

        public void SendMeAway()
        {
            if (Dead)
            {
                BulletinContainer.CallBulletin("AnimalBad");
                VisualHUD.DisplayMessage(string.Format("A {0} DIED", this.animalType));
                GameStatus.DecreasePopulatity(20);
            }
            else
            {
                BulletinContainer.CallBulletin("AnimalGood");
                VisualHUD.DisplayMessage(string.Format("You saved a {0}",this.animalType));
                GameStatus.IncreasePopularity(10);
            }
            this.CurrentTile.OccupiedAnimal = null;
            GameStatus.RemoveAnimal(this);
        }
    }
}
