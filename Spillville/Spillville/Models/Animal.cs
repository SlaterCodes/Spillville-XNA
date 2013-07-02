using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Spillville.MainGame;

namespace Spillville.Models
{
    public class Animal : IDrawableModel
    {
        public float health { get; protected set; }
        public bool Dead { get; protected set; }
        public bool Clean { get; protected set; }
        public TimeSpan CleaningTime { get; protected set; }
        private TimeSpan StartCleanTime;
        private bool forward;
        private float customeRotation;

        public BoundingBox boundingBox { get; protected set; }
        public Model ModelObject { get; set; }
        public Vector3 ModelPosition { get; set; }
        public Matrix[] boneTransforms { get; private set; }
        public Vector3 EmissiveColor { get; set; }
        public float ModelScale { get; protected set; }
        public float ModelRotation { get; set; }

        public bool CoveredInOil { get { return true;/*OilSpill.HasOil(this.ModelPosition);*/ } }

        public void Initialize(Model model)
        {
            //Model = model;
            ModelObject = model;
            boneTransforms = ModelDrawer.GetBoneTransforms(model);   
            ModelPosition = new Vector3(-300, 0,-300);
            //ModelSpeed = 0f;
            //ModelMaxSpeed = 10f;
            Dead = false;
            forward = true;
            health = 50f;

            //this.Selectable = false;

            Clean = false;
            StartCleanTime = TimeSpan.Zero;

            //base.Initialize(true);
        }

        public void Update(GameTime gameTime)
        {
            if (!Dead && !Clean && StartCleanTime != TimeSpan.Zero && gameTime.TotalGameTime - StartCleanTime > CleaningTime)
            {
                Clean = true;
            }

            //TODO communicate with the reputation field
           // Wiggle();
            //base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }

		public bool DoesCollision { get; protected set; }

    	public bool IsBoundingBoxUpToDate { get; private set; }

    	public void UpdateBoundingBox()
    	{
    		throw new NotImplementedException();
    	}

    	public bool IsSelectable
    	{
    		get { return false; }
    	}

    	public void StartCleaning(GameTime gt)
        {
            if(!Dead)
                StartCleanTime = gt.TotalGameTime;
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
            health = Math.Max(0, health - amount);
            if (health==0)
            {
                Dead = true;
                GameStatus.DecreasePopulatity(20);
            }
        }

        public void Heal(float amount)
        {
            health = Math.Min(100, health + amount);
        }

        public void SendMeAway()
        {
            //this.RemoveAnimals(this.Id);
        }
    }
}
