
namespace IotDeviceSi7021TestHarness
{
    using System.Device.I2c;
    using System.Diagnostics;
    using System.Threading;
    using Iot.Device.Si7021;

    public class Program
    {
        public static void Main()
        {
            I2cConnectionSettings settings = new(1, Si7021.DefaultI2cAddress);

            using I2cDevice device = I2cDevice.Create(settings);
            using Si7021 sensor = new(device, Resolution.Resolution1);
            {
               while (true)
                {
                    var tempValue = sensor.Temperature;

                    Debug.WriteLine($"Temperature: {tempValue.DegreesCelsius:N2}\u00B0C");

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
