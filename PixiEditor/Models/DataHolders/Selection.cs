﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using PixiEditor.Helpers;
using PixiEditor.Models.Enums;
using PixiEditor.Models.Layers;
using PixiEditor.Models.Position;
using PixiEditor.Models.Tools;
using PixiEditor.ViewModels;

namespace PixiEditor.Models.DataHolders
{
    public class Selection : NotifyableObject
    {
        private readonly Color _selectionBlue;
        private Layer _selectionLayer;

        public Selection(Coordinates[] selectedPoints)
        {
            SelectedPoints = new ObservableCollection<Coordinates>(selectedPoints);
            SelectionLayer = new Layer("_selectionLayer", ViewModelMain.Current.BitmapManager.ActiveDocument.Width,
                ViewModelMain.Current.BitmapManager.ActiveDocument.Height);
            _selectionBlue = Color.FromArgb(127, 142, 202, 255);
        }

        public ObservableCollection<Coordinates> SelectedPoints { get; private set; }

        public Layer SelectionLayer
        {
            get => _selectionLayer;
            set
            {
                _selectionLayer = value;
                RaisePropertyChanged("SelectionLayer");
            }
        }

        public void SetSelection(Coordinates[] selection, SelectionType mode)
        {
            Color _selectionColor = _selectionBlue;
            switch (mode)
            {
                case SelectionType.New:
                    SelectedPoints = new ObservableCollection<Coordinates>(selection);
                    SelectionLayer.Clear();
                    break;
                case SelectionType.Add:
                    SelectedPoints = new ObservableCollection<Coordinates>(SelectedPoints.Concat(selection).Distinct());
                    break;
                case SelectionType.Substract:
                    SelectedPoints = new ObservableCollection<Coordinates>(SelectedPoints.Except(selection));
                    _selectionColor = System.Windows.Media.Colors.Transparent;
                    break;
            }

            SelectionLayer.ApplyPixels(BitmapPixelChanges.FromSingleColoredArray(selection, _selectionColor));
        }

        public void Clear()
        {
            SelectionLayer.Clear();
            SelectedPoints.Clear();
        }
    }
}