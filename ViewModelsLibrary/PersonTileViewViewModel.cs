﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml.Navigation;
using ServicesLibrary.RepositoryService;
using TypesLibrary.Models;
using Windows.UI.Xaml.Controls;
using ServicesLibrary.TileServices;
using Windows.UI.StartScreen;

namespace ViewModelsLibrary
{
    public class PersonTileViewViewModel : ViewModelBase
    {
        #region Fields
        IRepositoryService<Person> _personsRepositary;
        DelegateCommand _pinCommand;
        ITileService _tileService;
        Symbol _pinSymbol;
        bool IsPinned;
        Person _showedPerson;
        #endregion

        #region Constructor
        public PersonTileViewViewModel(IRepositoryService<Person> personsRepositary, ITileService tileService)
        {
            _personsRepositary = personsRepositary;
            _tileService = tileService;
            _pinSymbol = Symbol.UnPin;
            _pinCommand = new DelegateCommand(PinExecute);
        }
        #endregion

        #region Navigation events
        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var Id = parameter as string;
            ShowedPerson = await _personsRepositary.GetByIdAsync(Id);
        }
        #endregion

        #region Bindable properties
        public Person ShowedPerson
        {
            set { Set(ref _showedPerson, value); }
            get { return _showedPerson; }
        }

        public Symbol PinSymbol
        {
            set { Set(ref _pinSymbol, value); }
            get { return _pinSymbol; }
        }
        #endregion

        #region Commands
        #region PinCommand
        public DelegateCommand PinCommand
        {
            get { return _pinCommand ?? new DelegateCommand(PinExecute); }
        }

        private async void PinExecute()
        {
            IsPinned = false;

            if (PinSymbol == Symbol.Pin)
            {
                var tileOnShow = new SecondaryTile(Guid.NewGuid().ToString(), $"{ShowedPerson.Name} {ShowedPerson.LastName}", ShowedPerson.Id, new Uri("ms-appx:///Assets/LogoForTile.png"), TileSize.Square150x150);
                IsPinned = await _tileService.RequestCreate(tileOnShow);
                PinSymbol = Symbol.UnPin;
            }
            else
            {
                _tileService.RequestDelete(ShowedPerson.Id);
                PinSymbol = Symbol.Pin;
            }

            if (IsPinned)
                PinSymbol = Symbol.UnPin;
        }
        #endregion
        #endregion
    }
}