using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;
namespace BUCEmulator
{
    class Program
    {
        static bool _continue;
        static SerialPort _sp;

        static bool   HardwareLine = false;

        static int    MuteStatus = 0;
        static int    MuteInvert = 0;
        static int    MuteInput = 0;
        static int    MuteBias = 0;
        static int    MuteOverride = 0;
        static int    MuteCommand = 0;
        static int    MuteFault = 0;
        static int    DATValue = 32;

        static int    PLLFault = 0;
        static int    PowerFault = 0;
        static int    OverTmpFault = 1;

        static String RFPowerReading = "+18.5";
        static int    BinaryPowerValue = 585;
        static int    ADCReading = 19;

        static int[] Temperatures = { 32, 37, 28, 59, 38 };

        static String PartName = "UC-Ku50";
        static String SoftwareVersion = "1.0.1_1";
        static String SerialNumber = "BUC-Ku25-30124";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            _sp = new SerialPort();
            Console.WriteLine("Press Enter without argument for default input");

            Console.WriteLine("Choose port for test run");
            Console.WriteLine("Available Ports: (Default COM3)");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            String _portNameInput = Console.ReadLine();
            if (_portNameInput == "")
            {
                _sp.PortName = "COM3";
            }
            else
            {
                _sp.PortName = _portNameInput;
            }
            Console.WriteLine("Port Name: " + _sp.PortName);
            _sp.NewLine = "\r";
            _sp.ReadTimeout = 500;
            _sp.WriteTimeout = 500;

            _continue = true;
            _sp.Open();

            Thread rThread = new Thread(read);
            rThread.Start();

            while (_continue)
            {
                if (Console.ReadLine().Equals("quit"))
                {
                    _continue = false;
                }
            }

            rThread.Join();
            _sp.Close();
        }

        public static void FormatResponse(ref String response)
        {
            response = "\r\n" + response + "\r\n";
        }
        public static void read()
        {
            while (_continue)
            {
                try
                {
                    String msg = _sp.ReadLine();
                    Console.WriteLine("======================");
                    Console.WriteLine("FROM DRIVER: " + msg);
                    String[] tokenizedMsg = msg.Split(' ');
                    if (tokenizedMsg[0].Equals("getmute"))
                    {
                        String response = String.Format("ok gate {0} uc {1} in {2} bias {3} ovrd {4} cmd {5} fault {6}",
                                                        MuteStatus,
                                                        MuteInvert,
                                                        MuteInput,
                                                        MuteBias,
                                                        MuteOverride,
                                                        MuteCommand,
                                                        MuteFault);
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "setmute")
                    {
                        var param = new Dictionary<String, String>();
                        String response;
                        if (tokenizedMsg.Length % 2 == 0)
                        {
                            response = "err Missing Parameter";
                        }
                        else
                        {
                            for (int i = 1; i <= tokenizedMsg.Length / 2; i++)
                            {
                                param[tokenizedMsg[(i - 1) * 2 + 1]] = tokenizedMsg[(i - 1) * 2 + 2];
                            }
                            response = "ok";
                            if (param.ContainsKey("uc"))
                            {
                                MuteInvert = param["uc"] == "1" ? 1:0;
                            }
                            if (param.ContainsKey("bias"))
                            {
                                MuteBias = param["bias"] == "1" ? 1 : 0;
                            }
                            if (param.ContainsKey("cmd"))
                            {
                                MuteCommand = param["cmd"] == "1" ? 1 : 0;

                                bool bInvert = MuteInvert == 1;
                                bool bCmd = MuteCommand == 1;
                                
                                bool bPLLFault = PLLFault == 1;
                                bool bPowerFault = PowerFault == 1;
                                bool bOverTempFault = OverTmpFault == 1;
                                bool bOnFault = MuteFault == 1;

                                MuteStatus = ((HardwareLine ^ bInvert) || bCmd || (bOnFault && (bPLLFault || bPowerFault || bOverTempFault)))? 1:0;
                            }

                        }
                        
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "getdat")
                    {
                        String response = "ok value " + DATValue;
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);

                    }
                    else if (tokenizedMsg[0] == "setdat")
                    {
                        String response;
                        if (tokenizedMsg.Length != 3)
                        {
                            response = "err Missing Parameter";
                        }
                        else
                        {
                            response = "ok";
                            DATValue = Int32.Parse(tokenizedMsg[2]);
                        }
                        
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "getident")
                    {
                        String response = String.Format("ok pn {0} swver {1} sn {2}",
                                                        PartName,
                                                        SoftwareVersion,
                                                        SerialNumber);
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "getrfpwr")
                    {
                        String response = String.Format("ok dBm {0} binary {1} adc {2}",
                                                        RFPowerReading,
                                                        BinaryPowerValue,
                                                        ADCReading);
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "getfaults")
                    {
                        String response = String.Format("ok mute {0} overTemp {1} pll {2}",
                                                        (MuteCommand == 1 || HardwareLine) ? 1:0,
                                                        OverTmpFault,
                                                        PLLFault);
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    else if (tokenizedMsg[0] == "gettemp")
                    {
                        int moduleID = Int32.Parse(tokenizedMsg[2])-1;
                        String response = String.Format("ok tempC {0} binary {1} adc {2}",
                                                        Temperatures[moduleID],
                                                        Temperatures[moduleID]+200,
                                                        Temperatures[moduleID]+1000);
                        FormatResponse(ref response);
                        Console.WriteLine("RESPONSE WITH: " + response);
                        _sp.WriteLine(response);
                    }
                    Console.WriteLine("======================");
                }
                catch (TimeoutException)
                {

                }
                
            }
        }
    }
}
