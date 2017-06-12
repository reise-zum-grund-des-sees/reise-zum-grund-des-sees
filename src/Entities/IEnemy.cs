using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    interface IEnemy : IReadonlyEnemy, IUpdateable, IRenderable, IHitable, ICollisionObject
    {

    }

    interface IReadonlyEnemy : IReadonlyPositionObject, ICollisionObject
    {

    }
}
