using Dbcheck;
using Microsoft.Extensions.DependencyInjection;
using Watertank.api.integration.test.Services;

namespace Watertank.api.integration.test
{
    public class LogicApiTests(ProgramTestApplicationFactory factory) : IClassFixture<ProgramTestApplicationFactory>
    {
        private readonly HttpClient client = factory.CreateClient();
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

            Dbintzz.Raw_Insert( datatoinsert );
            
            var data = Dbintzz.Raw_check( datatoinsert );

            data = data.OrderBy(x=> x.client_id).ToList();

            datatoinsert = datatoinsert.OrderBy(x => x.client_id).ToList();
            tank_ids = datatoinsert.Select(x => x.tank_id).ToList();
            ///////////////


            Assert.NotNull( data );
            Assert.Equal(datatoinsert.Count, data.Count );


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


    }
}
