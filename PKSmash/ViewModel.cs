using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace PKSmash
{
	public class ViewModel : BindableBase
	{
		private readonly Dispatcher currentDispatcher;

		public ObservableCollection<MemoryAddress> MemoryAddresses { get; set; }
		
		public string Delay { get; set; }


		public ICommand PeekCommand { get; private set; }
		private void OnPeek(object arg) { }
		private bool CanPeek(object arg) { return true; }

		public ICommand PokeCommand { get; private set; }
		private void OnPoke(object arg) { }
		private bool CanPoke(object arg) { return true; }

		public ICommand OpenCommand { get; private set; }
		private void OnOpen(object arg) { }
		private bool CanOpen(object arg) { return true; }

		public ICommand SaveCommand { get; private set; }
		private void OnSave(object arg) { }
		private bool CanSave(object arg) { return true; }


		//public string StatusBar
		//{
		//	get { return (string)GetValue(StatusBarProperty); }
		//	set { SetValue(StatusBarProperty, value); }
		//}

		//public static readonly DependencyProperty StatusBarProperty =
		//		DependencyProperty.Register("StatusBar", typeof(string), typeof(ViewModel), new PropertyMetadata(0));


		public ViewModel()
		{
			currentDispatcher = Dispatcher.CurrentDispatcher;

			this.PeekCommand = new DelegateCommand<object>(this.OnPeek, this.CanPeek);
			this.PokeCommand = new DelegateCommand<object>(this.OnPoke, this.CanPoke);
			this.OpenCommand = new DelegateCommand<object>(this.OnOpen, this.CanOpen);
			this.SaveCommand = new DelegateCommand<object>(this.OnSave, this.CanSave);

			this.MemoryAddresses = new ObservableCollection<MemoryAddress>();
		}




	}
}
