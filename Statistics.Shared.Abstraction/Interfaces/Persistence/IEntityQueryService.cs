namespace Statistics.Shared.Abstraction.Interfaces.Persistence;

public interface IEntityQueryService<TEntity, TSearchable> where TEntity : class, IEntity
    where TSearchable : class, ISearchable
{
    /// <summary>
    ///     Add one <typeparamref name="TEntity" /> to the database.
    /// </summary>
    /// <param name="entity">The data for a(n) <typeparamref name="TEntity" /> to be added.</param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>The newly added <typeparamref name="TEntity" />.</returns>
    Task AddEntity(TEntity entity, bool saveImmediately = true);

    /// <summary>
    ///     Add multiple <typeparamref name="TEntity" /> to the database.
    /// </summary>
    /// <param name="entities">The data for a(n) <typeparamref name="TEntity" /> to be added.</param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>All the newly added <typeparamref name="TEntity" />.</returns>
    Task AddEntities(IEnumerable<TEntity> entities, bool saveImmediately = true);

    /// <summary>
    ///     Get the first <typeparamref name="TEntity" /> matching properties defined in supplied
    ///     <typeparamref name="TSearchable"></typeparamref>.
    /// </summary>
    /// <param name="searchable">The <typeparamref name="TSearchable"></typeparamref> with query arguments set</param>
    /// <returns>
    ///     Returns first entry that matches properties defined in supplied
    ///     <param name="searchable"></param>
    /// </returns>
    Task<TEntity> GetEntity(TSearchable searchable);

    /// <summary>
    ///     Get all <typeparamref name="TEntity" /> matching properties defined in supplied
    ///     <typeparamref name="TSearchable"></typeparamref>.
    /// </summary>
    /// <param name="searchable">The <typeparamref name="TSearchable"></typeparamref> with query arguments set.</param>
    /// <returns>
    ///     Returns all entries matching properties defined in supplied
    ///     <param name="searchable"></param>
    ///     .
    /// </returns>
    Task<IEnumerable<TEntity>> GetEntities(TSearchable searchable);

    /// <summary>
    ///     Update a single <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="entity">The <typeparamref name="TEntity" /> to be updated.</param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>The updated <typeparamref name="TEntity" />.</returns>
    Task UpdateEntity(TEntity entity, bool saveImmediately = true);

    /// <summary>
    ///     Update multiple <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="entities">All the <typeparamref name="TEntity" /> to be updated.</param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>All the updated <typeparamref name="TEntity" />.</returns>
    Task UpdateEntities(IEnumerable<TEntity> entities, bool saveImmediately = true);

    /// <summary>
    ///     Delete the first <typeparamref name="TEntity" /> matching the supplied <typeparamref name="TSearchable" />.
    /// </summary>
    /// <param name="searchable">
    ///     The <typeparamref name="TSearchable"></typeparamref> with query arguments set to find what to
    ///     delete
    /// </param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>Nothing.</returns>
    Task DeleteEntity(TSearchable searchable, bool saveImmediately = true);

    /// <summary>
    ///     Delete the <typeparamref name="TEntity" /> with supplied <paramref name="id" />.
    /// </summary>
    /// <param name="id">Id of the <typeparamref name="TEntity" /> to delete.</param>
    /// <param name="saveImmediately">Determines if method will immediately call SaveChangesAsync(), or not.</param>
    /// <returns>Nothing.</returns>
    Task DeleteEntityById(int id, bool saveImmediately = true);
}
