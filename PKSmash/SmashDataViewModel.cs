using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PKSmash
{
	public class SmashDataViewModel : BindableBase
	{
		#region Properties

		USBGecko gecko;
		CancellationTokenSource cancellationTokenSource;

		private bool autoPeek = false;
		public bool AutoPeek
		{
			get { return this.autoPeek; }
			set
			{
				SetProperty(ref this.autoPeek, value);

				if (this.autoPeek)
				{
					// set cancellation token for task.
					cancellationTokenSource = new CancellationTokenSource();
					var token = cancellationTokenSource.Token;

					// start task to repeatedly peek.
					var listener = Task.Factory.StartNew(() =>
					{
						while (true)
						{
							OnPeek();
							Thread.Sleep(delay);
							if (token.IsCancellationRequested) { break; }
						}
					}, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
				}
				else
				{
					// cancel task to repeatedly poke.
					if (cancellationTokenSource != null)
					{
						cancellationTokenSource.Cancel();
					}
				}
			}
		}

		private int delay = 1000;
		public int Delay
		{
			get { return this.delay; }
			set { SetProperty(ref this.delay, value); }
		}

		private string inputFile = "[Select CSV with data]";
		public string InputFile
		{
			get { return this.inputFile; }
			set { SetProperty(ref this.inputFile, value); }
		}

		private string outputFile = "[Select JSON for output]";
		public string OutputFile
		{
			get { return this.outputFile; }
			set { SetProperty(ref this.outputFile, value); }
		}

		private ObservableCollection<SmashDatum> data;
		public ObservableCollection<SmashDatum> Data
		{
			get { return this.data; }
			set { SetProperty(ref this.data, value); }
		}

		#endregion


		#region Commands

		public ICommand PeekCommand { get; private set; }
		private void OnPeek()
		{
			// declare output for json.
			Dictionary<string, string> output = new Dictionary<string, string>();

			// cycle through each MemoryAddress in collection.
			foreach (SmashDatum datum in Data)
			{
				// skip current address if not checked to peek.
				if (!datum.DoPeek)
				{
					continue;
				}

				string previousResult;
				previousResult = datum.ConvertedResult;

				// start timer to determing time cost.
				Stopwatch watch = new Stopwatch();
				watch.Start();

				// try to peek the current address.
				try
				{
					uint addressAsUInt = Convert.ToUInt32(datum.Address, 16);
					byte[] response;

					if (datum.Offset == "")
					{
						response = gecko.peek(addressAsUInt, datum.Length);
					}
					else
					{
						response = gecko.peek(addressAsUInt, 4);
						Array.Reverse(response);
						addressAsUInt = BitConverter.ToUInt32(response, 0);
						addressAsUInt += Convert.ToUInt32(datum.Offset, 16);
						response = gecko.peek(addressAsUInt, datum.Length);
					}

					datum.HexResult = BitConverter.ToString(response).Replace("-", "");
					datum.ConvertedResult = Utils.ConvertResult(response, datum.Type);
				}
				catch (Exception)
				{
					datum.HexResult = "UNABLE TO PEEK!";
				}

				// stop timer and update time cost.
				watch.Stop();
				datum.TimeCost = watch.ElapsedMilliseconds;

				// add result to output json.
				output.Add(datum.Name, datum.ConvertedResult);

				if (datum.Name == "GameWinner" && previousResult.Length == 16 && datum.ConvertedResult.Length == 16)
				{
					if (previousResult.Substring(0, 8) != "00000300" && datum.ConvertedResult.Substring(0, 8) == "00000300")
					{
						this.eventAggregator.GetEvent<WinEvent>().Publish(datum.ConvertedResult.Substring(8, 2));
					}
				}

				if (datum.Name == "P1Character" && previousResult != datum.ConvertedResult)
				{
					this.eventAggregator.GetEvent<P1CharacterChangeEvent>().Publish(datum.ConvertedResult);
				}


				if (datum.Name == "P2Character" && previousResult != datum.ConvertedResult)
				{
					this.eventAggregator.GetEvent<P2CharacterChangeEvent>().Publish(datum.ConvertedResult);
				}


				if (datum.Name == "P3Character" && previousResult != datum.ConvertedResult)
				{
					this.eventAggregator.GetEvent<P3CharacterChangeEvent>().Publish(datum.ConvertedResult);
				}


				if (datum.Name == "P4Character" && previousResult != datum.ConvertedResult)
				{
					this.eventAggregator.GetEvent<P4CharacterChangeEvent>().Publish(datum.ConvertedResult);
				}
			}

			// write json to file.
			File.WriteAllText(OutputFile, JsonConvert.SerializeObject(output, Formatting.Indented));
		}
		private bool CanPeek()
		{
			return true;
		}

		public ICommand PokeCommand { get; private set; }
		private void OnPoke()
		{
			// cycle through each MemoryAddress in collection.
			foreach (SmashDatum datum in Data)
			{
				// skip current address if not checked to peek.
				if (!datum.DoPoke)
				{
					continue;
				}

				// skip current address if not acceptable length.
				if (datum.Length != 1 && datum.Length != 2 && datum.Length != 4)
				{
					//txtStatus.Text = "poke length must be 1, 2, or 4.";
					continue;
				}

				// start timer to determing time cost.
				Stopwatch watch = new Stopwatch();
				watch.Start();

				// try to poke the current address.
				try
				{
					uint addressAsUInt = Convert.ToUInt32(datum.Address, 16);
					uint dataAsUint = Convert.ToUInt32(datum.DesiredResult, 16);
					byte[] response;

					if (datum.Offset == "")
					{
						gecko.poke(addressAsUInt, datum.Length, dataAsUint);
					}
					else
					{
						response = gecko.peek(addressAsUInt, 4);
						Array.Reverse(response);
						addressAsUInt = BitConverter.ToUInt32(response, 0);
						addressAsUInt += Convert.ToUInt32(datum.Offset, 16);
						gecko.poke(addressAsUInt, datum.Length, dataAsUint);
					}
				}
				catch (Exception)
				{
					datum.HexResult = "UNABLE TO POKE!";
				}

				// stop timer and update time cost.
				watch.Stop();
				datum.TimeCost = watch.ElapsedMilliseconds;
			}
		}
		private bool CanPoke()
		{
			return true;
		}

		public ICommand OpenInputCommand { get; private set; }
		private void OnOpenInput()
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".csv";
			dlg.Filter = "CSV Files (*.csv)|*.csv";
			Nullable<bool> result = dlg.ShowDialog();

			if (result == true)
			{
				InputFile = dlg.FileName;
			}

			// grab each row from a csv file.
      string[] rows = File.ReadAllLines(InputFile);

			// convert each row to a MemoryAddress object.
			var linqdata = from row in rows.Skip(1)
								 let column = row.Split(',')
								 select new SmashDatum
								 {
									 Name = column[0],
									 Type = column[1],
									 Address = column[2],
									 Offset = column[3],
									 Length = uint.Parse(column[4]),
									 HexResult = column[5],
									 ConvertedResult = column[6],
									 DesiredResult = column[7],
									 TimeCost = long.Parse(column[8]),
									 DoPeek = bool.Parse(column[9]),
									 DoPoke = bool.Parse(column[10])
								 };

			// convert linq query to an ObservableCollection.
			Data = new ObservableCollection<SmashDatum>(linqdata);
		}
		private bool CanOpenInput()
		{
			return true;
		}

		public ICommand SaveOutputCommand { get; private set; }
		private void OnSaveOutput()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.DefaultExt = ".json";
			dlg.Filter = "JSON Files (*.json)|*.json";
			Nullable<bool> result = dlg.ShowDialog();

			if (result == true)
			{
				OutputFile = dlg.FileName;
			}
		}
		private bool CanSaveOutput()
		{
			return true;
		}

		#endregion


		#region Constructor

		private IEventAggregator eventAggregator;

		public SmashDataViewModel(IEventAggregator eventAggregator)
		{
			this.eventAggregator = eventAggregator;

			this.PeekCommand = new DelegateCommand(this.OnPeek, this.CanPeek);
			this.PokeCommand = new DelegateCommand(this.OnPoke, this.CanPoke);
			this.OpenInputCommand = new DelegateCommand(this.OnOpenInput, this.CanOpenInput);
			this.SaveOutputCommand = new DelegateCommand(this.OnSaveOutput, this.CanSaveOutput);

			gecko = new USBGecko();
			gecko.Connect();
		}

		#endregion

	}
}
