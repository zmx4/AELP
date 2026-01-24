using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public interface IUserWordQueryService
{
    public Task<WordDataModel?> QueryUserWordInfoAsync(int wordId);
}