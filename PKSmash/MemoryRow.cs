using System.Collections.ObjectModel;  // required for ObservableCollection.
using System.ComponentModel;  // required for the INotifyPropertyChanged interface.
using System.IO;  // required for File.
using System.Linq;  // requred to run linq queries.

namespace PKSmash
{
	public class MemoryAddress : INotifyPropertyChanged
	{
		// declare private fields.
		private string name;
		private string type;
		private string address;
		private string offset;
		private uint length;
		private string hexResult;
		private string convertedResult;
		private string desiredResult;
		private long timeCost;
		private bool doPeek;
		private bool doPoke;

		// declare public properties to expose the private fields with accessors.
		public string Name
		{
			get { return name; }
			set
			{
				if (name != value)
				{
					name = value;
					OnPropertyChanged("name");
				}
			}
		}

		public string Type
		{
			get { return type; }
			set
			{
				if (type != value)
				{
					type = value;
					OnPropertyChanged("type");
				}
			}
		}

		public string Address
		{
			get { return address; }
			set
			{
				if (address != value)
				{
					address = value;
					OnPropertyChanged("address");
				}
			}
		}

		public string Offset
		{
			get { return offset; }
			set
			{
				if (offset != value)
				{
					offset = value;
					OnPropertyChanged("offset");
				}
			}
		}

		public uint Length
		{
			get { return length; }
			set
			{
				if (length != value)
				{
					length = value;
					OnPropertyChanged("length");
				}
			}
		}

		public string HexResult
		{
			get { return hexResult; }
			set
			{
				if (hexResult != value)
				{
					hexResult = value;
					OnPropertyChanged("hexResult");
				}
			}
		}

		public string ConvertedResult
		{
			get { return convertedResult; }
			set
			{
				if (convertedResult != value)
				{
					convertedResult = value;
					OnPropertyChanged("convertedResult");
				}
			}
		}

		public string DesiredResult
		{
			get { return desiredResult; }
			set
			{
				if (desiredResult != value)
				{
					desiredResult = value;
					OnPropertyChanged("desiredResult");
				}
			}
		}

		public long TimeCost
		{
			get { return timeCost; }
			set
			{
				if (timeCost != value)
				{
					timeCost = value;
					OnPropertyChanged("timeCost");
				}
			}
		}

		public bool DoPeek
		{
			get { return doPeek; }
			set
			{
				if (doPeek != value)
				{
					doPeek = value;
					OnPropertyChanged("doPeek");
				}
			}
		}

		public bool DoPoke
		{
			get { return doPoke; }
			set
			{
				if (doPoke != value)
				{
					doPoke = value;
					OnPropertyChanged("doPoke");
				}
			}
		}

		// declare an event for when a private field changes.
		public event PropertyChangedEventHandler PropertyChanged;

		// declare a method to raise an event when a private field changes.
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public static class MemoryAddressService
	{
		public static ObservableCollection<MemoryAddress> CollectFromCSV(string filePath)
		{
			// grab each row from a csv file.
			string[] rows = File.ReadAllLines(filePath);

			// convert each row to a MemoryAddress object.
			var data = from row in rows.Skip(1)
								 let column = row.Split(',')
								 select new MemoryAddress
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
			ObservableCollection<MemoryAddress> oc = new ObservableCollection<MemoryAddress>(data);

			// return the ObservableCollection.
			return oc;
		}
	}
}
