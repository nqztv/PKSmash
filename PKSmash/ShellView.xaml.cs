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
using System.Windows.Input;

namespace PKSmash
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window
	{
		public ShellView()
		{
			InitializeComponent();
		}
	}
}