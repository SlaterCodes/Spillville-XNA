using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.MainGame.World;

namespace Spillville.Models
{
	public class SimpleModel : BaseModel, IDisposable
	{
		private static Queue<SimpleModel> _instances; 

		private Model _model;

		public override Model ModelObject { get { return _model; } }
        public bool InWater;

		private SimpleModel()
        {
            InWater = false;
        }

		public static SimpleModel Instance()
		{
			if(_instances == null)
			{
				_instances = new Queue<SimpleModel>();
			}

			return _instances.Count == 0 ? new SimpleModel() : _instances.Dequeue();
		}

		public void SetModel(Model model)
		{
			_model = model;
			boneTransforms = new Matrix[ModelObject.Bones.Count];
			ModelObject.CopyAbsoluteBoneTransformsTo(boneTransforms);
		}

		public void SetPostion(Vector2 loc)
		{
			_modelPosition = new Vector3(loc.X, 0, loc.Y);
		}

		public void SetPostion(Vector3 loc)
		{
            _modelPosition = loc;
		}

		public void SetRotation(Vector3 rotation)
		{
			ModelRotation = rotation;
		}

		public void SetScale(float scale)
		{
			ModelScale = scale;
		}


		public override void Update(GameTime gameTime)
		{
			//intetionally left blank
            if(InWater)
                _modelPosition.Y += WaterShader.GetWaveHeight(_modelPosition.Z);
		}

		public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			//intetionally left blank
		}

		public void Dispose()
		{
			SetRotation(Vector3.Zero);
			_instances.Enqueue(this);
		}

	}
}
