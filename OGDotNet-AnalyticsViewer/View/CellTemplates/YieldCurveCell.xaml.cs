﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OGDotNet.Mappedtypes.financial.model.interestrate.curve;
using OGDotNet.Mappedtypes.math.curve;

namespace OGDotNet.AnalyticsViewer.View.CellTemplates
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class YieldCurveCell : UserControl
    {
        public YieldCurveCell()
        {
            InitializeComponent();
        }


        private YieldCurve YieldCurve
        {
            get { return (DataContext as YieldCurve); }
        }

        private Curve Curve
        {
            get { return YieldCurve.Curve; }
        }


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateGraph();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateGraph();
        }


        private void UpdateGraph()
        {
            if (YieldCurve != null)
            {
                YieldCurve yieldCurve = YieldCurve;
                if (IsVirtual(yieldCurve.Curve))
                {//We can't display this
                    IsEnabled = false;
                    ShowDisabled();
                    canvas.Visibility = Visibility.Visible;
                    return;
                }

                var doubleMinX = Curve.XData.Min();
                var doubleMaxX = Curve.XData.Max();
                double xScale = ActualWidth/(doubleMaxX - doubleMinX);

                var doubleMinY = Math.Min(Curve.YData.Min(), 0);
                var doubleMaxY = Curve.YData.Max();
                double yScale = ActualHeight / (doubleMaxY - doubleMinY);

                myLine.Points.Clear();
                foreach (var tuple in    Curve.GetData())
                {
                    var x = (tuple.Item1 - doubleMinX) * xScale;
                    var y = ActualHeight - ((tuple.Item2 - doubleMinY) * yScale);
                    myLine.Points.Add(new Point(x, y));
                }

                xAxis.X1 = 0;
                xAxis.X2 = ActualWidth;

                xAxis.Y1 = ActualHeight - ((-doubleMinY)*yScale);
                xAxis.Y2 = xAxis.Y1;

                yAxis.X1 = 0;
                yAxis.X2 = 0;
                yAxis.Y1 = ActualHeight;
                yAxis.Y2 = 0;
                IsEnabled = true;
                canvas.Visibility = Visibility.Visible;
            }
            else
            {
                canvas.Visibility = Visibility.Hidden;
            }
        }

        private void ShowDisabled()
        {
            xAxis.X1 = 0;
            xAxis.X2 = ActualWidth;

            xAxis.Y1 = ActualHeight;
            xAxis.Y2 = 0;

            yAxis.X1 = 0;
            yAxis.X2 = ActualWidth;
            yAxis.Y1 = 0;
            yAxis.Y2 = ActualHeight;

            myLine.Points.Clear();
        }

        private static bool IsVirtual(Curve curve)
        {//TODO where should this live.  Probably on the curves themselves
            try
            {
                var xData = curve.XData;
                return false;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnabled)
            {
                detailsPopup.DataContext = Curve.GetData();
                detailsPopup.IsOpen = true;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            detailsPopup.IsOpen = false;
        }
    }
}
