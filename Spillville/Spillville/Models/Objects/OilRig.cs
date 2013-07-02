using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.Models.ParticlesSystem;
using Spillville.Utilities;


namespace Spillville.Models.Objects
{
	sealed class OilRig : BaseModel
	{
		private Model _model;
        public bool _IsBroken {  get; private set; }
        private Vector3 particlesPosition;
        private Vector3 smokeparticlesPosition;

		private bool _musicStarted;

		public OilRig() : base()
		{
			DoesCollision = false;
			ModelScale = 18.0f;
            _IsBroken = true;            
		}

		public new bool IsSelectable
		{
			get { return false; }
		}
		
		public override Model ModelObject
		{
			get { return _model; }
		}

		public void SetPossition(Vector3 pos)
		{
			_modelPosition = pos;
            particlesPosition = new Vector3(this.ModelPosition.X, this.ModelPosition.Y + 120, this.ModelPosition.Z);
            smokeparticlesPosition = new Vector3(this.ModelPosition.X, this.ModelPosition.Y + 140, this.ModelPosition.Z);
		}

		public new void Initialize()
		{
			_model = ModelFactory.Get(typeof(OilRig).Name);
			boneTransforms = ModelDrawer.GetBoneTransforms(_model);
		}

		public override void Update(GameTime gameTime)
		{
			if(!_musicStarted)
			{
				AudioManager.PlayManaged3DSoundEffect("Fire",_modelPosition,true);
				_musicStarted = true;
			}


            if (_IsBroken)
            {
                for (int i = 0; i < 2; i++)
                {
                    ParticlesSystemsCollection.ParticlesSystems[4].AddParticle(this.particlesPosition, Vector3.Zero);
                    ParticlesSystemsCollection.ParticlesSystems[3].AddParticle(this.smokeparticlesPosition, Vector3.Zero);
                }
            }
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
		}

	}
}
