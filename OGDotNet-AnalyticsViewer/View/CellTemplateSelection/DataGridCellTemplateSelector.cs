﻿//-----------------------------------------------------------------------
// <copyright file="DataGridCellTemplateSelector.cs" company="OpenGamma Inc. and the OpenGamma group of companies">
//     Copyright © 2009 - present by OpenGamma Inc. and the OpenGamma group of companies
//
//     Please see distribution for license.
// </copyright>
//-----------------------------------------------------------------------
using System.Windows;
using System.Windows.Controls;

namespace OGDotNet.AnalyticsViewer.View.CellTemplateSelection
{
    public abstract class DataGridCellTemplateSelector : CellTemplateSelectorBase
    {
        private readonly DataGridTemplateColumn _column;

        protected DataGridCellTemplateSelector(DataGridTemplateColumn column)
        {
            _column = column;
        }

        protected override DataTemplateSelector GetCurrentCellTemplateSelector()
        {
            return _column.CellTemplateSelector;
        }

        protected override void SetCurrentCellTemplateSelector(DataTemplateSelector selector)
        {
            _column.CellTemplateSelector = selector;
        }

        protected override void SetCurrentCellTemplate(DataTemplate template)
        {
            _column.CellTemplate = template;
        }
    }
}