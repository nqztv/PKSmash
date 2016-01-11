using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKSmash
{
	public class SmashDatum : BindableBase
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

		// constructor
		//public SmashDatum(string name, string type, string address, string offset, uint length, string hexResult, string convertedResult, string desiredResult, long timeCost, bool doPeek, bool doPoke)
		//{
		//	this.name = name;
		//	this.type = type;
		//	this.address = address;
		//	this.offset = offset;
		//	this.length = length;
		//	this.hexResult = hexResult;
		//	this.convertedResult = convertedResult;
		//	this.desiredResult = desiredResult;
		//	this.timeCost = timeCost;
		//	this.doPeek = doPeek;
		//	this.doPoke = doPoke;
		//}

		// declare public properties to expose the private fields with accessors.
		public string Name
		{
			get { return this.name; }
			set { SetProperty(ref this.name, value); }
		}

		public string Type
		{
			get { return this.type; }
			set { SetProperty(ref this.type, value); }
		}

		public string Address
		{
			get { return this.address; }
			set { SetProperty(ref this.address, value); }
		}

		public string Offset
		{
			get { return this.offset; }
			set { SetProperty(ref this.offset, value); }
		}

		public uint Length
		{
			get { return this.length; }
			set { SetProperty(ref this.length, value); }
		}

		public string HexResult
		{
			get { return this.hexResult; }
			set { SetProperty(ref this.hexResult, value); }
		}

		public string ConvertedResult
		{
			get { return this.convertedResult; }
			set { SetProperty(ref this.convertedResult, value); }
		}

		public string DesiredResult
		{
			get { return this.desiredResult; }
			set { SetProperty(ref this.desiredResult, value); }
		}

		public long TimeCost
		{
			get { return this.timeCost; }
			set { SetProperty(ref this.timeCost, value); }
		}

		public bool DoPeek
		{
			get { return this.doPeek; }
			set { SetProperty(ref this.doPeek, value); }
		}

		public bool DoPoke
		{
			get { return this.doPoke; }
			set { SetProperty(ref this.doPoke, value); }
		}
	}
}
