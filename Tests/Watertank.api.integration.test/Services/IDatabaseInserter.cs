
using Watertank.api.integration.test.Models;

namespace Watertank.api.integration.test.Services
{
   public interface IDatabaseInserter
    {

        void Raw_Insert(List<Dbrecord> _data);


        List<Dbrecord> Raw_check(List<Dbrecord> _data);
    }
}
