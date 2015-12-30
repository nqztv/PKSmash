using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace PKSmash
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
			{
				var viewName = viewType.FullName;
				var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
				var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName.Remove(viewName.Length - 4), viewAssemblyName);
				return Type.GetType(viewModelName);
			});

			var bs = new Bootstrapper();
			bs.Run();
		}
	}
}
