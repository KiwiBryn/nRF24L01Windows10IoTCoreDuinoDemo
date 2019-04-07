/*
 
Copyright ® 2019 April devMobile Software, All Rights Reserved
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
PURPOSE.
 
http://www.devmobile.co.nz
 
*/
using System;
using System.Diagnostics;
using System.Text;
using Radios.RF24;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace devmobile.IoTCore.BorosRf2BackGroundTask
{
	public sealed class StartupTask : IBackgroundTask
	{
		// nRF24 Hardware interface configuration
		private const byte ChipEnablePin0 = 24;
		private const byte ChipSelectPin0 = 0;
		private const byte InterruptPin0 = 27;
		private const string BaseStationAddress0 = "Node0";
		private const string DeviceAddress0 = "Node1";
		private const byte nRF24Channel0 = 20;
		private RF24 Radio0 = new RF24();
		private ThreadPoolTimer timer0;

		private const byte ChipEnablePin1 = 25;
		private const byte ChipSelectPin1 = 1;
		private const byte InterruptPin1 = 22;
		private const string BaseStationAddress1 = "Node1";
		private const string DeviceAddress1 = "Node0";
		private const byte nRF24Channel1 = 20;
		private RF24 Radio1 = new RF24();
		private ThreadPoolTimer timer1;

		private BackgroundTaskDeferral deferral;

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			Radio0.OnDataReceived += Radio0_OnDataReceived;
			Radio0.OnTransmitFailed += Radio0_OnTransmitFailed;
			Radio0.OnTransmitSuccess += Radio0_OnTransmitSuccess;

			Radio0.Initialize(ChipEnablePin0, ChipSelectPin0, InterruptPin0);
			Radio0.Address = Encoding.UTF8.GetBytes(BaseStationAddress0);
			Radio0.Channel = nRF24Channel0;
			Radio0.PowerLevel = PowerLevel.High;
			Radio0.DataRate = DataRate.DR250Kbps;
			Radio0.IsEnabled = true;

			Radio0.IsAutoAcknowledge = true;
			Radio0.IsDyanmicAcknowledge = false;
			Radio0.IsDynamicPayload = true;

			Debug.WriteLine("Address: " + Encoding.UTF8.GetString(Radio0.Address));
			Debug.WriteLine("PA: " + Radio0.PowerLevel);
			Debug.WriteLine("IsAutoAcknowledge: " + Radio0.IsAutoAcknowledge);
			Debug.WriteLine("Channel: " + Radio0.Channel);
			Debug.WriteLine("DataRate: " + Radio0.DataRate);
			Debug.WriteLine("IsDynamicAcknowledge: " + Radio0.IsDyanmicAcknowledge);
			Debug.WriteLine("IsDynamicPayload: " + Radio0.IsDynamicPayload);
			Debug.WriteLine("IsEnabled: " + Radio0.IsEnabled);
			Debug.WriteLine("Frequency: " + Radio0.Frequency);
			Debug.WriteLine("IsInitialized: " + Radio0.IsInitialized);
			Debug.WriteLine("IsPowered: " + Radio0.IsPowered);
			Debug.WriteLine("");

			timer0 = ThreadPoolTimer.CreatePeriodicTimer(SendMessageTimer0, TimeSpan.FromSeconds(5));

			Radio1.OnDataReceived += Radio1_OnDataReceived;
			Radio1.OnTransmitFailed += Radio1_OnTransmitFailed;
			Radio1.OnTransmitSuccess += Radio1_OnTransmitSuccess;

			Radio1.Initialize(ChipEnablePin1, ChipSelectPin1, InterruptPin1);
			Radio1.Address = Encoding.UTF8.GetBytes(BaseStationAddress1);
			Radio1.Channel = nRF24Channel1;
			Radio1.PowerLevel = PowerLevel.High;
			Radio1.DataRate = DataRate.DR250Kbps;
			Radio1.IsEnabled = true;

			Radio1.IsAutoAcknowledge = true;
			Radio1.IsDyanmicAcknowledge = false;
			Radio1.IsDynamicPayload = true;

			Debug.WriteLine("Address: " + Encoding.UTF8.GetString(Radio1.Address));
			Debug.WriteLine("PA: " + Radio1.PowerLevel);
			Debug.WriteLine("IsAutoAcknowledge: " + Radio1.IsAutoAcknowledge);
			Debug.WriteLine("Channel: " + Radio1.Channel);
			Debug.WriteLine("DataRate: " + Radio1.DataRate);
			Debug.WriteLine("IsDynamicAcknowledge: " + Radio1.IsDyanmicAcknowledge);
			Debug.WriteLine("IsDynamicPayload: " + Radio1.IsDynamicPayload);
			Debug.WriteLine("IsEnabled: " + Radio1.IsEnabled);
			Debug.WriteLine("Frequency: " + Radio1.Frequency);
			Debug.WriteLine("IsInitialized: " + Radio1.IsInitialized);
			Debug.WriteLine("IsPowered: " + Radio1.IsPowered);
			Debug.WriteLine("");

			timer1 = ThreadPoolTimer.CreatePeriodicTimer(SendMessageTimer1, TimeSpan.FromSeconds(5));

			deferral = taskInstance.GetDeferral();

			Debug.WriteLine("Start completed");
		}

		void SendMessageTimer0(ThreadPoolTimer timer)
		{
			Radio0.SendTo(Encoding.UTF8.GetBytes(DeviceAddress0), Encoding.UTF8.GetBytes("hello from 0 " + DateTime.Now.Second));
		}

		void SendMessageTimer1(ThreadPoolTimer timer)
		{
			Radio1.SendTo(Encoding.UTF8.GetBytes(DeviceAddress1), Encoding.UTF8.GetBytes("hello from 1" + DateTime.Now.Second));
		}

		private void Radio0_OnDataReceived(byte[] data)
		{
			// Display as Unicode
			string unicodeText = Encoding.UTF8.GetString(data);
			Debug.WriteLine("Unicode 0 - Payload Length {0} Unicode Length {1} Unicode text {2}", data.Length, unicodeText.Length, unicodeText);

			// display as hex
			Debug.WriteLine("Hex - Length {0} Payload {1}", data.Length, BitConverter.ToString(data));
		}

		private void Radio0_OnTransmitSuccess()
		{
			Debug.WriteLine("Transmit 0 Succeeded!");
		}

		private void Radio0_OnTransmitFailed()
		{
			Debug.WriteLine("Transmit 0 Failed!");
		}

		private void Radio1_OnDataReceived(byte[] data)
		{
			// Display as Unicode
			string unicodeText = Encoding.UTF8.GetString(data);
			Debug.WriteLine("Unicode 1 - Payload Length {0} Unicode Length {1} Unicode text {2}", data.Length, unicodeText.Length, unicodeText);

			// display as hex
			Debug.WriteLine("Hex - Length {0} Payload {1}", data.Length, BitConverter.ToString(data));
		}

		private void Radio1_OnTransmitSuccess()
		{
			Debug.WriteLine("Transmit 1 Succeeded!");
		}

		private void Radio1_OnTransmitFailed()
		{
			Debug.WriteLine("Transmit 1 Failed!");
		}
	}
}
