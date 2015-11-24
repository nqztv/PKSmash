using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;  // required for ObservableCollection.
using System.Diagnostics;  // required for Stopwatch.
using System.IO;  // required for File.
using System.Threading;  //required for polling.
using System.Threading.Tasks;  // required for polling.
using System.Windows;
using Microsoft.Win32;  // required for file dialogs.
using Newtonsoft.Json;  // required for json handling.

namespace PKSmash
{
	public partial class MainWindow : Window
	{
		USBGecko gecko = new USBGecko();
		ObservableCollection<MemoryAddress> memoryAddresses;
		CancellationTokenSource cancellationTokenSource;
		string inputCSV = @"input.csv";
		string outputJSON = @"memdump.json";
		
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenInput()
		{
			// create OpenFileDialog.
			OpenFileDialog dlg = new OpenFileDialog();

			// set filter for file extension and default file extension to show only csv files.
			dlg.DefaultExt = ".csv";
			dlg.Filter = "CSV Files (*.csv)|*.csv";

			// display OpenFileDialog.
			Nullable<bool> result = dlg.ShowDialog();

			// check if user selected a file. 
			if (result == true)
			{
				// update components.
				inputCSV = dlg.FileName;
				memoryAddresses = MemoryAddressService.CollectFromCSV(inputCSV);
				dbgMemoryAddresses.ItemsSource = memoryAddresses;
				txtStatus.Text = inputCSV + " opened as input.";
      }
		}

		private void SetOutput()
		{
			// create SaveFileDialog.
			SaveFileDialog dlg = new SaveFileDialog();

			// set filter for file extension and default file extension to show only json files.
			dlg.DefaultExt = ".json";
			dlg.Filter = "JSON Files (*.json)|*.json";

			// display SaveFileDialog.
			Nullable<bool> result = dlg.ShowDialog();

			// check if user selected a file. 
			if (result == true)
			{
				// update components.
				outputJSON = dlg.FileName;
				txtStatus.Text = outputJSON + " set as output.";
			}
		}

		private void GetCurrentMemory()
		{
			// declare output for json.
			Dictionary<string, string> output = new Dictionary<string, string>();

			// cycle through each MemoryAddress in collection.
			foreach (MemoryAddress address in memoryAddresses)
			{
				// skip current address if not checked to peek.
				if (!address.DoPeek)
				{
					continue;
				}

				// start timer to determing time cost.
				Stopwatch watch = new Stopwatch();
				watch.Start();

				// try to peek the current address.
				try
				{
					uint addressAsUInt = Convert.ToUInt32(address.Address, 16);
					byte[] response;

          if (address.Offset == "")
					{
						response = gecko.peek(addressAsUInt, address.Length);
					}
					else
					{
						response = gecko.peek(addressAsUInt, 4);
						Array.Reverse(response);
						addressAsUInt = BitConverter.ToUInt32(response, 0);
						addressAsUInt += Convert.ToUInt32(address.Offset, 16);
						response = gecko.peek(addressAsUInt, address.Length);
					}

					address.HexResult = BitConverter.ToString(response).Replace("-", "");
					address.ConvertedResult = Tools.ConvertResult(response, address.Type);
				}
				catch (Exception)
				{
					address.HexResult = "UNABLE TO PEEK!";
				}

				// stop timer and update time cost.
				watch.Stop();
				address.TimeCost = watch.ElapsedMilliseconds;

				// add result to output json.
				output.Add(address.Name, address.ConvertedResult);
			}

			// write json to file.
			File.WriteAllText(outputJSON, JsonConvert.SerializeObject(output, Formatting.Indented));
		}

		private void SetCurrentMemory()
		{
			// cycle through each MemoryAddress in collection.
			foreach (MemoryAddress address in memoryAddresses)
			{
				// skip current address if not checked to peek.
				if (!address.DoPoke)
				{
					continue;
				}

				// skip current address if not acceptable length.
				if (address.Length != 1 && address.Length != 2 && address.Length != 4)
				{
					txtStatus.Text = "poke length must be 1, 2, or 4.";
					continue;
				}

				// start timer to determing time cost.
				Stopwatch watch = new Stopwatch();
				watch.Start();

				// try to poke the current address.
				try
				{
					uint addressAsUInt = Convert.ToUInt32(address.Address, 16);
					uint dataAsUint = Convert.ToUInt32(address.DesiredResult, 16);
					byte[] response;

					if (address.Offset == "")
					{
						gecko.poke(addressAsUInt, address.Length, dataAsUint);
					}
					else
					{
						response = gecko.peek(addressAsUInt, 4);
						Array.Reverse(response);
						addressAsUInt = BitConverter.ToUInt32(response, 0);
						addressAsUInt += Convert.ToUInt32(address.Offset, 16);
						gecko.poke(addressAsUInt, address.Length, dataAsUint);
					}
				}
				catch (Exception)
				{
					address.HexResult = "UNABLE TO POKE!";
				}

				// stop timer and update time cost.
				watch.Stop();
				address.TimeCost = watch.ElapsedMilliseconds;
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Logger.Init();

			// set DataContext.
			this.DataContext = memoryAddresses;

			// set datagrid if default input file exists.
			if (File.Exists(inputCSV))
			{
				memoryAddresses = MemoryAddressService.CollectFromCSV(inputCSV);
				dbgMemoryAddresses.ItemsSource = memoryAddresses;
			}

			// connect usb gecko.
			gecko.Connect();
		}

		private void OpenInput_Click(object sender, RoutedEventArgs e)
		{
			OpenInput();
		}

		private void SetOutput_Click(object sender, RoutedEventArgs e)
		{
			SetOutput();
		}

		private void Peek_Click(object sender, RoutedEventArgs e)
		{
			GetCurrentMemory();
		}

		private void Poke_Click(object sender, RoutedEventArgs e)
		{
			SetCurrentMemory();
		}

		private void AutoPeek_Checked(object sender, RoutedEventArgs e)
		{
			// get delay.
			int delay = 0;

			if (!int.TryParse(txtDelay.Text, out delay))
			{
				return;
			}

			// set cancellation token for task.
			cancellationTokenSource = new CancellationTokenSource();
			var token = cancellationTokenSource.Token;

			// start task to repeatedly peek.
			var listener = Task.Factory.StartNew( () =>
			{
				while (true)
				{
					GetCurrentMemory();
					Thread.Sleep(delay);
					if (token.IsCancellationRequested) { break; }
				}
			}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void AutoPeek_Unchecked(object sender, RoutedEventArgs e)
		{
			// cancel task to repeatedly poke.
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
		}
	}
}
