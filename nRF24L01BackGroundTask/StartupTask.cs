/*
 
Copyright ® 2017 December devMobile Software, All Rights Reserved
 
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

namespace devmobile.IoTCore.nRF24L01BackGroundTask
{
	public sealed class StartupTask : IBackgroundTask
	{
		// nRF24 Hardware interface configuration
#if CEECH_NRF24L01P_SHIELD
		private const byte ChipEnablePin = 25;
      private const byte ChipSelectPin = 0;
      private const byte InterruptPin = 17;
#endif
#if BOROS_RF2_SHIELD_RADIO_0
		private const byte ChipEnablePin = 24;
      private const byte ChipSelectPin = 0;
      private const byte InterruptPin = 27;
#endif
#if BOROS_RF2_SHIELD_RADIO_1
		private const byte ChipEnablePin = 25;
      private const byte ChipSelectPin = 1;
      private const byte InterruptPin = 22;
#endif
		private const string BaseStationAddress = "Node1";
		private const byte nRF24Channel = 20;
		private RF24 Radio = new RF24();
		private BackgroundTaskDeferral deferral;
		private ThreadPoolTimer timer;


		public void Run(IBackgroundTaskInstance taskInstance)
		{
			Radio.OnDataReceived += Radio_OnDataReceived;
			Radio.OnTransmitFailed += Radio_OnTransmitFailed;
			Radio.OnTransmitSuccess += Radio_OnTransmitSuccess;

			Radio.Initialize(ChipEnablePin, ChipSelectPin, InterruptPin);
			Radio.Address = Encoding.UTF8.GetBytes(BaseStationAddress);
			Radio.Channel = nRF24Channel;
			Radio.PowerLevel = PowerLevel.High;
			Radio.DataRate = DataRate.DR250Kbps;
			Radio.IsEnabled = true;

			Radio.IsAutoAcknowledge = true;
			Radio.IsDyanmicAcknowledge = false;
			Radio.IsDynamicPayload = true;

			Debug.WriteLine("Address: " + Encoding.UTF8.GetString(Radio.Address));
			Debug.WriteLine("PA: " + Radio.PowerLevel);
			Debug.WriteLine("IsAutoAcknowledge: " + Radio.IsAutoAcknowledge);
			Debug.WriteLine("Channel: " + Radio.Channel);
			Debug.WriteLine("DataRate: " + Radio.DataRate);
			Debug.WriteLine("IsDynamicAcknowledge: " + Radio.IsDyanmicAcknowledge);
			Debug.WriteLine("IsDynamicPayload: " + Radio.IsDynamicPayload);
			Debug.WriteLine("IsEnabled: " + Radio.IsEnabled);
			Debug.WriteLine("Frequency: " + Radio.Frequency);
			Debug.WriteLine("IsInitialized: " + Radio.IsInitialized);
			Debug.WriteLine("IsPowered: " + Radio.IsPowered);

			deferral = taskInstance.GetDeferral();

			timer = ThreadPoolTimer.CreatePeriodicTimer(SendMessageTimer, TimeSpan.FromSeconds(20));

			Debug.WriteLine("Start completed");
		}

		void SendMessageTimer(ThreadPoolTimer timer)
		{
			const string DeviceAddress = "Dev01";

			Radio.SendTo(Encoding.UTF8.GetBytes(DeviceAddress), Encoding.UTF8.GetBytes("hello " + DateTime.Now.Second));
		}

		private void Radio_OnDataReceived(byte[] data)
		{
			// Display as Unicode
			string unicodeText = Encoding.UTF8.GetString(data);
			Debug.WriteLine("Unicode - Payload Length {0} Unicode Length {1} Unicode text {2}", data.Length, unicodeText.Length, unicodeText);

			// display as hex
			Debug.WriteLine("Hex - Length {0} Payload {1}", data.Length, BitConverter.ToString(data));
		}

		private void Radio_OnTransmitSuccess()
		{
			Debug.WriteLine("Transmit Succeeded!");
		}

		private void Radio_OnTransmitFailed()
		{
			Debug.WriteLine("Transmit Failed!");
		}
	}
}
