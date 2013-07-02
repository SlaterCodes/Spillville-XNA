using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

using SpillvilleDataTypes;

namespace Spillville.Utilities
{
    class AudioManager
    {
        private static bool _loaded;
        public static XBoxFriendlyDictionary<string, SoundEffect> SoundEffects { get; protected set; }
        public static XBoxFriendlyDictionary<string, Song> Songs { get; protected set; }

        private static XBoxFriendlyDictionary<string, SoundEffectInstance> _managedSoundEffects;
        private static XBoxFriendlyDictionary<string, SoundEffectInstance3D> _managed3DSoundEffects;
        
        public static void Initialize()
        {
            if (!_loaded)
            {
                SoundEffects = new XBoxFriendlyDictionary<string, SoundEffect>();
                Songs = new XBoxFriendlyDictionary<string, Song>();

                _managedSoundEffects = new XBoxFriendlyDictionary<string, SoundEffectInstance>();
                _managed3DSoundEffects = new XBoxFriendlyDictionary<string, SoundEffectInstance3D>();
            }
        }

        public static void LoadContent(ContentManager content)
        {
            if (!_loaded)
            {
                // Songs removed because Copyrighted and could not be included
                //Songs.Add("MainMenuMusic", content.Load<Song>(@"Audio\Songs\Mainmenu"));
                //Songs.Add("Gameplay1", content.Load<Song>(@"Audio\Songs\Gameplay1"));
				SoundEffects.Add("Dolphin", content.Load<SoundEffect>(@"Audio\Effects\Dolphin"));
				SoundEffects.Add("Fire", content.Load<SoundEffect>(@"Audio\Effects\Fire"));
				SoundEffects.Add("DinghySound", content.Load<SoundEffect>(@"Audio\Effects\Motor1"));
                SoundEffects.Add("TimeWarning", content.Load<SoundEffect>(@"Audio\Effects\TimeWarning"));
                SoundEffects.Add("LastTenSeconds", content.Load<SoundEffect>(@"Audio\Effects\LastTenSeconds"));
                SoundEffects.Add("birdScreams", content.Load<SoundEffect>(@"Audio\Effects\birdScreams"));                

                _loaded = true;
            }
        }

        public static void PlaySong(string songName)
        {

            if (Songs.ContainsKey(songName))
            {
                var song = Songs[songName];
                MediaPlayer.Play(song);
            }
        }

        public static void Update(GameTime gameTime)
        {
            for (var i = 0; i < _managed3DSoundEffects.Count; i++)
            {
                var soundEffect = _managed3DSoundEffects[_managed3DSoundEffects.GetKey(i)];
                if(soundEffect.SoundInstance.State == SoundState.Playing)
                    soundEffect.UpdateCamera();
            }
        }

        public static void StopManagedSound(string effectName)
        {
            if (_managedSoundEffects.ContainsKey(effectName))
            {
                var effect = _managedSoundEffects[effectName];
                effect.Stop();
            }
            if (_managed3DSoundEffects.ContainsKey(effectName))
            {
                var effect = _managed3DSoundEffects[effectName];
                effect.SoundInstance.Stop();
            }
        }

        public static void PlaySoundOnce(string effectName)
        {
            if (_managedSoundEffects.ContainsKey(effectName))
            {
                var soundEffect = _managedSoundEffects[effectName];
                if (soundEffect.State == SoundState.Stopped)
                {                    
                    soundEffect.Play();
                }
            }
        }

        public static void PlayManagedSoundEffect(string effectName,bool loopSound)
        {
            if (_managedSoundEffects.ContainsKey(effectName))
            {
                var soundEffect = _managedSoundEffects[effectName];
                if (soundEffect.State == SoundState.Stopped)
                {
                    soundEffect.IsLooped = loopSound;
                    soundEffect.Play();
                }
            }
            else
            {
                var newEffect = GetSoundEffect(effectName,loopSound);
                if (newEffect != null)
                {
                    _managedSoundEffects.Add(effectName, newEffect);
                    newEffect.Play();
                }    
            }
        }

        public static void PlayManaged3DSoundEffect(string effectName,  Vector3 position,bool loopSound)
        {
            if (_managed3DSoundEffects.ContainsKey(effectName))
            {
                var soundEffect = _managed3DSoundEffects[effectName];
                if (soundEffect.SoundInstance.State == SoundState.Stopped)
                {
                    soundEffect.UpdateModelPosition( position);
                    soundEffect.SoundInstance.Play();
                }
            }
            else
            {
                var newEffect = Get3DSoundEffect(effectName, position,loopSound);
                if (newEffect != null)
                {
                    _managed3DSoundEffects.Add(effectName, newEffect);
                    newEffect.SoundInstance.Play();
                }
            }
        }

        public static SoundEffectInstance GetSoundEffect(string effectName,bool loopSound)
        {
            try
            {
                var effect = SoundEffects[effectName];
                var effectInstance = effect.CreateInstance();
                effectInstance.IsLooped = loopSound;
                return effectInstance;
            }
            catch(ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public static SoundEffectInstance3D Get3DSoundEffect(string effectName, Vector3 modelPosition,bool loopSound)
        {
            try
            {
                var effect = SoundEffects[effectName];
                var effectInstance = effect.CreateInstance();
                effectInstance.IsLooped = loopSound;
                var effect3dInstance = SoundEffectInstance3D.Instance(effectInstance);
                effect3dInstance.UpdateModelPosition( modelPosition);
                effect3dInstance.UpdateCamera();
                return effect3dInstance;
            }
            catch(ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public static void Reset()
        {
            if (_loaded)
            {
                for (var i = 0; i < _managedSoundEffects.Count; i++)
                {
                    var soundEffect = _managedSoundEffects[_managedSoundEffects.GetKey(i)];
                    soundEffect.Dispose();
                }

                for (var i = 0; i < _managed3DSoundEffects.Count; i++)
                {
                    var soundEffect = _managed3DSoundEffects[_managed3DSoundEffects.GetKey(i)];
                    soundEffect.Dispose();
                }

                _managedSoundEffects.Clear();
                _managed3DSoundEffects.Clear();
            }
        }

        public static void Unload()
        {
            Reset();
            MediaPlayer.Stop();
            SoundEffects.Clear();
            Songs.Clear();
            SoundEffectInstance3D.Unload();
        }
        /*
        public static void playdolphin(Vector3 pos)
        {
            var emitter = new AudioEmitter();
            var listener = new AudioListener();
            var effect = SoundEffects[@"Dolphin"];
            var effectInstance = effect.CreateInstance();
            emitter.DopplerScale = .00001f;

            pos.Normalize();
            var newVect = Camera.Target;
            newVect.Normalize();
            
            listener.Position = newVect;
            emitter.Position = pos;
            
            effectInstance.Apply3D(listener, emitter);
            effectInstance.Play();

            
        }*/
    }

    class SoundEffectInstance3D : IDisposable
    {
        private static Queue<SoundEffectInstance3D> _instances; 

        public AudioEmitter SoundEmitter { get; protected set; }
        public AudioListener SoundListener { get; protected set; }
        
        private SoundEffectInstance _soundInstance;
        public SoundEffectInstance SoundInstance
        {
            get
            {
                return _soundInstance;
            }
            set
            {
                _soundInstance = value;
                _soundInstance.Apply3D(SoundListener, SoundEmitter);
            }
        }

        private SoundEffectInstance3D() 
        {
            SoundEmitter = new AudioEmitter();
            SoundListener = new AudioListener();
            SoundEmitter.DopplerScale = 10f;
        }

        public static SoundEffectInstance3D Instance(SoundEffectInstance soundEffectInstance)
        {
            if (_instances == null)
                _instances = new Queue<SoundEffectInstance3D>();
            var theInstance =  _instances.Count == 0 ? new SoundEffectInstance3D() : _instances.Dequeue();
            theInstance.SoundInstance = soundEffectInstance;
        	theInstance.SoundInstance.Volume = 0.6f;
            return theInstance;
        }

        public void UpdateModelPosition( Vector3 position)
        {
            SoundEmitter.Position = Vector3.Multiply(position,.001f);
            SoundInstance.Apply3D(SoundListener, SoundEmitter);
        }

        public void UpdateCamera()
        {
            
            SoundListener.Position = Vector3.Multiply(Camera.Position,.001f);
            SoundInstance.Apply3D(SoundListener, SoundEmitter);
        }

        public void Dispose()
        {
            SoundInstance.Stop();
            SoundInstance.Dispose();

            // Reset any changes to SoundEmitter and SoundListener
            SoundEmitter.DopplerScale = 10f;
            SoundEmitter.Forward = Vector3.Negate(Vector3.UnitZ);
            SoundEmitter.Position = Vector3.Zero;
            SoundEmitter.Up = Vector3.Up;
            SoundEmitter.Velocity = Vector3.Zero;

            SoundListener.Forward = Vector3.Negate(Vector3.UnitZ);
            SoundListener.Position = Vector3.Zero;
            SoundListener.Up = Vector3.Up;
            SoundListener.Velocity = Vector3.Zero;

            _instances.Enqueue(this);
        }

        public static void Unload()
        {
			if(_instances!=null)
            _instances.Clear();
        }
    }
}
