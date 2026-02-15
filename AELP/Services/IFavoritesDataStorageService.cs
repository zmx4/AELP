using System;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

/// <summary>
/// 提供收藏数据的增删改查与变更通知能力。
/// </summary>
public interface IFavoritesDataStorageService
{
    /// <summary>
    /// 将指定的 <see cref="Dictionary"/> 项异步添加到收藏并保存更改。
    /// 实现应负责持久化（例如写入文件或数据库）并在适当时保证线程安全。
    /// 成功添加后，建议实现触发 <see cref="OnFavoritesChanged"/> 事件以通知订阅者。
    /// </summary>
    /// <param name="favorite">要添加到收藏的 <see cref="Dictionary"/> 实例（位于 <c>AELP.Data</c> 命名空间）。</param>
    /// <returns>表示异步操作完成的 <see cref="Task"/>。</returns>
    public Task AddToFavorites(Dictionary favorite);
    /// <summary>
    /// 从收藏中异步移除指定的 <see cref="Dictionary"/> 项并保存更改。
    /// </summary>
    /// <param name="favorite">对应的收藏</param>
    /// <returns>表示异步操作完成的任务。</returns>
    public Task RemoveFromFavorites(Dictionary favorite);
    /// <summary>
    /// 异步保存整个收藏列表。实现应负责持久化（例如写入文件或数据库）并在适当时保证线程安全。
    /// </summary>
    /// <param name="favorites">收藏数组</param>
    /// <returns>表示异步操作完成的任务。</returns>
    public Task SaveFavorites(Dictionary[] favorites);
    /// <summary>
    /// 加载收藏列表的异步方法。实现应负责从持久化存储（例如文件或数据库）读取数据并返回一个包含收藏项的数组。
    /// </summary>
    /// <returns>收藏数组</returns>
    public Task<FavoritesDataModel[]> LoadFavorites();

    /// <summary>
    /// 当收藏数据发生变化时触发。
    /// </summary>
    public event EventHandler OnFavoritesChanged;
}

/// <summary>
/// 收藏项更新事件参数。
/// </summary>
public class FavoriteStorageUpdatedEventArgs(FavoritesDataModel favorite) : EventArgs
{
    /// <summary>
    /// 获取更新后的收藏项。
    /// </summary>
    public FavoritesDataModel UpdatedFavorite { get; } = favorite;
}