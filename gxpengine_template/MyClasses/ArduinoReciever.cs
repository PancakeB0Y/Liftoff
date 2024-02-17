using System;
using System.IO.Ports;

namespace gxpengine_template.MyClasses
{
    //use this for getting sensor info from arduino, NOT Input data
    public class ArduinoReciever
    {
        readonly SerialPort _port = new SerialPort();

        public ArduinoReciever()
        {
            //configuration
            //every time you connect arduino, you should change to the apropriate com
            _port.PortName = "COM9";
            _port.BaudRate = 9600;
            _port.RtsEnable = true;
            _port.DtrEnable = true;

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void Update()
        {
            if (!_port.IsOpen) { return; }

            string a = _port.ReadExisting();
            if (a != "")
                Console.WriteLine(a);
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                _port.Write(key.KeyChar.ToString());
            }
        }
    }
}
