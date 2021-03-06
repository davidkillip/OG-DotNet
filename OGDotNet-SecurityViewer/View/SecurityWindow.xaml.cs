﻿//-----------------------------------------------------------------------
// <copyright file="SecurityWindow.xaml.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OGDotNet.Mappedtypes.Core.Security;
using OGDotNet.Mappedtypes.Master.Security;
using OGDotNet.Mappedtypes.Util;
using OGDotNet.Model.Resources;
using OGDotNet.WPFUtils.Windsor;

namespace OGDotNet.SecurityViewer.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SecurityWindow : OGDotNetWindow
    {
        long _cancellationToken = long.MinValue;

        public SecurityWindow()
        {
            InitializeComponent();
            itemGrid.Items.Clear();
            typeBox.ItemsSource = new[] { string.Empty }.Concat(
                OGContext.SecurityMaster.MetaData(new SecurityMetaDataRequest()).SecurityTypes);
            typeBox.SelectedIndex = 0;
        }

        private RemoteSecurityMaster SecurityMaster
        {
            get { return OGContext.SecurityMaster; }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void CancelIfCancelled(long cancellationToken)
        {
            if (cancellationToken != _cancellationToken)
            {
                throw new OperationCanceledException();
            }
        }

        private void Update()
        {
            var token = ++_cancellationToken;

            string type = (string) typeBox.SelectedItem;
            string name = nameBox.Text;

            if (type == string.Empty)
            {
                type = null; //Wildcard
            }

            int currentPage = CurrentPage;

            ThreadPool.QueueUserWorkItem(delegate
                                             {
                                                 try
                                                 {
                                                     CancelIfCancelled(token);
                                                     var request = new SecuritySearchRequest(PagingRequest.OfPage(currentPage, 20), name, type);
                                                     var results = SecurityMaster.Search(request);
                                                     CancelIfCancelled(token);
                                                     Dispatcher.Invoke((Action)(() =>
                                                                                          {
                                                                                              CancelIfCancelled(token);
                                                                                              itemGrid.DataContext = results.Documents.Select(s => s.Security).ToList();
                                                                                              itemGrid.SelectedIndex = 0;
                                                                                              pageCountLabel.DataContext = results.Paging;
                                                                                              currentPageLabel.DataContext = results.Paging;
                                                                                              outerGrid.UpdateLayout();
                                                                                          }));
                                                 }
                                                 catch (OperationCanceledException)
                                                 {
                                                 }
                                             });
        }

        private int CurrentPage
        {
            get
            {
                int currentPage;
                if (!int.TryParse(currentPageLabel.Text, out currentPage))
                {
                    currentPage = 1;
                }
                return currentPage;
            }
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                currentPageLabel.Text = value.ToString();
            }
        }
        public int PageCount
        {
            get
            {
                return (int)pageCountLabel.Content;
            }
        }

        private void typeBox_SelectedItemChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            if (IsLoaded)
                Update();
        }

        private void nameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
                Update();
        }

        private void nextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage++;
            Update();
        }
        private void lastPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = PageCount;
            Update();
        }

        private void grid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (itemGrid.SelectedItem != null)
            {
                ISecurity security = (ISecurity)itemGrid.SelectedItem;

                var securities = new[] { security };

                ShowSecurities(securities);
            }
        }

        private void showAll_Click(object sender, RoutedEventArgs e)
        {
            ShowSecurities(itemGrid.Items.Cast<ISecurity>());
        }

        private void ShowSecurities(IEnumerable<ISecurity> securities)
        {
            SecurityTimeSeriesWindow.ShowDialog(securities, this);
        }

        private void grid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            detailsGrid.DataContext = itemGrid.SelectedItem;
        }

        private void firstPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            Update();
        }

        private void previousPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
            Update();
        }
    }
}
