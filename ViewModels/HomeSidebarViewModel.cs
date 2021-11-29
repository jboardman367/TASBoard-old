﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using TASBoard.Models;

namespace TASBoard.ViewModels
{
    public class HomeSidebarViewModel : ViewModelBase
    {
        private Workspace workspace;

        public Workspace Workspace { get => workspace; }
        public HomeSidebarViewModel(Workspace w)
        {
            workspace = w;

            // Create the empty observable collections
            keyNames = new();
            keyStyles = new();

            // Create the AddKey command
            var addKeyEnabled = this.WhenAnyValue(
                x => x.SelectedKey,
                x => !string.IsNullOrEmpty(x));

            AddKey = ReactiveCommand.Create(
                AddKeyToWorkspace,
                addKeyEnabled);

            // Create the Encode command
            var validMovie = this.WhenAnyValue(
                x => x.MoviePath,
                x => File.Exists(x));

            var validOutputPath = this.WhenAnyValue(
                x => x.OutputPath,
                x => Directory.Exists(Path.GetDirectoryName(x)));

            var encodeEnabled = Observable.Merge(validMovie, validOutputPath);

            Encode = ReactiveCommand.Create(
                () => workspace.Encode(MoviePath, OutputPath),
                encodeEnabled);
        }

        public ReactiveCommand<Unit, Unit> Encode { get; }

        private void AddKeyToWorkspace()
        {
            if (selectedKey is not null && selectedStyle is not null)
                workspace.AddKey(selectedStyle, selectedKey);
        }

        public ReactiveCommand<Unit, Unit> AddKey { get; }

        private string? selectedKey, selectedStyle;

        public string? SelectedKey
        {
            get => selectedKey;
            set => this.RaiseAndSetIfChanged(ref selectedKey, value);
        }
        public string? SelectedStyle
        {
            get => selectedStyle;
            set => this.RaiseAndSetIfChanged(ref selectedStyle, value);
        }


        public ObservableCollection<string> KeyStyles
        {
            get => keyStyles;
            set => this.RaiseAndSetIfChanged(ref keyStyles, value);
        }

        private ObservableCollection<string> keyStyles;

        public ObservableCollection<string> KeyNames
        {
            get => keyNames;
            set => this.RaiseAndSetIfChanged(ref keyNames, value);
        }

        private ObservableCollection<string> keyNames;

        public void UpdateDropdowns()
        {
            List<string> availableStyles = GetAvailableStyles();
            foreach (string item in availableStyles)
            {
                if (!keyStyles.Contains(item))
                {
                    KeyStyles.Add(item);
                }
            }

            foreach (string item in keyStyles)
            {
                if (!availableStyles.Contains(item))
                {
                    KeyStyles.Remove(item);
                }
            }

            List<string> availableNames = GetAvailableKeys();
            foreach (string item in availableNames)
            {
                if (!keyNames.Contains(item))
                {
                    KeyNames.Add(item);
                }
            }

            foreach (string item in keyNames)
            {
                if (!availableNames.Contains(item))
                {
                    KeyNames.Remove(item);
                }
            }
        }


        private List<string> GetAvailableStyles()
        {
            string[] directories = Directory.GetDirectories("Assets\\KeySprites");
            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i].Replace("Assets\\KeySprites\\", "");
            }
            return directories.ToList();
        }

        private List<string> GetAvailableKeys()
        {
            // Return an empty list if selectedStyle is null
            if (selectedStyle is null) { return new List<string>(); }

            List<string> keys = new();
            string[] mappedKeys = GetMappedKeys();

            // We only want to show keys that have both up and down sprites
            // and are mapped in mappings.txt
            foreach (string key in mappedKeys)
            {
                string path = "Assets\\KeySprites\\" + selectedStyle + "\\" + key;
                if (File.Exists(path + "_up.png") && File.Exists(path + "_down.png"))
                    keys.Add(key);
            }

            return keys;
        }

        static private string[] GetMappedKeys()
        {
            /* The mappings.txt file is in the form [keycode],[keyname] on each line
             * The mouse buttons (and potentially other buttons added later) are stored
             * differently in the movie file, so they are hard coded seperately.
             */
            string[] lines = File.ReadAllLines("Assets/mappings.txt");
            string[] mappedKeys = new string[lines.Length + 2];
            mappedKeys[0] = "LMB";
            mappedKeys[1] = "RMB";

            for (int i = 2; i < mappedKeys.Length; i++)
            {
                mappedKeys[i] = lines[i - 2].Split(',')[1];
            }

            return mappedKeys;
        }

        public string? MoviePath { get; set; }
        public string? OutputPath { get; set; }
    }
}
