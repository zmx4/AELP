using System.Threading.Tasks;

namespace AELP.Services;

public interface IUserDbService
{
    public bool IsInitialized { get; }
    
    public Task Initialize();
    
    
}