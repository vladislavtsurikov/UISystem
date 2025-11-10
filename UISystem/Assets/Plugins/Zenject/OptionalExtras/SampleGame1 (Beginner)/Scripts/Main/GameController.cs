using System;
using ModestTree;
using UnityEngine;

namespace Zenject.Asteroids
{
    public enum GameStates
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    public class GameController : IInitializable, ITickable, IDisposable
    {
        private readonly AsteroidManager _asteroidSpawner;
        private readonly Ship _ship;
        private readonly SignalBus _signalBus;

        public GameController(
            Ship ship, AsteroidManager asteroidSpawner,
            SignalBus signalBus)
        {
            _signalBus = signalBus;
            _asteroidSpawner = asteroidSpawner;
            _ship = ship;
        }

        public float ElapsedTime { get; private set; }

        public GameStates State { get; private set; } = GameStates.WaitingToStart;

        public void Dispose() => _signalBus.Unsubscribe<ShipCrashedSignal>(OnShipCrashed);

        public void Initialize()
        {
            Physics.gravity = Vector3.zero;

            Cursor.visible = false;

            _signalBus.Subscribe<ShipCrashedSignal>(OnShipCrashed);
        }

        public void Tick()
        {
            switch (State)
            {
                case GameStates.WaitingToStart:
                {
                    UpdateStarting();
                    break;
                }
                case GameStates.Playing:
                {
                    UpdatePlaying();
                    break;
                }
                case GameStates.GameOver:
                {
                    UpdateGameOver();
                    break;
                }
                default:
                {
                    Assert.That(false);
                    break;
                }
            }
        }

        private void UpdateGameOver()
        {
            Assert.That(State == GameStates.GameOver);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        private void OnShipCrashed()
        {
            Assert.That(State == GameStates.Playing);
            State = GameStates.GameOver;
            _asteroidSpawner.Stop();
        }

        private void UpdatePlaying()
        {
            Assert.That(State == GameStates.Playing);
            ElapsedTime += Time.deltaTime;
        }

        private void UpdateStarting()
        {
            Assert.That(State == GameStates.WaitingToStart);

            if (Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            Assert.That(State == GameStates.WaitingToStart || State == GameStates.GameOver);

            _ship.Position = Vector3.zero;
            ElapsedTime = 0;
            _asteroidSpawner.Start();
            _ship.ChangeState(ShipStates.Moving);
            State = GameStates.Playing;
        }
    }
}
