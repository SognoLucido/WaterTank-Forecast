using DataFlow_ReadAPI.Models;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public interface IDbFetch
    {
        Task<IEnumerable<DbDataInfoItem>?> GetinfoItem(Guid[] Ids ,bool clientid,bool zcode,bool totcap );
        Task<IEnumerable<DbDataInfoItem>?> GetinfoItem(Guid[] Ids, DateTime a, DateTime b);
        Task<IEnumerable<DBreturnData>> Fetchdata(Guid[]? tank_ids, int Rangedays);
    }
}
