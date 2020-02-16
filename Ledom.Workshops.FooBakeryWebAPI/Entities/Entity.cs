using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ledom.Workshops.FooBakeryWebAPI.Entities
{
    public abstract class Entity
    {
        public object _id { get; set; } = null;
    }
}
