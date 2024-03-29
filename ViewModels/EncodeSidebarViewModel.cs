﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using TASBoard.Models;
using TASBoard.MovieReaders;

namespace TASBoard.ViewModels
{
    public class EncodeSidebarViewModel : ViewModelBase
    {
        private Workspace workspace;

        public Workspace Workspace { get => workspace; }
        public EncodeSidebarViewModel(Workspace w)
        {
            workspace = w;

            // Create the Encode command
            var validMovie = this.WhenAnyValue(
                x => x.MoviePath,
                x => File.Exists(x) && IMovieReader.IsValidFile(x));

            var validOutputPath = this.WhenAnyValue(
                x => x.OutputPath,
                x => Directory.Exists(Path.GetDirectoryName(x)));

            var validNumerator = this.WhenAnyValue(
                x => x.FramerateNum,
                x => int.TryParse(x, out int _));

            var validDenominator = this.WhenAnyValue(
                x => x.FramerateDen,
                x => int.TryParse(x, out int _));

            var encodeEnabled = Observable.CombineLatest(validMovie, validOutputPath, validNumerator, validDenominator, (a, b, c, d) => a && b && c && d);

            Encode = ReactiveCommand.Create(
                () => workspace.Encode(MoviePath, OutputPath, new Fraction(int.Parse(framerateNum), int.Parse(framerateDen))),
                encodeEnabled);
        }

        public ReactiveCommand<Unit, Unit> Encode { get; }

        private string framerateNum = "60";
        private string framerateDen = "1";

        public string FramerateNum
        {
            get => framerateNum;
            set => this.RaiseAndSetIfChanged(ref framerateNum, value);
        }

        public string FramerateDen
        {
            get => framerateDen;
            set => this.RaiseAndSetIfChanged(ref framerateDen, value);
        }

        private string? moviePath;
        private string? outputPath;
        public string? MoviePath { get => moviePath; set => this.RaiseAndSetIfChanged(ref moviePath, value); }
        public string? OutputPath { get => outputPath; set => this.RaiseAndSetIfChanged(ref outputPath, value); }
    }
}
