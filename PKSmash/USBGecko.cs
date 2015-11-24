using System;
using FTD2XX_NET;

namespace PKSmash
{
	public class USBGecko
	{
		FTDI ftdiDevice = new FTDI();
		FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
		bool isConnected = false;

		private bool Initialize()
		{
			Logger.Log("INITIALIZING USB GECKO.");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			const uint FT_PURGE_RX = 1;
			const uint FT_PURGE_TX = 2;
			
			// reset device
			ftStatus = this.ftdiDevice.ResetDevice();
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("resetting device.");
			}
			else
			{
				Logger.Log("FAILED to reset device.");
				Disconnect();
				return false;
			}

			// purge rx buffers
			ftStatus = this.ftdiDevice.Purge(FT_PURGE_RX);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("purged rx buffers.");
			}
			else
			{
				Logger.Log("FAILED to purge rx buffers.");
				Disconnect();
				return false;
			}

			// purge tx buffers
			ftStatus = this.ftdiDevice.Purge(FT_PURGE_TX);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("purged tx buffers.");
			}
			else
			{
				Logger.Log("FAILED to purge tx buffers.");
				Disconnect();
				return false;
			}

			return true;
		}

		public void Connect()
		{
			Logger.Log("CONNECTING USB GECKO.");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			// determine the number of ftdi devices connected to the machine.
			uint ftdiDeviceCount = 0;
			ftStatus = this.ftdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Number of FTDI devices: " + ftdiDeviceCount.ToString());
			}
			else
			{
				Logger.Log("FAILED to get number of devices (error " + ftStatus.ToString() + ")");
				Disconnect();
				return;
			}

			// if no devices available, return
			if (ftdiDeviceCount == 0)
			{
				Logger.Log("FAILED because no devices returned.");
				Disconnect();
				return;
			}

			// allocate storage for device info list
			FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

			// populate our device list
			ftStatus = this.ftdiDevice.GetDeviceList(ftdiDeviceList);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				for (uint i = 0; i < ftdiDeviceCount; i++)
				{
					Logger.Log("Device Index: " + i.ToString());
					Logger.Log("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
					Logger.Log("Type: " + ftdiDeviceList[i].Type.ToString());
					Logger.Log("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
					Logger.Log("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
					Logger.Log("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
					Logger.Log("Description: " + ftdiDeviceList[i].Description.ToString());
				}
			}
			else
			{
				Logger.Log("FAILED to get device list.");
				Disconnect();
				return;
			}

			// open first device in our list by serial number
			ftStatus = this.ftdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("opened device with serial number " + ftdiDeviceList[0].SerialNumber);
			}
			else
			{
				Logger.Log("FAILED to open device (error " + ftStatus.ToString() + ")");
				Disconnect();
				return;
			}

			//// Set up device data parameters
			//// Set Baud rate to 9600
			//ftStatus = myFtdiDevice.SetBaudRate(9600);
			//if (ftStatus != FTDI.FT_STATUS.FT_OK)
			//{
			//	// Wait for a key press
			//	Logger.Log("Failed to set Baud rate (error " + ftStatus.ToString() + ")");
			//	Console.ReadKey();
			//	return;
			//}

			//// Set data characteristics - Data bits, Stop bits, Parity
			//ftStatus = myFtdiDevice.SetDataCharacteristics(FTDI.FT_DATA_BITS.FT_BITS_8, FTDI.FT_STOP_BITS.FT_STOP_BITS_1, FTDI.FT_PARITY.FT_PARITY_NONE);
			//if (ftStatus != FTDI.FT_STATUS.FT_OK)
			//{
			//	// Wait for a key press
			//	Logger.Log("Failed to set data characteristics (error " + ftStatus.ToString() + ")");
			//	Console.ReadKey();
			//	return;
			//}

			//// Set flow control - set RTS/CTS flow control
			//ftStatus = myFtdiDevice.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x11, 0x13);
			//if (ftStatus != FTDI.FT_STATUS.FT_OK)
			//{
			//	// Wait for a key press
			//	Logger.Log("Failed to set flow control (error " + ftStatus.ToString() + ")");
			//	Console.ReadKey();
			//	return;
			//}

			// set read timeout to 1 seconds, write timeout to 1 second
			ftStatus = this.ftdiDevice.SetTimeouts(1000, 1000);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Set Timeouts to 5 seconds for reads and infinite for writes.");
			}
			else
			{
				Logger.Log("FAILED to set timeouts (error " + ftStatus.ToString() + ")");
				Disconnect();
				return;
			}

			// set latency timer to minimum of 2ms.
			byte latencyTimer = 2;
			ftStatus = this.ftdiDevice.SetLatency(latencyTimer);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Set latency timer to 2ms.");
			}
			else
			{
				Logger.Log("FAILED to set latency timer.");
				Disconnect();
				return;
			}

			// set transfer rate from default of 4096 bytes to max 64k.
			uint transferSize = 65536;
			ftStatus = this.ftdiDevice.InTransferSize(transferSize);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Set transfer size to max 64kb.");
			}
			else
			{
				Logger.Log("FAILED to set transfer size");
				Disconnect();
				return;
			}

			// initialise usb gecko
			if (Initialize())
			{
				this.isConnected = true;
				return;
			}
			else
			{
				Logger.Log("FAILED to initialize ftdi device.");
				Disconnect();
				return;
			}
		}

		public void Disconnect()
		{
			Logger.Log("DISCONNECTING USB GECKO.");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			ftStatus = this.ftdiDevice.Close();
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("ftdi device closed.");
				this.isConnected = false;
			}
			else
			{
				Logger.Log("FAILED to close ftdi device");
			}
		}

		private FTDI.FT_STATUS ftdiRead(byte[] dataBuffer, uint numBytesToRead)
		{
			Logger.Log("called ftdiRead(byte[] " + BitConverter.ToString(dataBuffer) + ", uint " + numBytesToRead.ToString() + ").");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			uint numBytesRead = 0;

			ftStatus = this.ftdiDevice.Read(dataBuffer, numBytesToRead, ref numBytesRead);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("ftdiRead successful with dataBuffer = " + BitConverter.ToString(dataBuffer) + ".");

				// check if bytes were lost during reception.
				//if (numBytesRead != numBytesToRead)
				//{
				//	Logger.Log("FAILED ftdiRead because " + numBytesRead.ToString() + " != " + numBytesToRead.ToString() + ".");
				//}
			}
			else
			{
				Logger.Log("FAILED ftdiRead because of a fatal error.");
			}

			return ftStatus;
		}

		private FTDI.FT_STATUS ftdiWrite(byte[] dataBuffer, uint numBytesToWrite)
		{
			Logger.Log("called ftdiWrite(byte[] " + BitConverter.ToString(dataBuffer) + ", uint " + numBytesToWrite.ToString() + ").");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			uint numBytesWritten = 0;

			ftStatus = this.ftdiDevice.Write(dataBuffer, numBytesToWrite, ref numBytesWritten);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("ftdiWrite successful with dataBuffer = " + BitConverter.ToString(dataBuffer) + ".");

				// check if bytes were lost during transmission.
				if (numBytesWritten != numBytesToWrite)
				{
					Logger.Log("FAILED ftdiWrite because " + numBytesWritten.ToString() + " != " + numBytesToWrite.ToString() + ".");
				}
			}
			else
			{
				Logger.Log("FAILED ftdiWrite because of a fatal error.");
			}

			return ftStatus;
		}

		public byte[] peek(uint address, uint length)
		{
			Logger.Log("called peek at address " + address.ToString("X") + " with length " + length.ToString() + ".");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			// reset connection
			Initialize();

			// get start and end address and put them in powerpc endianness.
			ulong startAddress = address;
			ulong endAddress = address + length;
			ulong memRange = Tools.ReverseBytes((startAddress << 32) + endAddress);

			// set necessary packets
			byte[] cmdRead = { 4 };
			byte[] ack = { 170 };
			byte[] memRangeAsBytes = BitConverter.GetBytes(memRange);
			byte[] response = new Byte[length];
			byte[] emptyResponse = { 0 };

			// transmit readmem command to gecko.
			ftStatus = ftdiWrite(cmdRead, 1);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("readmem command sent to gecko successfully.");
			}
			else
			{
				Logger.Log("FAILED to send readmem command to gecko.");
				return emptyResponse;
			}

			// receive ack from gecko.
			byte[] ackResponse = new Byte[1];
			ftStatus = ftdiRead(ackResponse, 1);
			if (ftStatus == FTDI.FT_STATUS.FT_OK && BitConverter.ToString(ackResponse) == BitConverter.ToString(ack))
			{
				Logger.Log("response from readmem command received from gecko successfully.");
			}
			else
			{
				Logger.Log("FAILED to receive response from readmem command.");
				return emptyResponse;
			}

			// send memory range for the readmem command.
			ftStatus = ftdiWrite(memRangeAsBytes, 8);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Memory range for readmem command sent to gecko successfully.");
			}
			else
			{
				Logger.Log("FAILED to send memory range for the readmem command.");
				return emptyResponse;
			}

			// get memory values from given range.
			ftStatus = ftdiRead(response, length);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Response from readmem command received from gecko successfully.");
			}
			else
			{
				Logger.Log("Failed to receive response from readmem command.");
				return emptyResponse;
			}

			// send ack to gecko.
			ftStatus = ftdiWrite(ack, 1);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("ack packet sent.");
			}
			else
			{
				Logger.Log("FAILED to send ack pack.");
				return emptyResponse;
			}

			//Array.Reverse(response);
			return response;
		}

		public void poke(uint address, uint length, uint data)
		{
			Logger.Log("called poke at address " + address.ToString("X") + " with length " + length.ToString() + " and data " + data + ".");
			ftStatus = FTDI.FT_STATUS.FT_OK;

			// reset connection
			Initialize();

			// get start and end address and put them in powerpc endianness.
			ulong writeAddress = address;
			ulong writeData = data;
			ulong addressAndData = Tools.ReverseBytes((writeAddress << 32) + data);

			// set necessary packets
			byte[] response = new Byte[length];
			byte[] ack = { 170 };
			byte[] addressAndDataAsBytes = BitConverter.GetBytes(addressAndData);
			byte[] cmdWrite = { 3 };
			switch (length)
			{
				case 1:
					cmdWrite[0] = 1;
					break;
				case 2:
					cmdWrite[0] = 2;
					break;
				case 4:
					cmdWrite[0] = 3;
					break;
				default:
					return;
			}
			
			// transmit writemem command to gecko.
			ftStatus = ftdiWrite(cmdWrite, 1);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("writemem command sent to gecko successfully.");
			}
			else
			{
				Logger.Log("FAILED to send writemem command to gecko.");
				return;
			}
			
			// send address and data for the writemem command.
			ftStatus = ftdiWrite(addressAndDataAsBytes, 8);
			if (ftStatus == FTDI.FT_STATUS.FT_OK)
			{
				Logger.Log("Address and Data for readmem command sent to gecko successfully.");
			}
			else
			{
				Logger.Log("FAILED to send Address and Data for the readmem command.");
				return;
			}

			return;
		}
	}
}