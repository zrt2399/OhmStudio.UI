using System;
using System.Windows;
using System.Windows.Controls;

namespace OhmStudio.UI.Attaches
{
    public class GridAttach
    {
        #region RowDefinitions attached property

        /// <summary>
        /// Identified the RowDefinitions attached property.
        /// </summary>
        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached("RowDefinitions", typeof(string), typeof(GridAttach),
                new PropertyMetadata(string.Empty, OnRowDefinitionsPropertyChanged));

        /// <summary>
        /// Gets the value of the RowDefinitions property.
        /// </summary>
        public static string GetRowDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(RowDefinitionsProperty);
        }

        /// <summary>
        /// Sets the value of the RowDefinitions property.
        /// </summary>
        public static void SetRowDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(RowDefinitionsProperty, value);
        }

        /// <summary>
        /// Handles property changed event for the RowDefinitions property, constructing
        /// the required RowDefinitions elements on the grid which this property is attached to.
        /// </summary>
        private static void OnRowDefinitionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not Grid targetGrid)
            {
                return;
            }
            // construct the required row definitions
            targetGrid.RowDefinitions.Clear();
            string rowDefs = e.NewValue as string;
            var rowDefArray = rowDefs?.Split(',') ?? Array.Empty<string>();
            foreach (string rowDefinition in rowDefArray)
            {
                if (string.IsNullOrWhiteSpace(rowDefinition))
                {
                    targetGrid.RowDefinitions.Add(new RowDefinition());
                }
                else
                {
                    targetGrid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = ParseLength(rowDefinition)
                    });
                }
            }
        }

        #endregion


        #region ColumnDefinitions attached property

        /// <summary>
        /// Identifies the ColumnDefinitions attached property.
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("ColumnDefinitions", typeof(string), typeof(GridAttach),
                new PropertyMetadata(string.Empty, OnColumnDefinitionsPropertyChanged));

        /// <summary>
        /// Gets the value of the ColumnDefinitions property.
        /// </summary>
        public static string GetColumnDefinitions(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnDefinitionsProperty);
        }

        /// <summary>
        /// Sets the value of the ColumnDefinitions property.
        /// </summary>
        public static void SetColumnDefinitions(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnDefinitionsProperty, value);
        }

        /// <summary>
        /// Handles property changed event for the ColumnDefinitions property, constructing
        /// the required ColumnDefinitions elements on the grid which this property is attached to.
        /// </summary>
        private static void OnColumnDefinitionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not Grid targetGrid)
            {
                return;
            }

            // construct the required column definitions
            targetGrid.ColumnDefinitions.Clear();
            string columnDefs = e.NewValue as string;
            var columnDefArray = columnDefs?.Split(',') ?? Array.Empty<string>();
            foreach (string columnDefinition in columnDefArray)
            {
                if (string.IsNullOrWhiteSpace(columnDefinition))
                {
                    targetGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }
                else
                {
                    targetGrid.ColumnDefinitions.Add(new ColumnDefinition()
                    {
                        Width = ParseLength(columnDefinition)
                    });
                }
            }
        }

        #endregion

        /// <summary>
        /// Parses a string to create a GridLength.
        /// </summary>
        private static GridLength ParseLength(string length)
        {
            length = length.Trim();

            if (length.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                return new GridLength(0, GridUnitType.Auto);
            }
            else if (length.Contains("*"))
            {
                length = length.Replace("*", "");
                if (string.IsNullOrEmpty(length))
                {
                    length = "1";
                }
                return new GridLength(double.Parse(length), GridUnitType.Star);
            }

            return new GridLength(double.Parse(length), GridUnitType.Pixel);
        }
    }
}