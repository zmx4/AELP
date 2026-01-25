using System;
using System.Collections.Generic;
using AELP.Data;
using AELP.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class TestWordGetter(AppDbContext context) : ITestWordGetter
{
    private readonly AppDbContext _dbContext = context;

    public Task<List<Word>> GetTestWords(int count, TestRange testRange)
    {
        return testRange switch
        {
            TestRange.Cet4 => _dbContext.CET4s
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            TestRange.Cet6 => _dbContext.CET6s
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            TestRange.Senior => _dbContext.HighSchools
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            TestRange.Toefl => _dbContext.tfs
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            TestRange.Ielts => _dbContext.ys
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            TestRange.Primary => _dbContext.PrimarySchools
                .OrderBy(r => EF.Functions.Random())
                .Take(count)
                .ToListAsync()
                .ContinueWith(t => t.Result.Cast<Word>().ToList()),
            _ => throw new ArgumentOutOfRangeException(nameof(testRange), testRange, null)
        };
    }
}