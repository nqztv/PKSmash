using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace PKSmash
{
	public class MemoryRow : INotifyPropertyChanged
	{
		private string nameValue;
		public string name
		{
			get { return nameValue; }
			set
			{
				if (value != nameValue)
				{
					nameValue = value;
					OnPropertyChanged("name");
				}
			}
		}

		private string addressValue;
		public string address
		{
			get { return addressValue; }
			set
			{
				if (value != addressValue)
				{
					addressValue = value;
					OnPropertyChanged("address");
				}
			}
		}

		private bool isPointerValue;
		public bool isPointer
		{
			get { return isPointerValue; }
			set
			{
				if (value != isPointerValue)
				{
					isPointerValue = value;
					OnPropertyChanged("isPointer");
				}
			}
		}

		private string offsetValue;
		public string offset
		{
			get { return offsetValue; }
			set
			{
				if (value != offsetValue)
				{
					offsetValue = value;
					OnPropertyChanged("offset");
				}
			}
		}

		private uint lengthValue;
		public uint length
		{
			get { return lengthValue; }
			set
			{
				if (value != lengthValue)
				{
					lengthValue = value;
					OnPropertyChanged("length");
				}
			}
		}

		private string resultValue;
		public string result
		{
			get { return resultValue; }
			set
			{
				if (value != resultValue)
				{
					resultValue = value;
					OnPropertyChanged("result");
				}
			}
		}

		private string typeValue;
		public string type
		{
			get { return typeValue; }
			set
			{
				if (value != typeValue)
				{
					typeValue = value;
					OnPropertyChanged("type");
				}
			}
		}

		private string interpretationValue;
		public string interpretation
		{
			get { return interpretationValue; }
			set
			{
				if (value != interpretationValue)
				{
					interpretationValue = value;
					OnPropertyChanged("interpretation");
				}
			}
		}

		private long timeCostValue;
		public long timeCost
		{
			get { return timeCostValue; }
			set
			{
				if (value != timeCostValue)
				{
					timeCostValue = value;
					OnPropertyChanged("timeCost");
				}
			}
		}

		private bool autoUpdateValue;
		public bool autoUpdate
		{
			get { return autoUpdateValue; }
			set
			{
				if (value != autoUpdateValue)
				{
					autoUpdateValue = value;
					OnPropertyChanged("autoUpdate");
				}
			}
		}
		/*
		public MemoryRow(string n, string a, bool iP, string o, int l)
		{
			this.name = n;
			this.address = a;
			this.isPointer = iP;
			this.offset = o;
			this.length = l;
			this.result = "0";
			this.timeCost = 0;
			this.autoUpdate = true;
		}
		*/
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}


	public static class MemoryRowService
	{
		public static ObservableCollection<MemoryRow> ReadFile(string filePath)
		{
			var lines = File.ReadAllLines(filePath);

			var data = from l in lines.Skip(1)
								 let split = l.Split(',')
								 select new MemoryRow
								 {
									 name = split[0],
									 address = split[1],
									 isPointer = bool.Parse(split[2]),
									 offset = split[3],
									 length = uint.Parse(split[4]),
									 result = split[5],
									 type = split[6],
									 interpretation = split[7],
									 timeCost = long.Parse(split[8]),
									 autoUpdate = bool.Parse(split[9])
								 };

			var oc = new ObservableCollection<MemoryRow>();
			foreach (var row in data)
			{
				oc.Add(row);
			}
			return oc;
		}
	}
}
