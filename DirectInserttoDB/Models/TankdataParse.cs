using System.Globalization;

namespace DirectInserttoDB.Models
{
    class TankdataParse
    {


        public DateTime Time { get; init; }
        public Guid TankId { get; init; }
        public double CurrentVolume { get; init; }
        public Guid? ClientId { get; init; }
        public string? ZoneCode { get; init; }
        public double? TotalCapacity { get; init; }



        public static TankdataParse? Parse(string line)
        {

            //  return null;
            var parts = line.Split(',');

            if (parts.Length > 6 && parts.Length < 3) return null;

            try
            {

                if (!DateTime.TryParse(parts[0], out var _date)) return null;
                if (!Guid.TryParse(parts[1], out var _tankid)) return null;
                if (!double.TryParse(parts[2], out var _currentVol)) return null;


                Guid _clientId = Guid.Empty;

                if (parts.Length > 3 && !Guid.TryParse(parts[3], out _clientId))
                {
                    if (_clientId != Guid.Empty) return null;
                }


                string? _zoneCode = String.Empty;
                //db VARCHAR10 check
                if(parts.Length > 4 )
                if (parts[4].Length <= 10)
                {
                    _zoneCode = string.IsNullOrEmpty(parts[4]) ? null : parts[4];
                }
                else return null;


                Double? _totalCap;

                if (parts.Length > 5 && !string.IsNullOrEmpty(parts[5]))
                {
                    if (!double.TryParse(parts[5], out var _totalCapTEMP)) return null;
                    else
                    {
                        _totalCap = _totalCapTEMP;
                    }
                }
                else _totalCap = null;


                return new TankdataParse
                {
                    Time = _date,
                    TankId = _tankid,
                    CurrentVolume = _currentVol,
                    ClientId = _clientId == Guid.Empty ? null : _clientId,
                    ZoneCode = _zoneCode,
                    TotalCapacity = _totalCap
                };

            }
            catch
            {
                return null;
            }









            //var item = new TankdataParse
            //{

            //    Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            //    TankId = Guid.Parse(parts[1]),
            //    CurrentVolume = double.Parse(parts[2], CultureInfo.InvariantCulture),
            //    ClientId = Guid.TryParse(parts[3], out var cid) ? cid : null,
            //    ZoneCode = string.IsNullOrWhiteSpace(parts[4]) ? null : parts[4],
            //    TotalCapacity = double.TryParse(parts[5], out var cap) ? cap : null
            //};


        }







    }
}
