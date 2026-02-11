using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public interface IUserWordQueryService
{
    public Task<WordDataModel?> QueryUserWordInfoAsync(int wordId);
}