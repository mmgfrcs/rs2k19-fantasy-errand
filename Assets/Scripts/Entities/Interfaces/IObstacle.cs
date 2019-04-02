using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyErrand.Entities.Interfaces
{
    public interface IObstacle
    {
        bool IsHurdling { get; }
        float SpawnRate { get; }
    }
}
