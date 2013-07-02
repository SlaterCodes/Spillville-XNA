
using System.Collections.Generic;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spillville.MainGame.OilSpillContainer
{
    public static class OilSpillManager
    {

        private static List<OilSpill> _oilSpills = new List<OilSpill>();
        private static OilSpillRender OilRenderer = new OilSpillRender();

        public static void Initialize(GraphicsDevice gd)
        {
            OilRenderer.Initialize(gd);
        }

        public static void CreateOilSpill(Vector2 position,int size)
        {
            var to = new OilSpill();
            _oilSpills.Add(to);
            to.Initialize(position, size);
        }

        public static void RegisterOilSpill(OilSpill spill)
        {
            _oilSpills.Add(spill);
        }


        public static List<OilSpill> OilSpills()
        {
            return _oilSpills;
        }

        public static void Update(GameTime gt)
        {
            for (int i = 0; i < _oilSpills.Count; i++)
            {
                if (_oilSpills[i].Initialized)
                    _oilSpills[i].Update(gt);
            }
        }

        public static void Draw(GameTime gt)
        {
            for (int i = 0; i < _oilSpills.Count; i++)
            {
                if(_oilSpills[i].Initialized)
                    OilRenderer.Draw(_oilSpills[i], gt);
            }
        }

        

        public static void ClearSpills()
        {
            _oilSpills.Clear();
        }

    }
}
