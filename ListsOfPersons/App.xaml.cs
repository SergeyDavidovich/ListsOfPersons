﻿using Windows.UI.Xaml;
using System.Threading.Tasks;
using ListsOfPersons.Helpers.SettingsHelper;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using GalaSoft.MvvmLight.Ioc;
using RepositoryService;
using DomenModel.Models;
using System;
using ListsOfPersons.Views;
using System.Linq;
using ListsOfPersons.ViewModels;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Controls;
using Template10.Services.NavigationService;
using ListsOfPersons.Helpers.DialogHelper;
using ListsOfPersons.Helpers.TileHelper;
using Windows.UI.StartScreen;

namespace ListsOfPersons
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region app settings

            // some settings must be set in app.constructor
            var settings = SettingsHelper.Instance;
            RequestedTheme = settings.AppTheme;
            CacheMaxDuration = settings.CacheMaxDuration;
            ShowShellBackButton = settings.UseShellBackButton;

            #endregion
        }

        public override UIElement CreateRootElement(IActivatedEventArgs e)
        {
            var service = NavigationServiceFactory(BackButton.Attach, ExistingContent.Exclude);
            return new ModalDialog
            {
                DisableBackButtonWhenModal = true,
                Content = new Views.Shell(service),
                ModalContent = new Views.Busy(),
            };
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // Registering dependency in container
            SimpleIoc.Default.Register<MasterDetailPageViewModel>();
            SimpleIoc.Default.Register<IRepositoryService<Person>, PersonsRepositoryServiceFake>();
            SimpleIoc.Default.Register<AddEditPageViewModel>();
            SimpleIoc.Default.Register<FavoritePageViewModel>();
            SimpleIoc.Default.Register<IDialogHelper, PersonDialogHelper>();
            SimpleIoc.Default.Register<PersonContentDialogViewModel>();
            SimpleIoc.Default.Register<ITileHelper, PersonTileHelper>();
            SimpleIoc.Default.Register<PersonTileViewViewModel>();

            var arg = (LaunchActivatedEventArgs)args;
            string argument = arg.Arguments;

            if (!string.IsNullOrEmpty(argument))
                await NavigationService.NavigateAsync(typeof(Views.AddEditPage), argument);

            // TODO: add your long-running task here
            await NavigationService.NavigateAsync(typeof(Views.MasterDetailPage),argument);
        }

        public override INavigable ResolveForPage(Page page, NavigationService navigationService)
        {
            if (page is MasterDetailPage)
                return SimpleIoc.Default.GetInstance<MasterDetailPageViewModel>();
            else if (page is FavoritePage)
                return SimpleIoc.Default.GetInstance<FavoritePageViewModel>();
            else if (page is Views.TileViewPage.PersonTileView)
                return SimpleIoc.Default.GetInstance<PersonTileViewViewModel>();
            else if (page is AddEditPage)
                return SimpleIoc.Default.GetInstance<AddEditPageViewModel>();
            else
                return base.ResolveForPage(page, navigationService);
        }
    }
}
