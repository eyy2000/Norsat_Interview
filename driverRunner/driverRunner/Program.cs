using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using AtomBUC;

namespace driverRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            AtomBUCDriver driver = new AtomBUCDriver();
            Console.WriteLine("Press Enter without argument for default input");

            Console.WriteLine("Choose port for test run");
            Console.WriteLine("Available Ports: (Default COM2)");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            String _portNameInput = Console.ReadLine();
            if (_portNameInput == ""){
                driver.SetPortName("COM2");
            }
            else
            {
                driver.SetPortName(_portNameInput);
            }
            Console.WriteLine("Port Name: " + driver.GetPortName());


            Console.WriteLine("Set Integer Baud Rate (Default 9600)");
            String _BaudRateInput = Console.ReadLine();
            if (_BaudRateInput == "")
            {
                driver.SetPortBaudRate();
            }
            else
            {
                driver.SetPortBaudRate(Int32.Parse(_BaudRateInput));
            }
            Console.WriteLine("Baud Rate: " + driver.GetPortBaudRate());


            Console.WriteLine("Set Parity 0~4 (Default 0)");
            String _PortParityInput = Console.ReadLine();
            if (_PortParityInput == "")
            {
                driver.SetPortParity();
            }
            else
            {
                driver.SetPortParity(_PortParityInput);
            }
            Console.WriteLine("Parity: "    + driver.GetPortParity());


            Console.WriteLine("Set Data Bits 5~8 (Default 8)");
            String _PortDataBitsInput = Console.ReadLine();
            if (_PortDataBitsInput == "")
            {
                driver.SetPortDataBits();
            }
            else
            {
                driver.SetPortDataBits(Int32.Parse(_PortDataBitsInput));
            }
            Console.WriteLine("Data Bits: " + driver.GetPortDataBits());


            Console.WriteLine("Set Stop Bits 1~3 (Default 1)");
            String _PortStopBitsInput = Console.ReadLine();
            if (_PortStopBitsInput == "")
            {
                driver.SetPortStopBits();
            }
            else
            {
                driver.SetPortStopBits(_PortStopBitsInput);
            }
            Console.WriteLine("Stop Bits: " + driver.GetPortStopBits());


            Console.WriteLine("Set Handshake (Default 0)");
            String _PortHandshakeInput = Console.ReadLine();
            if (_PortHandshakeInput == "")
            {
                driver.SetPortHandshake();
            }
            else
            {
                driver.SetPortHandshake(_PortHandshakeInput);
            }
            Console.WriteLine("Handshake: " + driver.GetPortHandshake());
            
            Console.WriteLine("Opening Port...");
            try
            {
                driver.OpenSerialPort();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error Opening Port");
                Console.WriteLine(e);
                Console.WriteLine("Press Enter key to exit...");
                Console.ReadLine();
                System.Environment.Exit(1);
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'GetBUCMuteState\'");
            Console.WriteLine("Get mute state...");
            try
            {
                Console.WriteLine("Response: " + driver.GetBUCMuteState());
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'GetBUCMuteStateAll\'");
            Console.WriteLine("Get mute state...");
            try
            {
                bool[] _serverResp = driver.GetBUCMuteStateAll();
                Console.Write("Response: [");
                String sep = "";
                foreach(bool state in _serverResp)
                {
                    Console.Write(sep);
                    sep = ",";
                    Console.Write(state);
                }
                Console.WriteLine("]");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");


            Console.WriteLine("Demonstrating \'MuteBUC\'");
            Console.WriteLine("Set mute state...");
            try
            {
                driver.MuteBUC();
                Console.WriteLine("Check updated status");
                Console.WriteLine("Response: " + driver.GetBUCMuteState());
            }
            catch(AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'UnMuteBUC\'");
            Console.WriteLine("Set mute state...");
            try
            {
                driver.UnMuteBUC();
                Console.WriteLine("Check updated status");
                Console.WriteLine("Response: " + driver.GetBUCMuteState());
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");


            Console.WriteLine("Demonstrating \'GetSynthAttenuation\'");
            try
            {
                Console.WriteLine("Response: " + driver.GetSynthAttenuation() + "dB");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'SetSynthAttenuation\'");
            try
            {
                driver.SetSynthAttenuation(Convert.ToUInt16(15));
                Console.WriteLine("Check updated value");
                Console.WriteLine("Response: " + driver.GetSynthAttenuation() + "dB");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'SerialNumber\'");
            try
            {
                Console.WriteLine("Response: " + driver.GetSerialNumber());
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'GetRFForwardPower\'");
            try
            {
                Console.WriteLine("Response: " + driver.GetRFForwardPower() + "dBm");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'GetAllFaults\'");
            try
            {
                bool[] _serverResp = driver.GetAllFaults();
                Console.Write("Response: [" );
                String sep = "";
                foreach (bool fault in _serverResp)
                {
                    Console.Write(sep);
                    sep = ",";
                    Console.Write(fault);
                }
                Console.WriteLine("]");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");

            Console.WriteLine("Demonstrating \'GetTemperature\'");
            Console.WriteLine("Get One version");
            try
            {
                Console.WriteLine("Get temp of module 2");
                int _serverResp = driver.GetTemperature(Convert.ToByte(2));
                Console.WriteLine("Response: " + _serverResp);
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");
            Console.WriteLine("Get all version");
            try
            {
                int[] _serverResp = driver.GetTemperature();
                Console.Write("Response: [");
                String sep = "";
                foreach(int tmp in _serverResp)
                {
                    Console.Write(sep);
                    sep = ",";
                    Console.Write(tmp);
                }
                Console.WriteLine("]");
            }
            catch (AtomBUCErrorException e)
            {
                Console.Write("An Error has occured " + e.ToString());
            }
            Console.WriteLine("Done.");
            Console.WriteLine("");


            Console.WriteLine("Closing Port...");
            driver.CloseSerialPort();
            Console.WriteLine("Done.");

            Console.WriteLine("Press Enter key to exit...");
            Console.ReadLine();
        }
    }
}
