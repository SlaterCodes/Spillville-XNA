using Microsoft.Xna.Framework;
using Spillville.Models;

namespace Spillville.Utilities
{
    interface InputSelectable : IDrawableModel
    {

        void InputSelected();

        void InputHandleActionX(Vector3 target);

        void InputDeselected();

    }
}
