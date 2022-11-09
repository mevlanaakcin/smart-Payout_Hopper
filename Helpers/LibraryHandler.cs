using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using ITLlib;

namespace eSSP_example
{
    public static class LibraryHandler
    {
        static SSPComms m_LibHandle = new SSPComms();
        static Stopwatch m_StopWatch = new Stopwatch();
        static Exception m_LastEx;
        static Object m_Lock = new Object();
        static string m_PortName = "";
        static bool m_PortOpen = false;

        static public Exception LastException
        {
            get { return m_LastEx; }
            set { throw (new Exception("Can't modify this value!")); } 
        }

        // This helper allows one port to be open at once
        public static bool OpenPort(ref SSP_COMMAND cmd)
        {
            try
            {
                // Lock critical section
                lock (m_Lock)
                {
                    // If this exact port is already open, return true
                    if (cmd.ComPort == m_PortName && m_PortOpen)
                        return true;

                    // Open port
                    if (m_LibHandle.OpenSSPComPort(cmd))
                    {
                        // Remember details
                        m_PortName = cmd.ComPort;
                        m_PortOpen = true;
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                m_LastEx = ex;
                return false;
            }
        }

        public static void ClosePort()
        {
            try
            {
                lock (m_Lock)
                {
                    // Close the COM port and reset vars
                    m_LibHandle.CloseComPort();
                    m_PortName = "";
                    m_PortOpen = false;
                }
            }
            catch (Exception ex)
            {
                m_LastEx = ex;
            }
        }

        public static bool SendCommand(ref SSP_COMMAND cmd, ref SSP_COMMAND_INFO inf)
        {
            try
            {
                // Lock critical section to prevent multiple commands being sent simultaneously
                lock (m_Lock)
                {
                    return m_LibHandle.SSPSendCommand(cmd, inf);
                }
            }
            catch (Exception ex)
            {
                m_LastEx = ex;
                return false;
            }
        }

        public static bool InitiateKeys(ref SSP_KEYS keys, ref SSP_COMMAND cmd)
        {
            try
            {
                lock (m_Lock)
                {
                    return m_LibHandle.InitiateSSPHostKeys(keys, cmd);
                }
            }
            catch (Exception ex)
            {
                m_LastEx = ex;
                return false;
            }
        }

        public static bool CreateFullKey(ref SSP_KEYS keys)
        {
            try
            {
                lock (m_Lock)
                {
                    return m_LibHandle.CreateSSPHostEncryptionKey(keys);
                }
            }
            catch (Exception ex)
            {
                m_LastEx = ex;
                return false;
            }
        }
    }
}
