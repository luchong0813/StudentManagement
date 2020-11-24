using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StudentManagement.Infrastructure.Repositories
{
    /// <summary>
    /// 此接口是所有仓储的约定, 此接口仅作为约定，用于标识它们。
    /// </summary>
    /// <typeparam name="TEntity">当前传入仓储的实体类型</typeparam>
    /// <typeparam name="TprimaryKey">传入仓储的主键类型</typeparam>
    public interface IRepository<TEntity, TprimaryKey> where TEntity : class
    {
        #region 查询
        /// <summary>
        /// 获取用于整个表中检索实体的IQueryable
        /// </summary>
        /// <returns>可用于从数据库中选择实体</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 用于获取所有实体
        /// </summary>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList();

        /// <summary>
        /// 用于获取所有实体的异步实现
        /// </summary>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync();

        /// <summary>
        /// 获取传入本方法的所有实体
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取传入本方法的所有实体的异步实现
        /// </summary>
        /// <param name="predicate">筛选实体的条件</param>
        /// <returns>所有实体列表</returns>
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件获取实体信息
        /// 如果查询不到返回值，则会引发异常
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入的筛选条件获取实体信息的异步实现
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入数据获取实体，如果没有找到则返回Null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 通过传入数据获取实体的异步实现
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 添加
        /// <summary>
        /// 添加一个新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 添加一个新实体的异步实现
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);
        #endregion

        #region 更新
        /// <summary>
        /// 更细实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更细实体的异步实现
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        #endregion

        #region 删除
        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #region 总和计算
        /// <summary>
        /// 获取此仓储中所有实体的总和
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// 获取此仓储中所有实体的总和的异步实现
        /// </summary>
        /// <returns></returns>
        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        Task<long> LongCountAsync();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
