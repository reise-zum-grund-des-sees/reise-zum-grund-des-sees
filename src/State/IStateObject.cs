using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReiseZumGrundDesSees
{
    interface IStateObject
    {
        ConfigFile.ConfigNode GetState(ObjectIDMapper _mapper);
    }
}
