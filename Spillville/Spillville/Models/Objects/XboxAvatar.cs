

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spillville;
using Spillville.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Spillville.Models.Objects
{
    public class XboxAvatar
    {
        private AvatarAnimationPreset _idleAnimationPreset;
        private AvatarAnimationPreset _playAnimationPreset;
        private AvatarDescription _avatarDescription;
        private AvatarRenderer _avatarRenderer;
        private AvatarAnimation _avatarAnimation;

        private float _rotationYValue;
        public float RotationYValue
        {
            get { return _rotationYValue; }
            set { _rotationYValue = value; ApplyRender(); }
        }

        private Matrix _avatarWorldMatrix;

        private float _avatarScale;
        public float AvatarScale
        {
            get { return _avatarScale; }
            set { _avatarScale = value; ApplyRender(); }
        }

        private Vector3 _avatarPosition;
        public Vector3 AvatarPosition
        {
            get {return _avatarPosition;}
            set { _avatarPosition = value; ApplyRender(); }
        }

        private bool _playAnimationOn;
        private bool _doPlayAnimation;
        public bool RenderEnabled { get; private set; }
        public static bool IsRunningOnXbox { 
            get 
            { 
#if XBOX
                return true;
#else
                return false;
#endif
            }   
        }

        public bool IsPlayingActiveAnimation
        {
            get { return _playAnimationOn; }
        }

        public XboxAvatar() { }

        public void Initialize(Vector3 position)
        {
            _playAnimationOn = false;
            _doPlayAnimation = false;
            _idleAnimationPreset = AvatarAnimationPreset.Stand0;
            _playAnimationPreset = AvatarAnimationPreset.Clap;

            var playAvatar = Spillville.GamerAvatarDescription;

            _avatarDescription = playAvatar==null ? AvatarDescription.CreateRandom() : playAvatar;
            _avatarRenderer = Spillville.GamerAvatarRenderer;
            _avatarAnimation = new AvatarAnimation(_idleAnimationPreset);

            if (_avatarRenderer == null)
                RenderEnabled = false;
            else
                RenderEnabled = true;

            _avatarScale = 30f;
            _rotationYValue = 140f;
            _avatarPosition = position;

            ApplyRender();
        }


        // Will only change after the next play animation is done
        public void SetIdleAnimation(string animation)
        {
            if (RenderEnabled)
            {
                try
                {
                    _idleAnimationPreset = (AvatarAnimationPreset)Enum.Parse(typeof(AvatarAnimationPreset), animation, true);
                }
                catch (Exception exc) { }
            }
        }

        public void PlayAnimation(string animation,bool genderspecific)
        {
            if (RenderEnabled)
            {
                try
                {
                    var finalAnim = (genderspecific? _avatarDescription.BodyType.ToString()+animation :animation);
                    System.Diagnostics.Debug.WriteLine("Playing Avatar Animation: " + finalAnim);
                    _playAnimationPreset = (AvatarAnimationPreset)Enum.Parse(typeof(AvatarAnimationPreset), finalAnim, true);
                }
                catch (Exception exc) { }

                if (!_playAnimationOn && !_doPlayAnimation)
                {
                    _doPlayAnimation = true;
                }
            }
        }



        public void Update(GameTime gameTime)
        {
            if (RenderEnabled)
            {
                if (_doPlayAnimation)
                {
                    _avatarAnimation = new AvatarAnimation(_playAnimationPreset);
                    _playAnimationOn = true;
                    _doPlayAnimation = false;
                }

                if (_playAnimationOn && (_avatarAnimation.Length - _avatarAnimation.CurrentPosition).TotalMilliseconds < 150)
                {
                    _playAnimationOn = false;
                    _avatarAnimation = new AvatarAnimation(_idleAnimationPreset);
                }

                if (_avatarRenderer.State == AvatarRendererState.Ready)
                {
                    _avatarAnimation.Update(gameTime.ElapsedGameTime, true);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (RenderEnabled)
            {
                _avatarRenderer.World = _avatarWorldMatrix;
                _avatarRenderer.Projection = Camera.Projection;
                _avatarRenderer.View = Camera.View;
                _avatarRenderer.Draw(_avatarAnimation.BoneTransforms, _avatarAnimation.Expression);
            }
        }

        private void ApplyRender()
        {
            if (RenderEnabled)
            {
                _avatarWorldMatrix = Matrix.CreateScale(AvatarScale) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(_rotationYValue)) *
                    Matrix.CreateTranslation(AvatarPosition);
            }
        }

    }
}


