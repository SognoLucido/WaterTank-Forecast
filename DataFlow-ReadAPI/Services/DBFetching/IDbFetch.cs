using DataFlow_ReadAPI.Models;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public interface IDbFetch
    {
        Task<IEnumerable<DbInfoItem>?> GetinfoItem(Guid[] Ids ,bool clientid,bool zcode,bool totcap );
        Task<IEnumerable<DbInfoItemwithDATEtime>?> GetinfoItem(Guid[] Ids, bool clientid, bool zcode, bool totcap, DateTime a, DateTime b);
        Task<DBreturnDataDto?> Forecast(Guid[]? tank_ids, int Rangedays,Guid? clientid,string? zcode );
    }
}
