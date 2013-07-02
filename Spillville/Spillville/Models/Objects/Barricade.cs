using Spillville.MainGame;
using Spillville.Utilities;
using Spillville.MainGame.World;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.Models.Objects
{
	public class Barricade : INotifyOnRegister, IObjectWithMenu, InputSelectable
	{
		public GridTile Location { get; private set; }

		private readonly SimpleModel[] _children = new SimpleModel[8];

		public Barricade(GridTile location)
		{
			Location = location;

		}

		public void Initialize()
		{
			ModelScale = 15.0f;
			ModelObject = ModelFactory.Get(typeof(Barricade).Name);
			boneTransforms = ModelDrawer.GetBoneTransforms(ModelObject);
			

			_modelPosition = new Vector3(Location.CenterPoint.X, 0, Location.CenterPoint.Y);

			UpdateBoundingBox();
			IsBoundingBoxUpToDate = true;
		}

	

		public Model ModelObject { get; private set; }
        public Vector3 ModelRotation { get; private set; }

		private Vector3 _modelPosition;
		public Vector3 ModelPosition { get { return _modelPosition; } }

		public float ModelScale { get; private set; }

		public Vector3 EmissiveColor { get; set; }

		public Matrix[] boneTransforms { get; private set; }
		public BoundingBox boundingBox { get; private set; }


		public void Update(GameTime gameTime)
		{
			_modelPosition.Y += WaterShader.GetWaveHeight(_modelPosition.Z);
		}

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{

		}

		public bool DoesCollision
		{
			get { return true; }
		}

		public bool IsBoundingBoxUpToDate { get; private set; }

		public void UpdateBoundingBox()
		{
			boundingBox = ModelDrawer.GetBoundingBoxUsingFindBoundary(this);
			//boundingBox = new BoundingBox(boundingBox.Min,boundingBox.Max);
			IsBoundingBoxUpToDate = true;
		}

		public void Registered()
		{
			//Todo: Thread this

			bool ne = true, nw = true, se = true, sw = true;

			if (Location.N.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(0, 0));
				ne = false;
				nw = false;
			}
			if (Location.W.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(1, MathHelper.ToRadians(90)));
				nw = false;
				sw = false;
			}
			if (Location.S.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(2, MathHelper.ToRadians(180)));
				se = false;
				sw = false;
			}
			if (Location.E.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(3, MathHelper.ToRadians(-90)));
				ne = false;
				se = false;
			}

			if (ne && Location.NE.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(4, MathHelper.ToRadians(-45)));
			}
			if (se && Location.SE.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(5, MathHelper.ToRadians(-135)));
			}
			if (sw && Location.SW.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(6, MathHelper.ToRadians(135)));
			}
			if (nw && Location.NW.HasBarricade)
			{
				GameStatus.RegisterDrawableUnit(CreateArm(7, MathHelper.ToRadians(45)));
			}
		}

		public SimpleModel CreateArm(int index, float rotation)
		{
			var sm = SimpleModel.Instance();
			sm.SetModel(ModelFactory.Get("Barricade.Arm"));
			sm.SetScale(22.0f);
			sm.SetPostion(Location.CenterPoint);
			sm.SetRotation(new Vector3(0,rotation,0));
            sm.InWater = true;

			_children[index] = sm;

			return sm;
		}

		public void UnRegistered()
		{
			for (var i = 0; i < 7; i++)
			{
				if (_children[i] != null)
				{
					GameStatus.UnRegisterDrawableUnit(_children[i]);
					_children[i].Dispose();
					_children[i] = null;
				}
			}


		}



		public MenuItem GetUpperMenu()
		{
			return new MenuItem
			       	{
			       		Enabled = true,
			       		Selectable = true,
			       		Name = @"Remove Barrier",
			       		Image = VisualHUD.IconDictionary[@"Remove"]
			       	};
		}

		public MenuItem GetRightMenu()
		{
			return null;
		}

		public MenuItem GetLowerMenu()
		{
			return null;
		}

		public MenuItem GetLeftMenu()
		{
			return null;
		}

		public void HandleSelection(MenuSelection selection, GridTile placementLocation)
		{
			if(selection.Equals(MenuSelection.UpperMenu))
			{
				Location.DestroyBarricade();
			}
		}

		public void InputSelected()
		{
			
		}

		public void InputHandleActionX(Vector3 target)
		{
		}

		public void InputDeselected()
		{
		}

	}
}
