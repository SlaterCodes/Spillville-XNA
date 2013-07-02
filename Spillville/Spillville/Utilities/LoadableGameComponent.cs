using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Spillville.Utilities
{
    public interface LoadableGameComponent : IGameComponent
    {
        // Called when the Component has finished loading the class (sh
        void LoadingDone(object sender,EventArgs args);
        // Should reflect whether or not Component has loaded
        bool IsLoaded { get; }
        // Should disable the game Component (disables Update and Draw)
        void DisableComponent();
        // Should enable the game Component (enables Update and Draw)
        void EnableComponent();
        // Event that the class must fire when it has finished loading
        event EventHandler DoneLoading;
    }
}
