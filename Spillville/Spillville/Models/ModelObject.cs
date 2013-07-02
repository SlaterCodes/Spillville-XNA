using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spillville.StateManager.ManagedScreens;
using Spillville.Utilities;
using Spillville.MainGame;
using Spillville.MainGame.World;

namespace Spillville
{
	internal enum ActionType
	{
		ModelAction,
		LocationAction,
		None
	};
    
    internal enum Status 
    { 
        Idle, 
        Relocating, 
        CleaningUpOil,
        SavingAnimals
    };
    

	internal abstract class ModelObject
	{
		#region Class Variables


        private TimeSpan _boundCalcSpan;
        private BoundingBox _boundingBox;
        public BoundingBox BoundingBox { get { return _boundingBox; } }

		protected Model Model;

        protected Vector3 _modelPosition;
        public Vector3 ModelPosition 
        {
            get { return _modelPosition; }
            set { _modelPosition = value; }
        }

		public float ModelRotation;
		protected float ModelScale;

		protected Matrix CustomRotationMatrix;

        protected Vector3 _modelDestination;
        public Vector3 ModelDestination
        {
            get { return _modelDestination; }
            protected set { _modelDestination = value; }
        }


		public float ModelSpeed;
        public float ModelMaxSpeed;

		//protected float aspectRatio;
		private Vector3 _previousModelPosition;
		private Matrix[] _boneTransforms;


		public static Dictionary<Guid, ModelObject> ModelObjectsInGame;

		protected ModelObject _actionTarget;
		protected Vector3 _actionLocation;

		protected ActionType _actionType;

		public Color SelectedColor = Color.ForestGreen;

        protected Boolean moving;

		/// <summary>
		/// Highlights the ModelObject, if it is selected.
		/// </summary>
		public bool Selected;

		/// <summary>
		/// Get/Set if the ModelObject can be selected by a cursor ray.
		/// </summary>
		public bool Selectable;

		public bool AutoDrawn = true;

		public Guid Id { get; private set; }

		protected bool LiveObject;
		protected bool Movable;

        public Status status { get; protected set; }

		protected string Icon;

		/// <summary>
		/// waves :D
		/// </summary>
		private float waveTranslationConstant;
		private float moveModelFloat;

		private List<ModelObject> _collidedModels;

        private  static List<Guid> _removeQueue;

		#endregion

		public void PerformActionOnTarget(ModelObject target)
		{
			_actionType = ActionType.ModelAction;
			_actionTarget = target;
		}

		public void PerformActionOnTarget(Vector3 target)
		{
			_actionType = ActionType.LocationAction;
			_actionLocation = target;
		}

        /*
        public virtual void LoadContent(ContentManager m)
        {
            // staticModel = m.Load<Model>(asdfasdf);
        }*/

		public void Initialize(bool liveObject)
		{

			Id = Guid.NewGuid();
			LiveObject = liveObject;

            this.status = Status.Idle;

			// For drawing. boneTransforms should never change
			_boneTransforms = new Matrix[Model.Bones.Count];
			Model.CopyAbsoluteBoneTransformsTo(_boneTransforms);

			if (Icon != null)
			{
				VisualHUD.RegisterIcon(GetType().Name, Icon);
			}

			this._modelDestination.X = ModelPosition.X;
            this._modelDestination.Y = ModelPosition.Y;
            this._modelDestination.Z = ModelPosition.Z;

            moving = false;

			CustomRotationMatrix = Matrix.Identity;

			_collidedModels = new List<ModelObject>();

			//too lazy to pull it from waterShader class, so hardcoded for now
			waveTranslationConstant = (8 + (ModelPosition.Y / 4)) / 32;
            _boundCalcSpan = TimeSpan.Zero;
			if (LiveObject)
			{
				Movable = true;
				Selectable = true;
				if (ModelObjectsInGame == null)
				{
					ModelObjectsInGame = new Dictionary<Guid, ModelObject>();
				}

				ModelObjectsInGame.Add(Id, this);


                _boundingBox = new BoundingBox();
			}
			//please initilize this no matter what .\/.

            _removeQueue = new List<Guid>();
			FindBoundary();                        
		}

		public virtual void Update(GameTime gameTime)
		{

            if (this is InputController || 
                ((_modelDestination.X != ModelPosition.X  || _modelDestination.Z != ModelPosition.Z) &&
                gameTime.TotalGameTime - _boundCalcSpan > TimeSpan.FromMilliseconds(800)))
            {
                FindBoundary();
                _boundCalcSpan = gameTime.TotalGameTime;
            }

			float m = (float)gameTime.ElapsedGameTime.Milliseconds / 1000;
			moveModelFloat += m;
			moveModelFloat = moveModelFloat / 2;

            //ModelPosition.Z +=

            switch (_actionType)
            {
                case ActionType.ModelAction:
                    HandleActionOnModel(gameTime, _actionTarget);
                    break;
                case ActionType.LocationAction:
                    HandleActionAtLocation(gameTime, _actionLocation);
                    break;
                case ActionType.None:
                    //do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

			// Calculate bounding box for this object
			if (LiveObject)
			{
				//check within range because model will never be percisely at destination
                if (Movable && Helper.Distance(ModelPosition, ModelDestination) > 10)
                {
                    Step();
                }
                else
                {
                    if (moving)
                    {
                        moving = false;
                        _modelPosition.X = _modelDestination.X;
                        _modelPosition.Z = _modelDestination.Z;
                        HandleActionAtDestination(gameTime, ModelPosition);
                        FindBoundary();
                    }
                    
                }

				if (!_previousModelPosition.Equals(ModelPosition))
				{
					_previousModelPosition = ModelPosition;
				}

			}
		}

        public virtual void HandleActionAtLocation(GameTime gameTime, Vector3 actionLocation)
        {

        }

        public virtual void HandleActionAtDestination(GameTime gameTime, Vector3 actionLocation)
        {

        }

        public virtual void HandleActionOnModel(GameTime gameTime, ModelObject actionTarget)
        {

        }

        public virtual void HandleIncomingAction(String action)
        {

        }

		public List<ModelObject> GetCollision()
		{
			_collidedModels.Clear();
			foreach (ModelObject modelInGame in ModelObjectsInGame.Values)
			{
				if (BoundingBox.Intersects(modelInGame.BoundingBox) && modelInGame != this)
				{
					_collidedModels.Add(modelInGame);
				}
			}
			return _collidedModels;
		}


		public virtual void Draw(SpriteBatch spriteBatch)
		{
			float rotationArgument = (float)Math.Sin(moveModelFloat * waveTranslationConstant);

			foreach (ModelMesh mesh in Model.Meshes)
			{
                Matrix effectWorldMatrix = _boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(ModelScale) * Matrix.CreateRotationY(ModelRotation) *
										   Matrix.CreateTranslation(ModelPosition);// *Matrix.CreateRotationY(rotationArgument);
				foreach (BasicEffect effect in mesh.Effects)
				{
					if (Selectable && Selected)
					{
						effect.EmissiveColor = SelectedColor.ToVector3();
					}
					else
					{
						effect.EmissiveColor = Vector3.Zero;
					}

					effect.EnableDefaultLighting();
					effect.World = effectWorldMatrix;
					effect.View = Camera.View;
					effect.Projection = Camera.Projection;
					foreach (EffectPass pass in effect.CurrentTechnique.Passes)
					{
						pass.Apply();
					}
				}


				mesh.Draw();
			}
		}

		private void FindBoundary()
		{
            _boundingBox.Min = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(ModelPosition));
            _boundingBox.Max = Vector3.Transform(Vector3.Zero, Matrix.CreateTranslation(ModelPosition));

			Func<VertexElement, bool> elementPredicate =
				ve =>
				ve.VertexElementUsage == VertexElementUsage.Position &&
				ve.VertexElementFormat == VertexElementFormat.Vector3;

			foreach (ModelMesh mesh in Model.Meshes)
			{
				Matrix transform = _boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(ModelScale)* Matrix.CreateRotationY(ModelRotation) * CustomRotationMatrix *
								   Matrix.CreateTranslation(ModelPosition);

				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					VertexDeclaration vd = meshPart.VertexBuffer.VertexDeclaration;
					VertexElement[] elements = vd.GetVertexElements();

					Vector3[] vertexData = new Vector3[meshPart.NumVertices];
					meshPart.VertexBuffer.GetData(
						(meshPart.VertexOffset * vd.VertexStride) + elements.First(elementPredicate).Offset,
						vertexData, 0, vertexData.Length, vd.VertexStride);

					Vector3[] transformedPositions = new Vector3[vertexData.Length];
					Vector3.Transform(vertexData, ref transform, transformedPositions);

                    _boundingBox = BoundingBox.CreateMerged(BoundingBox,
														   BoundingBox.CreateFromPoints(transformedPositions));
				}
			}
            
		}

		public static void DrawVisableGameModelObjects(SpriteBatch spriteBatch)
		{
			foreach (ModelObject modelObject in ModelObjectsInGame.Values)
			{
				if (modelObject.AutoDrawn)
				{
					modelObject.Draw(spriteBatch);
				}
			}
		}

		public static void UpdateAllModels(GameTime gameTime)
		{
            foreach (var rem in _removeQueue)
            {
                if (ModelObjectsInGame.Keys.Contains(rem))
                {
                    ModelObjectsInGame.Remove(rem);
                }
            }

            _removeQueue.Clear();

            foreach (ModelObject modelObject in ModelObjectsInGame.Values)
			{
				modelObject.Update(gameTime);
			}


		}

		private void Step()
		{
            // TODO give proper acceleration and deceleration
            float distance = Vector3.Distance(_modelPosition, _modelDestination);
			float atan =
				(float)Math.Atan2((ModelDestination.Z - ModelPosition.Z), (ModelDestination.X - ModelPosition.X));

            float distcalc = distance / ModelMaxSpeed;

			ModelRotation = (-atan - MathHelper.ToRadians(90.0f));

            _modelPosition.X += distcalc * (float)Math.Cos(atan);
            _modelPosition.Z += distcalc * (float)Math.Sin(atan);
		}

        public void SetDestination(Vector3 v)
        {
            if (Movable)
            {
                this._modelDestination.X = v.X;
                this._modelDestination.Z = v.Z;

                if (Helper.Distance(this.ModelPosition, this.ModelDestination) > 10)
                {
                    moving = true;
                }
            }
        }
        public void SetDestination(float x, float y, float z)
        {
            this.SetDestination(new Vector3(x, y, z));
        }

        public void pathFind(Vector3 start, Vector3 goal)
        {

        }

        protected void RemoveAnimals(Guid animal)
        {
            _removeQueue.Add(animal);
        }
	}
	//end class
}