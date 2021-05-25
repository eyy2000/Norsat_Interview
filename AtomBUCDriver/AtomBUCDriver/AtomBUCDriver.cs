using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Threading;

namespace AtomBUC
{   
    public class AtomBUCErrorException : Exception
    {
        public AtomBUCErrorException()
        {

        }

        public AtomBUCErrorException(String _errorMessage) : base(String.Format("AtomBUCError: {0}", _errorMessage))
        {

        }
    }
    public class AtomBUCDriver
    {
        protected SerialPort _serialPort = new SerialPort();

        protected int nMuteStatus = 7;
        protected int nFaultType = 3;

        public AtomBUCDriver()
        {
            _serialPort.NewLine = "\r\n";
        }

        /// <summary>
        /// Opens a serial Port
        /// </summary>
        /// <returns>
        /// Void
        /// </returns>
        public void OpenSerialPort()
        {
            try
            {
                _serialPort.Open();
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Closes a serial Port
        /// </summary>
        /// <returns>
        /// Void
        /// </returns>
        public void CloseSerialPort()
        {
            try
            {
                _serialPort.Close();
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Sents a message through the serial port. 
        /// Format the message with <CR> according to protocol.
        /// Returns the response as a tokenized String array.
        /// </summary>
        /// <param name="_msg">String - Input to be sent</param>
        /// <returns>
        /// String[] - The tokenized response as a String array
        /// </returns>
        protected String[] SentMessage(String _msg)
        {
            _serialPort.Write(_msg+'\r');
            _serialPort.ReadLine();
            return _serialPort.ReadLine().Split();
        }


        /// <summary>
        /// Gets the temperature of a module in Celsius accroding to the id provided
        /// </summary>
        /// <param name="_id">uint8 - ID of the module</param>
        /// <returns>
        /// int - The temperature of the module in Celsius
        /// </returns>
        public int GetTemperature(byte _id)
        {
            String[] _tokenizedResponse = SentMessage("gettemp id " + _id);
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }

            return Int32.Parse(_tokenizedResponse[2]);
        }

        /// <summary>
        /// Gets the temperature of all 5 modules in Celsius
        /// </summary>
        /// <returns>
        /// int[] - The temperature of all modules in Celsius as int array
        /// </returns>
        public int[] GetTemperature()
        {
            int[] result = new int[5];

            for(int i = 1; i<=5; i++)
            {
                try
                {
                    result[i - 1] = GetTemperature(Convert.ToByte(i));
                }
                catch(AtomBUCErrorException e)
                {
                    throw e;
                }

            }

            return result;
        }


        /// <summary>
        /// Gets all of the fault status
        /// </summary>
        /// <returns>
        /// bool[] - MuteFault, OverTemperatureFault, PLLFault in that order as bool array
        /// </returns>
        public bool[] GetAllFaults()
        {
            String[] _tokenizedResponse = SentMessage("getfaults");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }

            /*
             *  The array contains three boolean in the following order
             *  bool bMuteFault    = _tokenizedResponse[2] == "1";
             *  bool bOverTmpFault = _tokenizedResponse[4] == "1";
             *  bool bPLLFault     = _tokenizedResponse[6] == "1";
             */

            bool[] result = new bool[nFaultType];
            for(int i = 1; i<= nFaultType; i++)
            {
                result[i-1] = _tokenizedResponse[i * 2] == "1";
            }
            return result;
        }


        /// <summary>
        /// Gets RF forward power in dBm
        /// </summary>
        /// <returns>
        /// double - The RF forward power in dBm as double
        /// </returns>
        public double GetRFForwardPower()
        {
            String[] _tokenizedResponse = SentMessage("getrfpwr dir 1");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }

            return Convert.ToDouble(_tokenizedResponse[2]);
        }


        /// <summary>
        /// Gets serial number of device as string
        /// </summary>
        /// <returns>
        /// String - The serial number of device as string
        /// </returns>
        public String GetSerialNumber()
        {
            String[] _tokenizedResponse = SentMessage("getident");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }

            return _tokenizedResponse[6];
        }


        /// <summary>
        /// Gets Synthesizer Attenuation in dB
        /// </summary>
        /// <returns>
        /// double - The Synthesizer Attenuation in dB
        /// </returns>
        public double GetSynthAttenuation()
        {
            String[] _tokenizedResponse = SentMessage("getdat");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }

            return Int32.Parse(_tokenizedResponse[2])/2.0;
        }


        /// <summary>
        /// Sets Synthesizer Attenuation
        /// </summary>
        /// <param name="_synthValue">uint16 - The DAT value that represents the attenuation. Valid values range from 0 to 63.</param>
        /// <returns>
        /// Void
        /// </returns>
        public void SetSynthAttenuation(ushort _synthValue)
        {
            String[] _tokenizedResponse = SentMessage("setdat value " + _synthValue);
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }
        }


        /// <summary>
        /// Mutes the BUC
        /// </summary>
        /// <returns>
        /// Void
        /// </returns>
        public void MuteBUC()
        {
            String[] _tokenizedResponse = SentMessage("setmute cmd 1");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length-1));
            }
        }


        /// <summary>
        /// Unmutes the BUC
        /// </summary>
        /// <returns>
        /// Void
        /// </returns>
        public void UnMuteBUC()
        {
            String[] _tokenizedResponse = SentMessage("setmute cmd 0");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }
        }


        /// <summary>
        /// Gets the BUC Mute state, checks for Mute Status value in the device
        /// </summary>
        /// <returns>
        /// bool - A boolean representing if the device is muted. True if the device is muted.
        /// </returns>
        public bool GetBUCMuteState()
        {
            String[] _tokenizedResponse = SentMessage("getmute");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }
            return _tokenizedResponse[2] == "1";
        }


        /// <summary>
        /// Gets all the states from the getmute command
        /// </summary>
        /// <returns>
        /// bool[] - An array representing the all of the states as bool
        /// </returns>
        public bool[] GetBUCMuteStateAll()
        {
            String[] _tokenizedResponse = SentMessage("getmute");
            if (_tokenizedResponse[0] == "err")
            {
                throw new AtomBUCErrorException(String.Join(" ", _tokenizedResponse, 1, _tokenizedResponse.Length - 1));
            }
            /*  The array contains three boolean in the following order
             *  bool _muteStatus   = _tokenizedResponse[2] == "1";
             *  bool _muteInvert   = _tokenizedResponse[4] == "1";
             *  bool _muteInput    = _tokenizedResponse[6] == "1";
             *  bool _muteBias     = _tokenizedResponse[8] == "1";
             *  bool _muteOverride = _tokenizedResponse[10] == "1";
             *  bool _muteCommand  = _tokenizedResponse[12] == "1";
             *  bool _muteFault    = _tokenizedResponse[14] == "1";
             */
            bool[] result = new bool[nMuteStatus];
            for (int i = 1; i <= nMuteStatus; i++)
            {
                result[i-1] = _tokenizedResponse[i * 2] == "1";
            }
            return result;
        }


        /// <summary>
        /// Set port name of serial port, default COM1
        /// </summary>
        /// <param name="_portName">String - The name of serial port</param>
        /// <returns>
        /// String - The updated port name as string
        /// </returns>
        public String SetPortName(String _portName="COM1")
        {
            _serialPort.PortName = _portName;
            return _serialPort.PortName;
        }


        /// <summary>
        /// Get port name of serial port
        /// </summary>
        /// <returns>
        /// String - The port name as string
        /// </returns>
        public String GetPortName()
        {
            return _serialPort.PortName;
        }


        /// <summary>
        /// Set baud rate of serial port, default 9600
        /// </summary>
        /// <param name="_baudRate">int - The baud rate</param>
        /// <returns>
        /// int - The updated baud rate as int
        /// </returns>
        public int SetPortBaudRate(int _baudRate=9600)
        {
            _serialPort.BaudRate = _baudRate;
            return _serialPort.BaudRate;
        }


        /// <summary>
        /// get baud rate of serial port
        /// </summary>
        /// <returns>
        /// int - The baud rate as int
        /// </returns>
        public int GetPortBaudRate()
        {
            return _serialPort.BaudRate;
        }


        /// <summary>
        /// Set parity of serial port, default 0
        /// </summary>
        /// <param name="_parity">String - The Parity, ranges from 0~4</param>
        /// <returns>
        /// int - The updated parity as Parity
        /// </returns>
        public Parity SetPortParity(String _parity="0")
        {
            _serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity, true);
            return _serialPort.Parity;
        }


        /// <summary>
        /// Get parity of serial port
        /// </summary>
        /// <returns>
        /// int - The parity as Parity
        /// </returns>
        public Parity GetPortParity()
        {
            return _serialPort.Parity;
        }

        /// <summary>
        /// Set data bits of serial port, default 8
        /// </summary>
        /// <param name="_dataBits">int - The data bits, ranges from 5~8</param>
        /// <returns>
        /// int - The updated data bits as int
        /// </returns>
        public int SetPortDataBits(int _dataBits=8)
        {
            _serialPort.DataBits = _dataBits;
            return _serialPort.DataBits;
        }


        /// <summary>
        /// Get data bits of serial port
        /// </summary>
        /// <returns>
        /// int - The data bits as int
        /// </returns>
        public int GetPortDataBits()
        {
            return _serialPort.DataBits;
        }


        /// <summary>
        /// Set Stop Bits of serial port, default 1
        /// </summary>
        /// <param name="_stopBits">String - The stop bits, ranges from 1~3</param>
        /// <returns>
        /// int - The updated stop bits as int
        /// </returns>
        public StopBits SetPortStopBits(String _stopBits="1")
        {
            _serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits, true);
            return _serialPort.StopBits;
        }


        /// <summary>
        /// Get Stop Bits of serial port
        /// </summary>
        /// <returns>
        /// int - The stop bits as int
        /// </returns>
        public StopBits GetPortStopBits()
        {
            return _serialPort.StopBits;
        }


        /// <summary>
        /// Set Handshake of serial port, default 0
        /// </summary>
        /// <param name="_handshake">String - The hand shake</param>
        /// <returns>
        /// String - The updated handshake
        /// </returns>
        public Handshake SetPortHandshake(String _handshake="0")
        {
            _serialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), _handshake, true);
            return _serialPort.Handshake;
        }


        /// <summary>
        /// Get Handshake of serial port, default 0
        /// </summary>
        /// <returns>
        /// String - The handshake
        /// </returns>
        public Handshake GetPortHandshake()
        {
            return _serialPort.Handshake;
        }
    }


}
