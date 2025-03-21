namespace DataFlow_ReadAPI.Models
{

    /*  NAME                  DATA TYPE
        time                  timestamp with timezone  //todo? without TZ 
        tank_id               uuid
        current_volume        double precision
        client_id             uuid
        zone_code             VARCHAR(10)
        total_capacity        double precision
    */


    //dbtable
    public enum Watertank
    {
        time,
        tank_id,
        current_volume,
        client_id,
        zone_code,
        total_capacity
    }
}
