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
#undef DEBUG
namespace devMobile.NetNF.Sensor
{
    using System;
    using System.Diagnostics;
    using System.Device.I2c;

    public class SiliconLabsSI7005
    {
        private readonly I2cDevice SI7005Device;
        private const byte DeviceId = 0x40;

        private const int RegisterIdConfiguration = 0x03;
        private const int RegisterIdStatus = 0x00;
        private const byte RegisterIdDeviceId = 0x11;
        private const byte RegisterDeviceId = 0x50;
        private const byte StatusMask = 0x01;
        private const byte CommandMeasureTemperature = 0x11;
        private const byte CommandMeasureHumidity = 0x01;
        private const byte ConversionDataRegister = 0x01;


        public SiliconLabsSI7005(int i2cBusID, int address = DeviceId)
        {
            // instantiate I2C controller
            SI7005Device = I2cDevice.Create(new I2cConnectionSettings(i2cBusID, address));

            byte[] readBuffer = new byte[1];

            SI7005Device.WriteRead(new byte[] { RegisterIdDeviceId }, readBuffer);

            if (readBuffer[0] != RegisterDeviceId)
            {
                throw new Exception("DeviceId invalid");
            }
        }

        public double Temperature()
        {
            bool conversionInProgress = true;

            Debug.WriteLine("Temperature measurement start");

            byte[] CmdBuffer = { RegisterIdConfiguration, CommandMeasureTemperature };

            SI7005Device.Write(CmdBuffer);

            Debug.WriteLine(" Wait");

            // Wait for measurement
            do
            {
                byte[] WaitWriteBuffer = { RegisterIdStatus };
                byte[] WaitReadBuffer = new byte[1];

                SI7005Device.WriteRead(WaitWriteBuffer, WaitReadBuffer);
                if ((WaitReadBuffer[RegisterIdStatus] & StatusMask) != StatusMask)
                {
                    conversionInProgress = false;
                }
            } while (conversionInProgress);


            // Read temperature value
            Debug.WriteLine(" Read");
            byte[] valueWriteBuffer = { ConversionDataRegister };
            byte[] valueReadBuffer = new byte[2];

            SI7005Device.WriteRead(valueWriteBuffer, valueReadBuffer);

            //   // Convert bye to centigrade
            int temp = valueReadBuffer[0];

            temp = temp << 8;
            temp = temp + valueReadBuffer[1];
            temp = temp >> 2;
            /* 
              Formula: Temperature(C) = (Value/32) - 50	  
            */
            double temperature = (temp / 32.0) - 50.0;

            Debug.WriteLine($" Done {temperature}°");

            return temperature;
        }
    }
}