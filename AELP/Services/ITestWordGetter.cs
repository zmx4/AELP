using System.Collections.Generic;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

public interface ITestWordGetter
{
    public Task<List<Word>> GetTestWords(int count, TestRange testRange);
}