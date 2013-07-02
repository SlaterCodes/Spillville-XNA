using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Spillville.Utilities
{
    public interface Loadable : IGameComponent
    {
        bool DoneLoading { get; }
        
        void DisableComponent();
        void EnableComponent();

    }
}
