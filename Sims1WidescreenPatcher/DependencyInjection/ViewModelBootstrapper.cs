﻿using Sims1WidescreenPatcher.Core.Services;
using Sims1WidescreenPatcher.Core.ViewModels;
using Splat;

namespace Sims1WidescreenPatcher.DependencyInjection
{
	public static class ViewModelBootstrapper
	{
		public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
		{
			services.Register(() => new CustomResolutionDialogViewModel());
			services.Register(() => new CustomYesNoDialogViewModel());
			services.Register(() => new MainWindowViewModel(
				resolver.GetService<IResolutionsService>() ?? throw new Exception(),
				resolver.GetService<CustomYesNoDialogViewModel>() ?? throw new Exception(),
				resolver.GetService<CustomResolutionDialogViewModel>() ?? throw new Exception()));
		}
	}
}
