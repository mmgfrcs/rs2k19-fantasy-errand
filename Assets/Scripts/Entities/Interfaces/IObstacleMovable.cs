using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyErrand.Entities.Interfaces
{
    public interface IObstacleMovable : IObstacle
    {
        float MoveSpeed { get; }

        /// <summary>
        /// Moves this <see cref="IObstacleMovable"/> object. Called per Update cycle.
        /// </summary>
        void DoMove();
    }
}
