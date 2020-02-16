using System;

namespace Ledom.Workshops.FooBakeryWebAPI.Entities
{
    public abstract class EntityDTO<TEntity> where TEntity : Entity
    {
        public TEntity ToEntity(object _idValue = null)
        {
            TEntity temp = ToEntity();
            temp._id = _idValue;
            return temp;
        }
        protected abstract TEntity ToEntity();

        public EntityDTO()
        {

        }

        public EntityDTO(TEntity source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!FromEntity(source))
                throw new ArgumentException(nameof(source));
        }

        protected abstract bool FromEntity(TEntity source);
    }
}
