using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIngestAPI.Services
{
    public interface IDbService
    {

        Task Insertdata(string data,string _idTankitem , string _idMqttclient,string _thisAPPsubmqttID);
        Task InitCreation(bool _enableinfoLog);
       // Task Cleantable(); //opt

    }
}
