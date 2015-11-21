using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FTD2XX_NET;
using Microsoft.Win32;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.IO;

namespace PKSmash
{
	public partial class MainWindow : Window
	{
		USBGecko gecko = new USBGecko();
		ObservableCollection<MemoryRow> MemoryRows;
		CancellationTokenSource cancellationTokenSource;
		string outputJSON;
		
		public MainWindow()
		{
			InitializeComponent();
		}

		private string interpretResult(byte[] input, string type)
		{
			string interpretation = "";

			switch (type)
			{
				case "float":
					interpretation = BitConverter.ToSingle(input, 0).ToString();
					break;
				case "uint":
					interpretation = BitConverter.ToUInt32(input, 0).ToString();
					break;
				default:
					interpretation = BitConverter.ToString(input).Replace("-", "");
					break;
			}

			return interpretation;
		}

		private void getCurrentMemoryValues()
		{
			Dictionary<string, string> output = new Dictionary<string, string>();

			foreach (MemoryRow row in MemoryRows)
			{
				if (!row.autoUpdate)
				{
					continue;
				}

				Stopwatch watch = new Stopwatch();
				watch.Start();

				try
				{
					uint addressAsUInt = Convert.ToUInt32(row.address, 16);
					byte[] response = gecko.peek(addressAsUInt, row.length);

					if (row.isPointer)
					{
						Array.Reverse(response);
						addressAsUInt = BitConverter.ToUInt32(response, 0);
					}

					if (row.offset != "0")
					{
						addressAsUInt += Convert.ToUInt32(row.offset, 16);
						response = gecko.peek(addressAsUInt, row.length);
					}

					row.result = BitConverter.ToString(response).Replace("-", "");
					row.interpretation = interpretResult(response, row.type);
				}
				catch (Exception)
				{
					row.result = "ERROR";
				}

				output.Add(row.name, row.interpretation);

				watch.Stop();

				row.timeCost = watch.ElapsedMilliseconds;
			}

			File.WriteAllText(@"memdump.json", JsonConvert.SerializeObject(output, Formatting.Indented));
		}


		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			MemoryRows = MemoryRowService.ReadFile("input.csv");
			this.DataContext = MemoryRows;

			gecko.Connect();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Create OpenFileDialog 
			OpenFileDialog dlg = new OpenFileDialog();

			// Set filter for file extension and default file extension 
			dlg.DefaultExt = ".csv";
			dlg.Filter = "CSV Files (*.csv)|*.csv";

			// Display OpenFileDialog by calling ShowDialog method 
			Nullable<bool> result = dlg.ShowDialog();


			// Get the selected file name and display in a TextBox 
			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				statusBar.Text = filename;
				MemoryRows = MemoryRowService.ReadFile(filename);
				memDataGrid.ItemsSource = MemoryRows;
			}
		}

		private void autoUpdate_Checked(object sender, RoutedEventArgs e)
		{
			int delay = 0;

			if (!int.TryParse(msDelay.Text, out delay))
			{
				return;
			}

			cancellationTokenSource = new CancellationTokenSource();
			var token = cancellationTokenSource.Token;

			var listener = Task.Factory.StartNew( () =>
			{
				while (true)
				{
					getCurrentMemoryValues();
					Thread.Sleep(delay);
					if (token.IsCancellationRequested)
					{
						break;
					}
				}
			}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		private void autoUpdate_Unchecked(object sender, RoutedEventArgs e)
		{
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
		}

		private void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			getCurrentMemoryValues();
		}
	}
}
