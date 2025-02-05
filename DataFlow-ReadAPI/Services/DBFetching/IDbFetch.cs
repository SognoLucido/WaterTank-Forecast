using DataFlow_ReadAPI.Models;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public interface IDbFetch
    {
        Task<IEnumerable<DBreturnData>> Fetchdata(Guid[]? tank_ids, int Rangedays);
    }
}
