using UnityEngine;

namespace FantasyErrand.Entities.Interfaces
{
    public interface IObstacleRotatable : IObstacle
    {
        Vector3 RotateSpeed { get; }

        /// <summary>
        /// Rotates this <see cref="IObstacleRotatable"/> object. Called per Update cycle.
        /// </summary>
        void DoRotate();
    }
}
