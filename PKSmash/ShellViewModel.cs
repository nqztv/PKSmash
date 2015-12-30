using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKSmash
{
	public class ShellViewModel : BindableBase
	{
		private readonly IRegionManager regionManager;

		public DelegateCommand<string> NavigateCommand { get; set; }

		public ShellViewModel(IRegionManager regionManager)
		{
			this.regionManager = regionManager;

			NavigateCommand = new DelegateCommand<string>(Navigate);
		}

		private void Navigate(string uri)
		{
			this.regionManager.RequestNavigate("ContentRegion", uri);
		}
	}
}
