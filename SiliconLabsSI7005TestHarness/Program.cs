//---------------------------------------------------------------------------------
// Copyright (c) June 2020, devMobile Software
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------
namespace devMobile.SiliconLabsSI7005TestHarness
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using devMobile.NetNF.Sensor;
    using System.Device.Gpio;

    public class Program
    {
        private static GpioController s_GpioController;

        public static void Main()
        {
            s_GpioController = new GpioController();

            try
            {
                Debug.WriteLine("devMobile.SiliconLabsSI7005TestHarness starting");

                // STM32F091RC: PA5 is LED_GREEN
                // nanoff --target ST_NUCLEO64_F091RC --update
                //GpioPin led = GpioController.GetDefault().OpenPin(PinNumber('A', 5));

                // nanoff --target NETDUINO3_WIFI --update
                //GpioPin led = GpioController.GetDefault().OpenPin(PinNumber('A', 10));

                // nanoff --target ST_STM32F769I_DISCOVERY --update
                GpioPin led = s_GpioController.OpenPin(PinNumber('J', 5));

                led.SetPinMode(PinMode.Output);

                SiliconLabsSI7005 sensor = new SiliconLabsSI7005(1);
                Debug.WriteLine(" while starting");

                while (true)
                {
                    double temperature = sensor.Temperature();

                    Debug.WriteLine($"{DateTime.UtcNow:hh:mm:ss} T:{temperature:f1}?");

                    led.Toggle();

                    Thread.Sleep(10000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            Debug.WriteLine("Terminated");
            Thread.Sleep(Timeout.Infinite);
        }

        static int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();

            return ((port - 'A') * 16) + pin;
        }
    }
}
