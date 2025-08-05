
//using System.Data;
using Dapper;
using Dbcheck;
using Npgsql;
using Watertank.api.integration.test.Models;

namespace Watertank.api.integration.test.Services;

public class DatabaseInserter(Dbinit dbinit) : IDatabaseInserter
{

    public List<Dbrecord> Raw_check(List<Dbrecord> _data)
    {


        using var Dbconn = new NpgsqlConnection(dbinit.Connstring);

        Dbconn.Open();

        List<Guid> Tankidtoquery = [];

        if (_data != null && _data.Count > 0)
        {
            
            Tankidtoquery = _data.Select(a => a.tank_id).ToList();
           
        }
        else { throw new ArgumentNullException(nameof(_data)); }



        string sql = "SELECT * FROM watertank " +
                "WHERE tank_id = ANY(@TANKIDs)";

        var datareturn = Dbconn.Query<Dbrecord>(sql, new { TANKIDs = Tankidtoquery }).ToList();

        return datareturn;


        //toedit
       
    }


    /// <param name="_zone_code"> Max 10 char included </param>
    public void Raw_Insert(List<Dbrecord> _data) 
    {

        if(_data.Count == 0) throw new ArgumentNullException();
        foreach (var i in _data) 
        {
            if (i.zone_code is not null && i.zone_code.Length > 10)
                throw new ArgumentException($"tank_id_trace:{i.tank_id} : zonecode must be null or contain fewer than 10 characters included");
        }


        using var Dbconn = new NpgsqlConnection(dbinit.Connstring);

        const string sql = @"INSERT INTO watertank (time, tank_id, current_volume, client_id, zone_code,total_capacity)
                                   VALUES (@time, @tank_id, @current_volume, @client_id, @zone_code, @total_capacity)";



        Dbconn.Execute(sql, _data);

    }




}

