﻿using ArkhamOverlay.Data;
using ArkhamOverlay.Pages.ChooseEncounters;
using ArkhamOverlay.Pages.SelectCards;
using ArkhamOverlay.Services;
using Microsoft.Win32;
using PageController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace ArkhamOverlay.Pages.Main {
    public class MainController : Controller<MainView, MainViewModel> {
        private Overlay _overlay;
        private readonly ArkhamDbService _arkhamDbService = new ArkhamDbService();
        private readonly IList<SelectCardsController> _selectCardsControllers = new List<SelectCardsController>();

        private readonly GameFileService _gameFileService;
        private readonly IControllerFactory _controllerFactory;

        public MainController(AppData appData, GameFileService gameFileService, IControllerFactory controllerFactory) {
            ViewModel.AppData = appData;

            _gameFileService = gameFileService;
            _controllerFactory = controllerFactory;

            LoadEncounterSets();

            View.Closed += (s, e) => {
                ClearPlayerCardsWindows();

                if (_overlay != null) {
                    _overlay.Close();
                }
            };
        }

        private void LoadEncounterSets() {
            var worker = new BackgroundWorker();
            worker.DoWork += (x, y) => {
                _arkhamDbService.FindMissingEncounterSets(ViewModel.AppData.Configuration);
            };
            worker.RunWorkerAsync();
        }

        private void ClearPlayerCardsWindows() {
            while (_selectCardsControllers.Count > 0) {
                _selectCardsControllers.First().Close();
            }
        }

        private void ShowSelectCardsWindow(ISelectableCards selectableCards) {
            var left = View.Left + View.Width + 10;
            var width = (double)786;
            var top = View.Top;
            SelectCardsController controller = null;
            foreach (var selectCardsControllerInList in _selectCardsControllers) {
                if (selectCardsControllerInList.SelectableCards == selectableCards) {
                    controller = selectCardsControllerInList;
                    controller.Activate();
                    break;
                } else {
                    if (selectCardsControllerInList.Top + selectCardsControllerInList.Height > top) {
                        top = selectCardsControllerInList.Top + selectCardsControllerInList.Height + 10;
                        width = selectCardsControllerInList.Width;
                        left = selectCardsControllerInList.Left;
                    }
                }
            }

            if (controller == null) {
                controller = _controllerFactory.CreateController<SelectCardsController>();
                controller.SelectableCards = selectableCards;
                controller.Left = left;
                controller.Top = top;
                controller.Width = width;

                controller.Closed += () => {
                    _selectCardsControllers.Remove(controller);
                };

                _selectCardsControllers.Add(controller);
            }

            if (!controller.SelectableCards.Loading) {
                controller.Show();
            } else {
                ShowSelectCardsWindowWhenFinishedLoading(controller);
            }
        }

        private void ShowSelectCardsWindowWhenFinishedLoading(SelectCardsController selectCardsController) {
            var timer = new DispatcherTimer {
                Interval = new TimeSpan(500)
            };

            timer.Tick += (x, y) => {
                if (selectCardsController.SelectableCards.Loading) {
                    return;
                }

                timer.Stop();
                selectCardsController.Show();
            };

            timer.Start();
        }


        [Command]
        public void SaveGame() {
            _gameFileService.Save(ViewModel.AppData.Game.Name + ".json");
        }

        [Command]
        public void LoadGame() {
            var dialog = new OpenFileDialog {
                DefaultExt = "json",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (dialog.ShowDialog() == true) {
                _gameFileService.Load(dialog.FileName);
                ClearPlayerCardsWindows();
            }
        }

        [Command]
        public void SetEncounterSets() {
            var chooseEncounters = _controllerFactory.CreateController<ChooseEncountersController>();
            chooseEncounters.ShowDialog();
        }

        [Command]
        public void ShowOtherEncounters() {
            ShowSelectCardsWindow(ViewModel.AppData.Game.ScenarioCards);
        }

        [Command]
        public void ShowLocations() {
            ShowSelectCardsWindow(ViewModel.AppData.Game.LocationCards);
        }

        [Command]
        public void ShowEncounterDeck() {
            ShowSelectCardsWindow(ViewModel.AppData.Game.EncounterDeckCards);
        }

        [Command]
        public void Refresh(Player player) {
            _arkhamDbService.LoadPlayer(player);
            var worker = new BackgroundWorker();
            worker.DoWork += (x, y) => {
                _arkhamDbService.LoadPlayerCards(player);
            };
            worker.RunWorkerAsync();
        }

        private void MainWindowActivated(object sender, EventArgs e) {
            foreach (var selectCardsWindow in _selectCardsControllers) {
                selectCardsWindow.Show();
            }
        }

        [Command]
        public void PlayerSelected(SelectableCards selectableCards) {
            ShowSelectCardsWindow(selectableCards);
        }

        [Command]
        public void ShowOverlay() {
            if (_overlay != null) {
                _overlay.Activate();
                return;
            }


            _overlay = new Overlay {
                Top = View.Top + View.Height + 10,
            };
            _overlay.SetAppData(ViewModel.AppData);

            _overlay.Closed += (x, y) => {
                _overlay = null;
            };

            _overlay.Show();
        }

        [Command]
        public void ClearCards(object sender) {
            ViewModel.AppData.Game.ClearAllCards();
        }

        [Command]
        public void ToggleActAgendaBar() {
            if (_overlay != null) {
                _overlay.ToggleActAgendaBar();
            }
        }
    }
}
