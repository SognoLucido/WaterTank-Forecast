using System.Net.Http.Json;
using DataFlow_ReadAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Watertank.api.integration.test.Services;

namespace Watertank.api.integration.test
{
    public class LogicApiTests(ProgramTestApplicationFactory factory) : IClassFixture<ProgramTestApplicationFactory>
    {

        private readonly IDatabaseInserter Dbintzz = factory.Services.GetService<IDatabaseInserter>()!;

        /// <summary>
        /// (the api retrieves only data from the database).Testing the water tank DB table and its parameters with raw sql
        /// </summary>
        [Fact]
        public void Raw_DB_Tablecheck()
        {

            List<Guid> tank_ids = [];

            List<Dbrecord> datatoinsert = [

                new Dbrecord(DateTime.UtcNow ,new Guid("604D9A08-6C42-47E3-BE74-C68546600001"),101  ),
                new Dbrecord(DateTime.UtcNow ,new Guid("604D9A08-6C42-47E3-BE74-C68546600005"),101.3442  ),
                new Dbrecord(DateTime.UtcNow ,new Guid("604D9A08-6C42-47E3-BE74-C68546600002"),102 , new Guid("D007940E-E82A-4357-B372-430B31D3B002") ),
                 new Dbrecord(DateTime.UtcNow ,new Guid("604D9A08-6C42-47E3-BE74-C68546600003"),103 , null ,"test_zcode"  ),
                 new Dbrecord(DateTime.UtcNow ,new Guid("604D9A08-6C42-47E3-BE74-C68546600004"),104, null ,null,204 ),

                ];



            ///////////////

            Dbintzz.Raw_Insert(datatoinsert);

            var data = Dbintzz.Raw_check(datatoinsert);

            data = data.OrderBy(x => x.client_id).ToList();

            datatoinsert = datatoinsert.OrderBy(x => x.client_id).ToList();
            tank_ids = datatoinsert.Select(x => x.tank_id).ToList();
            ///////////////


            Assert.NotNull(data);
            Assert.Equal(datatoinsert.Count, data.Count);


            for (int i = 0; i < datatoinsert.Count; i++)
            {
                Assert.Contains(data, x => x.tank_id == datatoinsert[i].tank_id);
                var extractedrow = data.Where(x => x.tank_id == datatoinsert[i].tank_id).First();


                if (datatoinsert[i].total_capacity is not null)
                {

                    Assert.Equal(datatoinsert[i].total_capacity, extractedrow.total_capacity);
                }

                if (datatoinsert[i].zone_code is not null)
                {

                    Assert.Equal(datatoinsert[i].zone_code, extractedrow.zone_code);
                }

                if (datatoinsert[i].client_id is not null)
                {
                    Assert.Equal(datatoinsert[i].client_id, extractedrow.client_id);
                }




            }

        }


        [Fact]
        public async Task ApiForecast_getINFOTANK_endpoint()
        {
            var client = factory.CreateClient();

            DateTime date = DateTime.UtcNow;
            Guid TestTankid_1 = new("F2D485A8-AC9E-4668-BB2C-E4A00C1FF001");
            Guid TestTankid_2 = new("F2D485A8-AC9E-4668-BB2C-E4A00C1FF002");
            Guid TestClientid_1 = new("285D7C8C-2AB4-4DB9-B0B2-7494C505F001");
            Guid TestClientid_2 = new("285D7C8C-2AB4-4DB9-B0B2-7494C505F002");

            string SharedZode = "ZC_HELLO";

            List<Dbrecord> datatoinsert = [
                new Dbrecord(date,TestTankid_1,100,TestClientid_1,SharedZode,201),
                 new Dbrecord(date,TestTankid_2,102,TestClientid_2,SharedZode,202),
                ];

            Dbintzz.Raw_Insert(datatoinsert);

            ///////////// ACT


            var result_1 = await client.GetFromJsonAsync<List<DbInfoItem>>($"/api/tank/info?tankid={TestTankid_1}&clientid=true&zonecode=true&totalcapacity=true");
            var result_2 = await client.GetFromJsonAsync<List<DbInfoItem>>($"/api/tank/info?tankid={TestTankid_2}&clientid=true&zonecode=true&totalcapacity=true");

            /////////////

            //holy 
            Assert.True(
                new DateOnly(date.Year, date.Month, date.Day) == new DateOnly(result_1[0].time.Year, result_1[0].time.Month, result_1[0].time.Day)
                );

            //   Assert.Equal(date, result_1[0].time);
            Assert.Equal(TestTankid_1, result_1[0].tank_id);
            Assert.Equal(datatoinsert[0].current_volume, result_1[0].current_volume);
            Assert.Equal(SharedZode, result_1[0].zone_code);
            Assert.Equal(datatoinsert[0].total_capacity, result_1[0].total_capacity);
            Assert.Equal(TestClientid_1, result_1[0].client_id);

            //  Assert.Equal(date, result_2[1].time);

            Assert.True(
               new DateOnly(date.Year, date.Month, date.Day) == new DateOnly(result_2[0].time.Year, result_2[0].time.Month, result_2[0].time.Day)
               );

            Assert.Equal(TestTankid_2, result_2[0].tank_id);
            Assert.Equal(datatoinsert[1].current_volume, result_2[0].current_volume);
            Assert.Equal(SharedZode, result_2[0].zone_code);
            Assert.Equal(datatoinsert[1].total_capacity, result_2[0].total_capacity);
            Assert.Equal(TestClientid_2, result_2[0].client_id);

        }






        [Fact]
        public async Task ApiForecast_calculationFORECAST_logic()
        {
            var client = factory.CreateClient();
            int Dayincremental = 0;
            //DateTime time = DateTime.UtcNow;
            DateTime time = new(2025, 01, 01, 12, 0, 0);
            string zone_code = "ZC_HELLO";
            Guid TestTankid_1 = new("0636C302-4A85-4A0B-9B6D-B731C4890001");
            Guid TestTankid_2 = new("D57390AD-055B-4F19-8941-A798F4040002");
            Guid TestTankid_3 = new("AE4FF79E-1FBE-4172-95F5-9AE4D0D2C003");
            Guid TestClientid_2 = new("3CDE7984-225C-49F3-BAA6-E981D728FD74"); 


            List<Dbrecord> data_1 = [

              new Dbrecord(time  ,TestTankid_1,30,zone_code:zone_code),
                new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_1,30,zone_code:zone_code),
                 new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_1,30,zone_code:zone_code),  
                new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_1,28,zone_code:zone_code),
                 new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_1,25,zone_code:zone_code),
                 new Dbrecord(Incremental_Date(ref time, 3) ,TestTankid_1,19,zone_code:zone_code),
                 new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_1,15,zone_code:zone_code),

                ];


            ResetDate(ref time);
            Dayincremental = default;

            List<Dbrecord> data_2 = [

                new Dbrecord(time,TestTankid_2,30,TestClientid_2,zone_code),
                new Dbrecord(Incremental_Date(ref time, 1,2 ) ,TestTankid_2,28,TestClientid_2,zone_code),
                 new Dbrecord(Incremental_Date(ref time, Hour:3) ,TestTankid_2,25,TestClientid_2,zone_code), 
                  new Dbrecord(Incremental_Date(ref time, Hour:1) ,TestTankid_2,35,TestClientid_2,zone_code),
                    new Dbrecord(Incremental_Date(ref time, 1)  ,TestTankid_2,30, TestClientid_2,zone_code),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_2,24, TestClientid_2, zone_code),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_2,22, TestClientid_2, zone_code),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_2,19, TestClientid_2, zone_code)
                ];

            ResetDate(ref time);
            Dayincremental = default;

            List<Dbrecord> data_3 = [

                new Dbrecord(time,TestTankid_3,30,TestClientid_2),
                new Dbrecord(Incremental_Date(ref time, 1,2 ) ,TestTankid_2,28,TestClientid_2),
                 new Dbrecord(Incremental_Date(ref time, Hour:3) ,TestTankid_3,40,TestClientid_2), 
                  new Dbrecord(Incremental_Date(ref time, Hour:1) ,TestTankid_3,35,TestClientid_2),
                    new Dbrecord(Incremental_Date(ref time, 1)  ,TestTankid_3,30, TestClientid_2),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_3,24, TestClientid_2),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_3,22, TestClientid_2),
                     new Dbrecord(Incremental_Date(ref time, 1) ,TestTankid_3,19, TestClientid_2)
                ];


         

            data_1.AddRange(data_2);
            data_1.AddRange(data_3);
                
            Dbintzz.Raw_Insert(data_1);

            /// Forecasting formula explanation:
            /// 
            ///     nextForecastedDay = lastDay + 
            ///             (int)
            ///             (currentResourceLevel / 
            ///             (totalDailyConsumptionSum / totalDaysAppliedTo)
            ///             )
            ///
            /// - `lastDay`: The last known valid day
            /// - `currentResourceLevel`: Current amount of the resource
            /// - `totalDailyConsumptionSum`: Sum of daily consumption across all units (range_days or less) 
            /// - `totalDaysAppliedTo`: Number of days the consumption data covers


            ////////////  ACT

            var result_data_1 = await client.GetFromJsonAsync<DBreturnDataDto>($"/api/tank/forecast?tankids={TestTankid_1}");
            var result_data_2 = await client.GetFromJsonAsync<DBreturnDataDto>($"/api/tank/forecast?tankids={TestTankid_2}");
            var result_data_3 = await client.GetFromJsonAsync<DBreturnDataDto>($"/api/tank/forecast?tankids={TestTankid_3}");

            var get_zonecode = await client.GetFromJsonAsync<DBreturnDataDto>($"/api/tank/forecast?zone_code={zone_code}");
            var get_client = await client.GetFromJsonAsync<DBreturnDataDto>($"/api/tank/forecast?client_id={TestClientid_2}");
            
            ////////////
            
            var date_1 = result_data_1.Data.ToList()[0].empty_at_day;
            var date_2 = result_data_2.Data.ToList()[0].empty_at_day;
            var date_3 = result_data_3.Data.ToList()[0].empty_at_day;

            var zonecodelist = get_zonecode.Data.ToList();
            var clientlist = get_client.Data.ToList();

            Assert.Equal(new DateOnly(2025,1,17), new DateOnly(date_1.Year,date_1.Month,date_1.Day));
            Assert.Equal(new DateOnly(2025,1,11), new DateOnly(date_2.Year,date_2.Month,date_2.Day));
            Assert.Equal(date_2, date_3);

            Assert.Equal(2,get_zonecode.Data.Count());
            Assert.Equal(2, get_client.Data.Count());

            Assert.Contains(zonecodelist, x => x.tank_id == TestTankid_1);
            Assert.Contains(zonecodelist, x => x.tank_id == TestTankid_2);

            Assert.Contains(clientlist, x => x.tank_id == TestTankid_2);
            Assert.Contains(clientlist, x => x.tank_id == TestTankid_3);

        }



        void ResetDate(ref DateTime date) => date = new(2025, 01, 01, 12, 0, 0);

        DateTime Incremental_Date(ref DateTime date, int? Day = null, int? Hour = null)
        {

            //DateTime tempdate = date;

            //if (Hour is not null && Hour > 0)
            //{
            //    tempdate = tempdate.AddHours((int)Hour);
            //}

            //if (Day is not null && Day > 0)
            //{
            //    tempdate = tempdate.AddDays((int)Day);

            //}

            //date = tempdate;
            //return date;





            if (Hour is not null && Hour > 0)
            {
                date = date.AddHours((int)Hour);
            }

            if (Day is not null && Day > 0)
            {
                date = date.AddDays((int)Day);
            }

            return date;
        }

    }
}
