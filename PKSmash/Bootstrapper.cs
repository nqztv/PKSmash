using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;

namespace PKSmash
{
	public class Bootstrapper : UnityBootstrapper
	{
		protected override DependencyObject CreateShell()
		{
			return Container.Resolve<ShellView>();
		}

		protected override void InitializeShell()
		{
			Application.Current.MainWindow.Show();
		}

		protected override void ConfigureContainer()
		{
			base.ConfigureContainer();

			//Container.RegisterType(typeof(object), typeof(StreamControlView), "StreamControl");
			//Container.RegisterType(typeof(object), typeof(GeckoCommandsView), "GeckoCommands");

			Container.RegisterTypeForNavigation<StreamControlView>("StreamControl");
			Container.RegisterTypeForNavigation<SmashDataView>("SmashData");
			//Container.RegisterTypeForNavigation<GeckoCommandsView>("GeckoCommands");
		}
	}
}
