using System;
using System.Collections.Generic;

namespace Chargoon.ContainerManagement.Domain.Data.Repositories
{
    public interface ICrudRepository<TEntity, TPrimaryKey>
    {
        IEnumerable<TEntity> GetAll();
        TEntity Get(TPrimaryKey key);
        TEntity Insert(TEntity obj);
        TEntity Update(TEntity obj);
        TEntity Delete(TEntity obj);
        TEntity Delete(TPrimaryKey key);
    }
}
