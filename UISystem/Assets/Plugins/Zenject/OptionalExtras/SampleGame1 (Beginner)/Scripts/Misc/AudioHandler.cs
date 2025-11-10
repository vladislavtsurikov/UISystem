using System;
using UnityEngine;

namespace Zenject.Asteroids
{
    public class AudioHandler : IInitializable, IDisposable
    {
        private readonly AudioSource _audioSource;
        private readonly Settings _settings;
        private readonly SignalBus _signalBus;

        public AudioHandler(
            AudioSource audioSource,
            Settings settings,
            SignalBus signalBus)
        {
            _signalBus = signalBus;
            _settings = settings;
            _audioSource = audioSource;
        }

        public void Dispose() => _signalBus.Unsubscribe<ShipCrashedSignal>(OnShipCrashed);

        public void Initialize() => _signalBus.Subscribe<ShipCrashedSignal>(OnShipCrashed);

        private void OnShipCrashed() => _audioSource.PlayOneShot(_settings.CrashSound);

        [Serializable]
        public class Settings
        {
            public AudioClip CrashSound;
        }
    }
}
