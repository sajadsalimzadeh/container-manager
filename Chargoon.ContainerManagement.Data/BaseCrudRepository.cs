using Chargoon.ContainerManagement.Domain.Data.Repositories;
using Dapper.FastCrud;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chargoon.ContainerManagement.Data
{
    public abstract class BaseCrudRepository<TEntity, TPrimaryKey> : BaseRepository, ICrudRepository<TEntity, TPrimaryKey>
    {
        public BaseCrudRepository(IConfiguration configuration) : base(configuration)
        {
        }

        private TEntity SetKey(TEntity obj, TPrimaryKey key)
        {
            var keyProperty = typeof(TEntity).GetProperties().FirstOrDefault(x => x.GetCustomAttribute<KeyAttribute>() != null);
            if (keyProperty == null) throw new Exception($"{typeof(TEntity).Name} Key not exists");
            keyProperty.SetValue(obj, key);
            return obj;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return conn.Find<TEntity>();
        }

        public virtual TEntity Get(TPrimaryKey key)
        {
            var obj = SetKey(Activator.CreateInstance<TEntity>(), key);
            return conn.Get(obj);
        }

        public virtual TEntity Insert(TEntity obj)
        {
            conn.Insert(obj);
            return obj;
        }

        public virtual TEntity Update(TEntity obj)
        {
            conn.Update(obj);
            return obj;
        }

        public virtual TEntity Delete(TEntity obj)
        {
            conn.Delete(obj);
            return obj;
        }

        public virtual TEntity Delete(TPrimaryKey key)
        {
            var obj = Get(key);
            return Delete(obj);
        }
    }
}
