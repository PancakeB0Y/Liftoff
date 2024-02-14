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
            _port.PortName = "COM9";
            _port.BaudRate = 9600;
            _port.RtsEnable = true;
            _port.DtrEnable = true;
            _port.Open();
        }

        public void Update()
        {
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
