﻿using System;
using System.Windows;
using FocusTreeManager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FocusTreeManager.ViewModel
{
    public class ProjectEditorViewModel : ViewModelBase
    {

        private ProjectModel project;

        public ProjectModel Project
        {
            get
            {
                return project;
            }
            set
            {
                if (value == project)
                {
                    return;
                }
                project = value;
                RaisePropertyChanged(() => Project);
            }
        }

        private string selectedString;

        public string SelectedString
        {
            get
            {
                return selectedString;
            }
            set
            {
                selectedString = value;
                DeleteModFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand AcceptCommand { get; set; }

        public RelayCommand CancelCommand { get; set; }

        public RelayCommand AddModFolderCommand { get; set; }

        public RelayCommand DeleteModFolderCommand { get; set; }

        public ProjectEditorViewModel()
        {
            AcceptCommand = new RelayCommand(Accept);
            CancelCommand = new RelayCommand(Cancel);
            AddModFolderCommand = new RelayCommand(AddModFolder);
            DeleteModFolderCommand = new RelayCommand(DeleteModFolder, CanDeleteModFolder);
        }

        public void Accept()
        {
            Close();
        }

        public void Cancel()
        {
            Project = null;
            Close();
        }

        public void AddModFolder()
        {
            ResourceDictionary resourceLocalization = new ResourceDictionary
            {
                Source = new Uri(Configurator.getLanguageFile(), UriKind.Relative)
            };
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = resourceLocalization["Game_Path_Title"] as string,
                IsFolderPicker = true,
                InitialDirectory = Environment.GetFolderPath(Environment.
                                   SpecialFolder.MyDocuments),
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = "C:",
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Project.ListModFolders.Add(dialog.FileName);
                AsyncImageLoader.AsyncImageLoader.Worker.RefreshFromMods();
            }
            Activate();
        }

        public void DeleteModFolder()
        {
            if (string.IsNullOrEmpty(selectedString))
            {
                return;
            }
            Project.ListModFolders.Remove(selectedString);
            AsyncImageLoader.AsyncImageLoader.Worker.RefreshFromMods();
        }

        public bool CanDeleteModFolder()
        {
            return !string.IsNullOrEmpty(selectedString);
        }

        private void Close()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }

        private void Activate()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Activate();
                }
            }
        }
    }
}