using System.Collections.Generic;
using System.Linq;
using Spillville.MainGame;


namespace Spillville.Models
{
	
	public static class ModelCollider
	{
		
		public static IEnumerable<IDrawableModel> GetCollidedModels(IDrawableModel model)
		{
			if(!model.DoesCollision)
			{
				return null;
			}

			UpdateModel(model);
			var modelList = new List<IDrawableModel>();

			foreach (var currentModel in GameStatus.DrawList.Where(currentModel => currentModel != model && currentModel.DoesCollision))
			{
				UpdateModel(currentModel);
				if(HasCollision(currentModel,model))
				{
					modelList.Add(currentModel);
				}
			}

			return modelList;

		}


		private static void UpdateModel(IDrawableModel model)
		{
			if(!model.IsBoundingBoxUpToDate)
			{
				model.UpdateBoundingBox();
			}
		}


		public static bool HasCollision(IDrawableModel model1, IDrawableModel model2)
		{
			return model1.boundingBox.Intersects(model2.boundingBox);
		}


	}
}
