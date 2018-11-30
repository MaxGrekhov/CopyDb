using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CopyDb.Models;

namespace CopyDb.Core
{
    public interface IDbProvider : IDisposable
    {
        IDisposable BeginTransaction();
        void Commit();

        Task<List<TableInfo>> GetInfos(string filter);
        Task<List<dynamic>> Select(TableInfo info, int page, int count);
        Task BeforeInsert(TableInfo info);
        Task Insert(TableInfo info, List<dynamic> items);
        Task AfterInsert(TableInfo info);
        Task Delete(TableInfo info);
    }
}
