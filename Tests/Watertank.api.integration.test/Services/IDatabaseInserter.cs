using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watertank.api.integration.test.Services
{
   public interface IDatabaseInserter
    {

        void Raw_Insert(List<Dbrecord> _data);


        List<Dbrecord> Raw_check(List<Dbrecord> _data);
    }
}
