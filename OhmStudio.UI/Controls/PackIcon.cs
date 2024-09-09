﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    public enum PackIconFlipOrientation
    {
        /// <summary>
        /// No flip
        /// </summary>
        Normal,

        /// <summary>
        /// Flip the icon horizontal
        /// </summary>
        Horizontal,

        /// <summary>
        /// Flip the icon vertical
        /// </summary>
        Vertical,

        /// <summary>
        /// Flip the icon vertical and horizontal
        /// </summary>
        Both
    }

    public enum PackIconKind
    {
        Save,
        SaveAs,
        SaveAll,
        Wrench,
        SolidWrench,
        CollapseAll,
        SectionCollapseAll,
        ExpandAll,
        SectionExpandAll,
        UserMinus,
        UserPlus,
        SolidUserPlus,
        AccountMultiplePlus,
        AccountMultipleRemove,
        AccountGroup,
        FileAdd,
        FileOutline,
        FileRemove,
        FileSearch,
        FileProcess,
        FileTextOutline,
        FileZip,
        Folder,
        NewFolder,
        FolderOutline,
        FolderAdd,
        FolderOutlinepenOutline,
        FolderRemove,
        FolderSearch,
        Play,
        PlayiOS,
        CogOutline,
        Fire,
        FireAlert,
        Pause,
        PauseiOS,
        Stop,
        SquareiOS,
        Refresh,
        RefreshiOS,
        AddiOS,
        Add,
        AddBox,
        AddCircle,
        AddCircleOutline,
        RemoveiOS,
        RemoveCircle,
        RemoveCircleOutline,
        Remove,
        IndeterminateCheckBox,
        CopyiOS,
        FileCopyLine,
        Paste,
        BuildiOS,
        CheckmarkiOS,
        CloseiOS,
        FingerPrintiOS,
        PrintiOS,
        SettingsiOS,
        WifiiOS,
        FeatureSearchOutline,
        ClearAll,
        InterfacePassword,
        FormTextboxPassword,
        RegularMessageAlt,
        RegularMessage,
        MessageBulleted,
        MessageBulletedOff,
        BackspaceiOS,
        TrashiOS,
        ApplicationExport,
        ApplicationImport,
        CurrentAc,
        CurrentDc,
        Signal2g,
        Signal3g,
        Signal4g,
        Signal5g,
        Ethernet,
        SerialPort,
        EvPlugType1,
        EvPlugType2,
        PowerPlug,
        PowerPlugOff,
        Bluetooth,
        BluetoothOff
    }

    public class PackIcon : Control
    {
        private static readonly Lazy<IDictionary<PackIconKind, PathInfo>> _dataIndex =
            new Lazy<IDictionary<PackIconKind, PathInfo>>(PackIconDataFactory.Create);

        static PackIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
        }

        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register(nameof(Kind), typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(default(PackIconKind), KindPropertyChangedCallback));

        private static void KindPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ((PackIcon)dependencyObject).UpdateData();
        }

        /// <summary>
        /// Gets or sets the icon to display.
        /// </summary>
        public PackIconKind Kind
        {
            get => (PackIconKind)GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        private static readonly DependencyPropertyKey DataPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(string.Empty));

        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the icon path data for the current <see cref="Kind"/>.
        /// </summary>
        [TypeConverter(typeof(GeometryConverter))]
        public string Data
        {
            get => (string)GetValue(DataProperty);
            private set => SetValue(DataPropertyKey, value);
        }

        internal static readonly DependencyProperty FlipProperty =
            DependencyProperty.Register(nameof(Flip), typeof(PackIconFlipOrientation), typeof(PackIcon));

        internal PackIconFlipOrientation Flip
        {
            get => (PackIconFlipOrientation)GetValue(FlipProperty);
            set => SetValue(FlipProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateData();
        }

        private void UpdateData()
        {
            PathInfo pathInfo = null;
            _dataIndex.Value?.TryGetValue(Kind, out pathInfo);
            if (pathInfo != null)
            {
                Flip = pathInfo.Flip;
                Data = pathInfo.Path;
            }
        }
    }

    [MarkupExtensionReturnType(typeof(PackIcon))]
    public class PackIconExtension : MarkupExtension
    {
        public PackIconExtension()
        { }

        public PackIconExtension(PackIconKind kind)
        {
            Kind = kind;
        }

        public PackIconExtension(PackIconKind kind, double size) : this(kind)
        {
            Size = size;
        }

        public PackIconExtension(PackIconKind kind, double size, double padding) : this(kind, size)
        {
            Padding = padding;
        }

        [ConstructorArgument("kind")]
        public PackIconKind Kind { get; set; }

        [ConstructorArgument("size")]
        public double? Size { get; set; }

        [ConstructorArgument("padding")]
        public double? Padding { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = new PackIcon { Kind = Kind };

            if (Size.HasValue)
            {
                result.Height = Size.Value;
                result.Width = Size.Value;
            }
            if (Padding.HasValue)
            {
                result.Padding = new Thickness(Padding.Value);
            }
            return result;
        }
    }

    public class PathInfo
    {
        public PathInfo(PackIconFlipOrientation flip, string path)
        {
            Flip = flip;
            Path = path;
        }

        public PackIconFlipOrientation Flip { get; set; }

        public string Path { get; set; }
    }

    public static class PackIconDataFactory
    {
        public static IDictionary<PackIconKind, PathInfo> Create() => new Dictionary<PackIconKind, PathInfo>()
        {
            { PackIconKind.Save, new PathInfo (PackIconFlipOrientation.Both,"M166.9125 185.675L185.6625 166.925L187.5 162.5V18.75L181.25 12.5H18.75L12.5 18.75V181.25L18.75 187.5H162.5L166.9125 185.675zM25 175V25H175V159.9L159.9125 175H137.5V125H50V175H25zM100 175V137.5H125V175H100z")  },
            { PackIconKind.SaveAs, new PathInfo (PackIconFlipOrientation.Vertical,"M138 183.375L158.875 162.5L162.5 153.625V149.5H156.25L150 143.25V153.625L129.125 174.5H125V124.5H50V174.5H25V49.5H62.5L56.25 37H25L12.5 49.5V174.5L25 187H129.125L138 183.375zM87.5 137.5H112.5V175H87.5V137.5zM168.75 137.5L187.5 118.75L187.25 110.125L118.5 41.375L116.875 39.875L112.25 35.25L111 34.125L73.5 15.375L65.125 23.75L83.875 61.25L85 62.5L89.625 67.125L91.125 68.75L159.875 137.5H168.75zM91 47L84.5 34.5L97.5 40.5L91 47zM99.625 59.875L109.5 50L173.875 114.375L164 124.25L99.625 59.875z")  },
            { PackIconKind.SaveAll, new PathInfo (PackIconFlipOrientation.Both,"M185.625 166.875L166.875 185.625L162.5 187.5H56L49.75 181.25V150H18.75L12.5 143.75V18.75L18.75 12.5H143.75L150 18.75V50H181.25L187.5 56.25V162.5L185.625 166.875zM137.5 25H25V137.5H37.5V99.125H112.5V137.5H122.375L137.5 122.375V25zM75 112.5V137.5H100V112.5H75zM175 62.5H150V125L148.125 129.375L129.375 148.125L125 150H62.5V175H160.125L175.25 159.875L175 62.5z")  },
            { PackIconKind.Wrench, new PathInfo (PackIconFlipOrientation.Normal,"M22.7,19L13.6,9.9C14.5,7.6 14,4.9 12.1,3C10.1,1 7.1,0.6 4.7,1.7L9,6L6,9L1.6,4.7C0.4,7.1 0.9,10.1 2.9,12.1C4.8,14 7.5,14.5 9.8,13.6L18.9,22.7C19.3,23.1 19.9,23.1 20.3,22.7L22.6,20.4C23.1,20 23.1,19.3 22.7,19Z")  },
            { PackIconKind.SolidWrench, new PathInfo (PackIconFlipOrientation.Vertical,"M917.845 699.221l-165.973-165.931-150.827 150.869 165.931 165.931c-31.177 13.176-67.427 20.833-105.466 20.833-153.191 0-277.376-124.185-277.376-277.376 0-38.061 7.666-74.332 21.539-107.353l-0.681 1.827-267.776-267.733c-7.729-7.716-12.51-18.382-12.51-30.165s4.781-22.449 12.509-30.165l90.54-90.54c7.716-7.729 18.382-12.51 30.165-12.51s22.449 4.781 30.165 12.509l267.734 267.777c31.211-13.208 67.504-20.883 105.589-20.883 76.582 0 145.915 31.035 196.108 81.215l-0.001-0.001c50.166 50.21 81.191 119.55 81.191 196.137 0 38.073-7.667 74.355-21.542 107.386l0.681-1.828z")  },
            { PackIconKind.CollapseAll, new PathInfo (PackIconFlipOrientation.Vertical,"M112.5 87.5H50V75H112.5V87.5zM62.5 162.5L75 175H162.5L175 162.5V75L162.5 62.5H137.5V37.5L125 25H37.5L25 37.5V125L37.5 137.5H62.5V162.5zM75 137.5H125L137.5 125V75H162.5V162.5H75V137.5zM125 125H37.5V37.5H125V125z")  },
            { PackIconKind.SectionCollapseAll, new PathInfo (PackIconFlipOrientation.Normal,"M 19,29L 47,29L 47,57L 19,57L 19,29 Z M 43,33L 23,33.0001L 23,53L 43,53L 43,33 Z M 39,41L 39,45L 27,45L 27,41L 39,41 Z M 24,24L 51.9999,24.0001L 51.9999,52L 48.9999,52.0001L 48.9999,27.0001L 24,27.0001L 24,24 Z M 54,47L 53.9999,22.0001L 29,22L 29,19L 57,19L 57,47L 54,47 Z")  },
            { PackIconKind.ExpandAll, new PathInfo (PackIconFlipOrientation.Vertical,"M112.5 87.5H50V75H112.5V87.5zM87.5 50V112.5H75V50H87.5zM62.5 162.5L75 175H162.5L175 162.5V75L162.5 62.5H137.5V37.5L125 25H37.5L25 37.5V125L37.5 137.5H62.5V162.5zM75 137.5H125L137.5 125V75H162.5V162.5H75V137.5zM125 125H37.5V37.5H125V125z")  },
            { PackIconKind.SectionExpandAll, new PathInfo (PackIconFlipOrientation.Normal,"M 19,29L 47,29L 47,57L 19,57L 19,29 Z M 43,33L 23,33.0001L 23,53L 43,53L 43,33 Z M 39,41L 39,45L 35,45L 35,49L 31,49L 31,45L 27,45L 27,41L 31,41L 31,37L 35,37L 35,41L 39,41 Z M 24,24L 51.9999,24.0001L 51.9999,52L 48.9999,52.0001L 48.9999,27.0001L 24,27.0001L 24,24 Z M 53.9999,47L 53.9999,22.0001L 29,22L 29,19L 56.9999,19.0001L 57,47L 53.9999,47 Z")  },
            { PackIconKind.UserMinus, new PathInfo (PackIconFlipOrientation.Normal,"M21,10.5H17a1,1,0,0,0,0,2h4a1,1,0,0,0,0-2Zm-7.7,1.72A4.92,4.92,0,0,0,15,8.5a5,5,0,0,0-10,0,4.92,4.92,0,0,0,1.7,3.72A8,8,0,0,0,2,19.5a1,1,0,0,0,2,0,6,6,0,0,1,12,0,1,1,0,0,0,2,0A8,8,0,0,0,13.3,12.22ZM10,11.5a3,3,0,1,1,3-3A3,3,0,0,1,10,11.5Z")  },
            { PackIconKind.UserPlus, new PathInfo (PackIconFlipOrientation.Normal,"M21,10.5H20v-1a1,1,0,0,0-2,0v1H17a1,1,0,0,0,0,2h1v1a1,1,0,0,0,2,0v-1h1a1,1,0,0,0,0-2Zm-7.7,1.72A4.92,4.92,0,0,0,15,8.5a5,5,0,0,0-10,0,4.92,4.92,0,0,0,1.7,3.72A8,8,0,0,0,2,19.5a1,1,0,0,0,2,0,6,6,0,0,1,12,0,1,1,0,0,0,2,0A8,8,0,0,0,13.3,12.22ZM10,11.5a3,3,0,1,1,3-3A3,3,0,0,1,10,11.5Z")  },
            { PackIconKind.SolidUserPlus, new PathInfo (PackIconFlipOrientation.Vertical,"M192 595.115c0-85.12 64.213-149.333 149.333-149.333s149.333 64.213 149.333 149.333-64.213 149.333-149.333 149.333-149.333-64.213-149.333-149.333zM810.667 618.667h-85.333v-128h-128v-85.333h128v-128h85.333v128h128v85.333h-128zM170.667 149.333h426.667v42.667c0 117.632-95.701 213.333-213.333 213.333h-85.333c-117.632 0-213.333-95.701-213.333-213.333v-42.667h85.333z")  },
            { PackIconKind.AccountMultiplePlus, new PathInfo (PackIconFlipOrientation.Normal,"M19 17V19H7V17S7 13 13 13 19 17 19 17M16 8A3 3 0 1 0 13 11A3 3 0 0 0 16 8M19.2 13.06A5.6 5.6 0 0 1 21 17V19H24V17S24 13.55 19.2 13.06M18 5A2.91 2.91 0 0 0 17.11 5.14A5 5 0 0 1 17.11 10.86A2.91 2.91 0 0 0 18 11A3 3 0 0 0 18 5M8 10H5V7H3V10H0V12H3V15H5V12H8Z")  },
            { PackIconKind.AccountMultipleRemove, new PathInfo (PackIconFlipOrientation.Normal,"M24 17V19H21V17C21 15.45 20.3 14.06 19.18 13.06C24 13.55 24 17 24 17M18 5C19.66 5 21 6.34 21 8C21 9.66 19.66 11 18 11C17.69 11 17.38 10.95 17.1 10.86C17.67 10.05 18 9.07 18 8C18 6.94 17.67 5.95 17.1 5.14C17.38 5.05 17.69 5 18 5M13 5C14.66 5 16 6.34 16 8C16 9.66 14.66 11 13 11C11.34 11 10 9.66 10 8C10 6.34 11.34 5 13 5M19 17V19H7V17C7 14.79 9.69 13 13 13C16.31 13 19 14.79 19 17M.464 13.12L2.59 11L.464 8.88L1.88 7.46L4 9.59L6.12 7.46L7.54 8.88L5.41 11L7.54 13.12L6.12 14.54L4 12.41L1.88 14.54Z")  },
            { PackIconKind.AccountGroup, new PathInfo (PackIconFlipOrientation.Normal,"M12,5.5A3.5,3.5 0 0,1 15.5,9A3.5,3.5 0 0,1 12,12.5A3.5,3.5 0 0,1 8.5,9A3.5,3.5 0 0,1 12,5.5M5,8C5.56,8 6.08,8.15 6.53,8.42C6.38,9.85 6.8,11.27 7.66,12.38C7.16,13.34 6.16,14 5,14A3,3 0 0,1 2,11A3,3 0 0,1 5,8M19,8A3,3 0 0,1 22,11A3,3 0 0,1 19,14C17.84,14 16.84,13.34 16.34,12.38C17.2,11.27 17.62,9.85 17.47,8.42C17.92,8.15 18.44,8 19,8M5.5,18.25C5.5,16.18 8.41,14.5 12,14.5C15.59,14.5 18.5,16.18 18.5,18.25V20H5.5V18.25M0,20V18.5C0,17.11 1.89,15.94 4.45,15.6C3.86,16.28 3.5,17.22 3.5,18.25V20H0M24,20H20.5V18.25C20.5,17.22 20.14,16.28 19.55,15.6C22.11,15.94 24,17.11 24,18.5V20Z")  },
            { PackIconKind.FileAdd, new PathInfo (PackIconFlipOrientation.Vertical,"M768 0h-640v896h384v-256h256v-64h64v128l-256 256h-512v-1024h768v128h-64v-64zM576 896l192-192h-192v192zM832 512h-128v-128h-128v-128h128v-128h128v128h128v128h-128v128z")  },
            { PackIconKind.FileOutline, new PathInfo (PackIconFlipOrientation.Vertical,"M640 960h-512v-1024h768v768l-256 256zM576 640h256v-640h-640v896h384v-256zM640 704v192l192-192h-192z")  },
            { PackIconKind.FileSearch, new PathInfo (PackIconFlipOrientation.Vertical,"M768 97.92v-97.92h-640v896h384v-256h256v-35.84c24.73-14.631 45.845-32.282 63.738-52.813l0.262 152.653-256 256h-512v-1024h768v97.92zM576 896l192-192h-192v192zM945.92 145.92l-122.88 122.88c-5.725 5.447-12.859 9.465-20.798 11.452 18.665 29.336 29.953 65.126 29.953 103.553 0 106.039-85.961 192-192 192s-192-85.961-192-192c0-106.039 85.961-192 192-192 38.427 0 74.217 11.289 104.235 30.73 1.305-8.716 5.323-15.85 10.789-21.595l122.862-122.86c8.549-7.982 20.063-12.883 32.723-12.883 26.51 0 48 21.49 48 48 0 12.659-4.901 24.174-12.908 32.751zM640 256c-70.692 0-128 57.308-128 128s57.308 128 128 128c70.692 0 128-57.308 128-128s-57.308-128-128-128z")  },
            { PackIconKind.FileProcess, new PathInfo (PackIconFlipOrientation.Vertical,"M768 960h-448v-384h44.8l12.8-44.8 6.4-6.4v371.2h320v-256h256v-576h-384l19.2-32-32-32h460.8v704l-256 256zM768 704v192l192-192h-192zM352 224c0-35.346-28.654-64-64-64s-64 28.654-64 64c0 35.346 28.654 64 64 64s64-28.654 64-64zM505.6 166.4l70.4 25.6v64l-70.4 25.6c-6.4 19.2-12.8 38.4-25.6 57.6l32 64-44.8 44.8-64-32c-19.2 12.8-38.4 19.2-57.6 25.6l-25.6 70.4h-64l-25.6-70.4c-19.2-6.4-38.4-12.8-57.6-25.6l-64 32-44.8-44.8 32-70.4c-12.8-19.2-19.2-38.4-25.6-57.6l-70.4-19.2v-64l70.4-25.6c6.4-19.2 12.8-38.4 25.6-57.6l-32-64 44.8-44.8 70.4 32c19.2-12.8 38.4-19.2 57.6-25.6l19.2-70.4h64l25.6 70.4c19.2 6.4 38.4 12.8 57.6 25.6l64-32 44.8 44.8-32 70.4c12.8 12.8 19.2 32 25.6 51.2zM288 96c-70.4 0-128 57.6-128 128s57.6 128 128 128 128-57.6 128-128c0-70.4-57.6-128-128-128z")  },
            { PackIconKind.FileRemove, new PathInfo (PackIconFlipOrientation.Vertical,"M768 0h-640v896h384v-256h256v-165.76l64 64v165.76l-256 256h-512v-1024h768v165.76l-64 64v-165.76zM576 896l192-192h-192v192zM960 448l-64 64-128-128-128 128-64-64 128-128-128-128 64-64 128 128 128-128 64 64-128 128 128 128z")  },
            { PackIconKind.FileTextOutline, new PathInfo (PackIconFlipOrientation.Vertical,"M640 960h-512v-1024h768v768l-256 256zM576 640h256v-640h-640v896h384v-256zM640 704v192l192-192h-192zM256 512h512v-64h-512v64zM256 384h512v-64h-512v64zM256 256h512v-64h-512v64z")  },
            { PackIconKind.FileZip, new PathInfo (PackIconFlipOrientation.Vertical,"M640 960h-512v-1024h768v768l-256 256zM576 0h-256v179.2l44.8 140.8h153.6l57.6-140.8v-179.2zM832 0h-192v192l-64 192h-128v64h-128v-64l-64-192v-192h-64v896h256v-64h128v-64h-128v-64h128v-64h256v-640zM640 704v192l192-192h-192zM320 576h128v-64h-128v64zM320 832h128v-64h-128v64zM320 704h128v-64h-128v64zM448 640h128v-64h-128v64zM448 512h128v-64h-128v64zM384 192h128v-128h-128v128z")  },
            { PackIconKind.Folder, new PathInfo (PackIconFlipOrientation.Vertical,"M181.25 162.5H96.375L85.75 173.125L81.375 175H18.875L12.625 168.75V31.25L18.875 25H181.375L187.625 31.25V156.25L181.25 162.5zM174.875 56.375V37.5H24.875V112.5H81L85.375 114.375L96.125 125.125H175V106.375L174.875 56.375zM174.875 137.5H93.625L89.25 135.625L78.5 124.875H25V162.375H78.625L89.25 151.75L93.75 149.875H175L174.875 137.5z")  },
            { PackIconKind.NewFolder, new PathInfo (PackIconFlipOrientation.Vertical,"M181.25 175H96.375L85.75 185.625L81.375 187.5H18.875L12.625 181.25V43.75L18.875 37.5H87.5V50H24.875V125H81L85.375 126.875L96.125 137.625H175V118.875L174.9875 112.5H187.625V168.75L181.25 175zM174.875 150H93.625L89.25 148.125L78.5 137.375H25V174.875H78.625L89.25 164.25L93.75 162.375H175L174.875 150zM162.5 0H150V37.5H112.5V50H150V87.5H162.5V50H200V37.5H162.5V0z")  },
            { PackIconKind.FolderOutline, new PathInfo (PackIconFlipOrientation.Vertical,"M448 704l-64 128h-256l-64-128h-64v-704h1024v704h-576zM960 64h-896v576h38.4l64 128h166.4l76.8-128h550.4v-576z")  },
            { PackIconKind.FolderAdd, new PathInfo (PackIconFlipOrientation.Vertical,"M896 576v128h-448l-64 128h-256l-64-128h-64v-704h896v64h-832v576h39.68l64 128h164.48l76.16-128h423.68v-64h64zM896 512h-128v-128h-128v-128h128v-128h128v128h128v128h-128v128z")  },
            { PackIconKind.FolderOutlinepenOutline, new PathInfo (PackIconFlipOrientation.Vertical,"M896 576v128h-448l-64 128h-256l-64-128h-64v-704h896l128 576h-128zM953.6 512l-102.4-448-761.6 6.4 147.2 441.6h716.8zM64 640h38.4l64 128h166.4l76.8-128h422.4v-64h-640l-128-377.6v441.6z")  },
            { PackIconKind.FolderRemove, new PathInfo (PackIconFlipOrientation.Vertical,"M832 165.76v-101.76h-768v576h39.68l64 128h164.48l76.16-128h423.68v-165.76l64 64v165.76h-448l-64 128h-256l-64-128h-64v-704h896v101.76l-64 64zM1024 448l-64 64-128-128-128 128-64-64 128-128-128-128 64-64 128 128 128-128 64 64-128 128 128 128z")  },
            { PackIconKind.FolderSearch, new PathInfo (PackIconFlipOrientation.Vertical,"M832 97.92v-33.92h-768v576h39.68l64 128h164.48l76.16-128h423.68v-58.24c24.665-11.444 45.808-26.035 64.077-43.593l-0.077 165.833h-448l-64 128h-256l-64-128h-64v-704h896v33.92zM1009.92 145.92l-122.88 122.88c-5.725 5.447-12.859 9.465-20.798 11.452 18.665 29.336 29.953 65.126 29.953 103.553 0 106.039-85.961 192-192 192s-192-85.961-192-192c0-106.039 85.961-192 192-192 38.427 0 74.217 11.289 104.235 30.73 1.305-8.716 5.323-15.85 10.789-21.595l122.862-122.86c8.549-7.982 20.063-12.883 32.723-12.883 26.51 0 48 21.49 48 48 0 12.659-4.901 24.174-12.908 32.751zM704 256c-70.692 0-128 57.308-128 128s57.308 128 128 128c70.692 0 128-57.308 128-128s-57.308-128-128-128z")  },
            { PackIconKind.Play, new PathInfo (PackIconFlipOrientation.Vertical,"M128 896v-896l768 448z")  },
            { PackIconKind.PlayiOS, new PathInfo (PackIconFlipOrientation.Vertical,"M128 104.3v303.4c0 6.4 6.5 10.4 11.7 7.2l240.5-151.7c5.1-3.2 5.1-11.1 0-14.3L139.7 97.2c-5.2-3.3-11.7.7-11.7 7.1z")  },
            { PackIconKind.CogOutline, new PathInfo (PackIconFlipOrientation.Vertical,"M12,8A4,4 0 0,1 16,12A4,4 0 0,1 12,16A4,4 0 0,1 8,12A4,4 0 0,1 12,8M12,10A2,2 0 0,0 10,12A2,2 0 0,0 12,14A2,2 0 0,0 14,12A2,2 0 0,0 12,10M10,22C9.75,22 9.54,21.82 9.5,21.58L9.13,18.93C8.5,18.68 7.96,18.34 7.44,17.94L4.95,18.95C4.73,19.03 4.46,18.95 4.34,18.73L2.34,15.27C2.21,15.05 2.27,14.78 2.46,14.63L4.57,12.97L4.5,12L4.57,11L2.46,9.37C2.27,9.22 2.21,8.95 2.34,8.73L4.34,5.27C4.46,5.05 4.73,4.96 4.95,5.05L7.44,6.05C7.96,5.66 8.5,5.32 9.13,5.07L9.5,2.42C9.54,2.18 9.75,2 10,2H14C14.25,2 14.46,2.18 14.5,2.42L14.87,5.07C15.5,5.32 16.04,5.66 16.56,6.05L19.05,5.05C19.27,4.96 19.54,5.05 19.66,5.27L21.66,8.73C21.79,8.95 21.73,9.22 21.54,9.37L19.43,11L19.5,12L19.43,13L21.54,14.63C21.73,14.78 21.79,15.05 21.66,15.27L19.66,18.73C19.54,18.95 19.27,19.04 19.05,18.95L16.56,17.95C16.04,18.34 15.5,18.68 14.87,18.93L14.5,21.58C14.46,21.82 14.25,22 14,22H10M11.25,4L10.88,6.61C9.68,6.86 8.62,7.5 7.85,8.39L5.44,7.35L4.69,8.65L6.8,10.2C6.4,11.37 6.4,12.64 6.8,13.8L4.68,15.36L5.43,16.66L7.86,15.62C8.63,16.5 9.68,17.14 10.87,17.38L11.24,20H12.76L13.13,17.39C14.32,17.14 15.37,16.5 16.14,15.62L18.57,16.66L19.32,15.36L17.2,13.81C17.6,12.64 17.6,11.37 17.2,10.2L19.31,8.65L18.56,7.35L16.15,8.39C15.38,7.5 14.32,6.86 13.12,6.62L12.75,4H11.25Z")  },
            { PackIconKind.Fire, new PathInfo (PackIconFlipOrientation.Normal,"M17.66 11.2C17.43 10.9 17.15 10.64 16.89 10.38C16.22 9.78 15.46 9.35 14.82 8.72C13.33 7.26 13 4.85 13.95 3C13 3.23 12.17 3.75 11.46 4.32C8.87 6.4 7.85 10.07 9.07 13.22C9.11 13.32 9.15 13.42 9.15 13.55C9.15 13.77 9 13.97 8.8 14.05C8.57 14.15 8.33 14.09 8.14 13.93C8.08 13.88 8.04 13.83 8 13.76C6.87 12.33 6.69 10.28 7.45 8.64C5.78 10 4.87 12.3 5 14.47C5.06 14.97 5.12 15.47 5.29 15.97C5.43 16.57 5.7 17.17 6 17.7C7.08 19.43 8.95 20.67 10.96 20.92C13.1 21.19 15.39 20.8 17.03 19.32C18.86 17.66 19.5 15 18.56 12.72L18.43 12.46C18.22 12 17.66 11.2 17.66 11.2M14.5 17.5C14.22 17.74 13.76 18 13.4 18.1C12.28 18.5 11.16 17.94 10.5 17.28C11.69 17 12.4 16.12 12.61 15.23C12.78 14.43 12.46 13.77 12.33 13C12.21 12.26 12.23 11.63 12.5 10.94C12.69 11.32 12.89 11.7 13.13 12C13.9 13 15.11 13.44 15.37 14.8C15.41 14.94 15.43 15.08 15.43 15.23C15.46 16.05 15.1 16.95 14.5 17.5H14.5Z")  },
            { PackIconKind.FireAlert, new PathInfo (PackIconFlipOrientation.Normal,"M15.66 11.2C15.43 10.9 15.15 10.64 14.89 10.38C14.22 9.78 13.46 9.35 12.82 8.72C11.33 7.26 11 4.85 11.95 3C11 3.23 10.17 3.75 9.46 4.32C6.87 6.4 5.85 10.07 7.07 13.22C7.11 13.32 7.15 13.42 7.15 13.55C7.15 13.77 7 13.97 6.8 14.05C6.57 14.15 6.33 14.09 6.14 13.93C6.08 13.88 6.04 13.83 6 13.76C4.87 12.33 4.69 10.28 5.45 8.64C3.78 10 2.87 12.3 3 14.47C3.06 14.97 3.12 15.47 3.29 15.97C3.43 16.57 3.7 17.17 4 17.7C5.08 19.43 6.95 20.67 8.96 20.92C11.1 21.19 13.39 20.8 15.03 19.32C16.86 17.66 17.5 15 16.56 12.72L16.43 12.46C16.22 12 15.66 11.2 15.66 11.2M12.5 17.5C12.22 17.74 11.76 18 11.4 18.1C10.28 18.5 9.16 17.94 8.5 17.28C9.69 17 10.4 16.12 10.61 15.23C10.78 14.43 10.46 13.77 10.33 13C10.21 12.26 10.23 11.63 10.5 10.94C10.69 11.32 10.89 11.7 11.13 12C11.9 13 13.11 13.44 13.37 14.8C13.41 14.94 13.43 15.08 13.43 15.23C13.46 16.05 13.1 16.95 12.5 17.5H12.5M21 13H19V7H21V13M21 17H19V15H21V17Z")  },
            { PackIconKind.Pause, new PathInfo (PackIconFlipOrientation.Normal,"M1536 1344v-1408c0 -35 -29 -64 -64 -64h-512c-35 0 -64 29 -64 64v1408c0 35 29 64 64 64h512c35 0 64 -29 64 -64zM640 1344v-1408c0 -35 -29 -64 -64 -64h-512c-35 0 -64 29 -64 64v1408c0 35 29 64 64 64h512c35 0 64 -29 64 -64z")  },
            { PackIconKind.PauseiOS, new PathInfo (PackIconFlipOrientation.Normal,"M199.9 416h-63.8c-4.5 0-8.1-3.6-8.1-8V104c0-4.4 3.6-8 8.1-8h63.8c4.5 0 8.1 3.6 8.1 8v304c0 4.4-3.6 8-8.1 8zM375.9 416h-63.8c-4.5 0-8.1-3.6-8.1-8V104c0-4.4 3.6-8 8.1-8h63.8c4.5 0 8.1 3.6 8.1 8v304c0 4.4-3.6 8-8.1 8z")  },
            { PackIconKind.Stop, new PathInfo (PackIconFlipOrientation.Normal,"M1536 1344v-1408c0 -35 -29 -64 -64 -64h-1408c-35 0 -64 29 -64 64v1408c0 35 29 64 64 64h1408c35 0 64 -29 64 -64z")  },
            { PackIconKind.SquareiOS, new PathInfo (PackIconFlipOrientation.Normal,"M388 416H124c-15.4 0-28-12.6-28-28V124c0-15.4 12.6-28 28-28h264c15.4 0 28 12.6 28 28v264c0 15.4-12.6 28-28 28z")  },
            { PackIconKind.Refresh, new PathInfo (PackIconFlipOrientation.Normal,"M9.77,15.53c0,0.94,0.24,1.82,0.71,2.62c0.47,0.8,1.11,1.44,1.91,1.9c0.8,0.47,1.67,0.7,2.61,0.7  c0.96,0,1.83-0.23,2.63-0.69c0.8-0.46,1.43-1.09,1.89-1.89c0.46-0.8,0.69-1.68,0.69-2.63c0-0.24-0.08-0.44-0.24-0.61  c-0.16-0.17-0.35-0.25-0.59-0.25c-0.23,0-0.43,0.08-0.6,0.25c-0.17,0.17-0.25,0.37-0.25,0.61c0,0.98-0.35,1.82-1.04,2.51  c-0.69,0.69-1.53,1.04-2.51,1.04c-0.97,0-1.79-0.35-2.47-1.04c-0.68-0.69-1.02-1.53-1.02-2.51c0-0.85,0.26-1.62,0.79-2.31  s1.14-1.06,1.84-1.1l-0.38,0.37c-0.16,0.18-0.24,0.37-0.24,0.58c0,0.22,0.08,0.42,0.24,0.6c0.36,0.35,0.77,0.35,1.21,0l1.84-1.82  c0.16-0.12,0.24-0.33,0.24-0.62c0-0.26-0.08-0.45-0.24-0.57L14.97,8.8c-0.18-0.16-0.37-0.24-0.57-0.24c-0.25,0-0.46,0.08-0.63,0.25  c-0.17,0.17-0.25,0.37-0.25,0.6c0,0.24,0.08,0.45,0.24,0.61l0.38,0.36c-1.25,0.22-2.29,0.82-3.12,1.8S9.77,14.27,9.77,15.53z")  },
            { PackIconKind.RefreshiOS, new PathInfo (PackIconFlipOrientation.Normal,"M433 288.8c-7.7 0-14.3 5.9-14.9 13.6-6.9 83.1-76.8 147.9-161.8 147.9-89.5 0-162.4-72.4-162.4-161.4 0-87.6 70.6-159.2 158.2-161.4 2.3-.1 4.1 1.7 4.1 4v50.3c0 12.6 13.9 20.2 24.6 13.5L377 128c10-6.3 10-20.8 0-27.1l-96.1-66.4c-10.7-6.7-24.6.9-24.6 13.5v45.7c0 2.2-1.7 4-3.9 4C148 99.8 64 184.6 64 288.9 64 394.5 150.1 480 256.3 480c100.8 0 183.4-76.7 191.6-175.1.8-8.7-6.2-16.1-14.9-16.1z")  },
            { PackIconKind.AddiOS, new PathInfo (PackIconFlipOrientation.Normal,"M368.5 240H272v-96.5c0-8.8-7.2-16-16-16s-16 7.2-16 16V240h-96.5c-8.8 0-16 7.2-16 16 0 4.4 1.8 8.4 4.7 11.3 2.9 2.9 6.9 4.7 11.3 4.7H240v96.5c0 4.4 1.8 8.4 4.7 11.3 2.9 2.9 6.9 4.7 11.3 4.7 8.8 0 16-7.2 16-16V272h96.5c8.8 0 16-7.2 16-16s-7.2-16-16-16z")  },
            { PackIconKind.Add, new PathInfo (PackIconFlipOrientation.Normal,"M405 235h-128v-128h-42v128h-128v42h128v128h42v-128h128v-42z")  },
            { PackIconKind.AddBox, new PathInfo (PackIconFlipOrientation.Normal,"M363 235v42h-86v86h-42v-86h-86v-42h86v-86h42v86h86zM405 448c23 0 43 -20 43 -43v-298c0 -23 -20 -43 -43 -43h-298c-24 0 -43 20 -43 43v298c0 23 19 43 43 43h298z")  },
            { PackIconKind.AddCircle, new PathInfo (PackIconFlipOrientation.Normal,"M363 235v42h-86v86h-42v-86h-86v-42h86v-86h42v86h86zM256 469c118 0 213 -95 213 -213s-95 -213 -213 -213s-213 95 -213 213s95 213 213 213z")  },
            { PackIconKind.AddCircleOutline, new PathInfo (PackIconFlipOrientation.Normal,"M256 85c94 0 171 77 171 171s-77 171 -171 171s-171 -77 -171 -171s77 -171 171 -171zM256 469c118 0 213 -95 213 -213s-95 -213 -213 -213s-213 95 -213 213s95 213 213 213zM277 363v-86h86v-42h-86v-86h-42v86h-86v42h86v86h42z")  },
            { PackIconKind.RemoveiOS, new PathInfo (PackIconFlipOrientation.Normal,"M368.5 240h-225c-8.8 0-16 7.2-16 16 0 4.4 1.8 8.4 4.7 11.3 2.9 2.9 6.9 4.7 11.3 4.7h225c8.8 0 16-7.2 16-16s-7.2-16-16-16z")  },
            { PackIconKind.Remove, new PathInfo (PackIconFlipOrientation.Normal,"M405 235h-298v42h298v-42z")  },
            { PackIconKind.RemoveCircle, new PathInfo (PackIconFlipOrientation.Normal,"M363 235v42h-214v-42h214zM256 469c118 0 213 -95 213 -213s-95 -213 -213 -213s-213 95 -213 213s95 213 213 213z")  },
            { PackIconKind.RemoveCircleOutline, new PathInfo (PackIconFlipOrientation.Normal,"M256 85c94 0 171 77 171 171s-77 171 -171 171s-171 -77 -171 -171s77 -171 171 -171zM256 469c118 0 213 -95 213 -213s-95 -213 -213 -213s-213 95 -213 213s95 213 213 213zM149 277h214v-42h-214v42z")  },
            { PackIconKind.IndeterminateCheckBox, new PathInfo (PackIconFlipOrientation.Normal,"M363 235v42h-86v86h-42v-86h-86v-42h86v-86h42v86h86zM405 448c23 0 43 -20 43 -43v-298c0 -23 -20 -43 -43 -43h-298c-24 0 -43 20 -43 43v298c0 23 19 43 43 43h298z")  },
            { PackIconKind.CopyiOS, new PathInfo (PackIconFlipOrientation.Normal,"M 116,412 V 80 H 96 C 78.4,80 64,94.4 64,112 v 352 c 0,17.6 14.4,32 32,32 h 256 c 17.6,0 32,-14.4 32,-32 V 444 H 148 c -17.6,0 -32,-14.4 -32,-32 z M 307,96 V 16 H 176 c -17.6,0 -32,14.4 -32,32 v 336 c 0,17.6 14.4,32 32,32 h 240 c 17.6,0 32,-14.4 32,-32 V 141 h -96 c -24.8,0 -45,-20.2 -45,-45 z m 45,19 h 90 c 3.3,0 6,-2.7 6,-6 0,-8.2 -3.7,-16 -10,-21.3 L 360.9,23.5 c -4.9,-4.1 -14.2,-7.4 -20.6,-7.4 -4.1,0 -7.4,3.3 -7.4,7.4 V 96 c 0.1,10.5 8.6,19 19.1,19 z")  },
            { PackIconKind.FileCopyLine, new PathInfo (PackIconFlipOrientation.Vertical,"M350 900V1050A50 50 0 0 0 400 1100H1000A50 50 0 0 0 1050 1050V350A50 50 0 0 0 1000 300H850V150C850 122.4000000000001 827.5 100 799.65 100H200.35A50.05 50.05 0 0 0 150 150L150.15 850C150.15 877.5999999999999 172.65 900 200.5 900H350zM250.15 800L250 200H750V800H250.15zM450 900H850V400H950V1000H450V900z")  },
            { PackIconKind.Paste, new PathInfo (PackIconFlipOrientation.Vertical,"M832 704h-192v256h-640v-896h384v-128h640v576l-192 192zM192 896h256v-64h-256v64zM960 0h-512v640h320v-192h192v-448zM832 512v128l128-128h-128z")  },
            { PackIconKind.BuildiOS, new PathInfo (PackIconFlipOrientation.Normal,"M441.1 131.1l-44.9 45.1c-.9.9-2.3 1.3-3.5 1.1l-46.4-8.4c-1.6-.3-2.9-1.6-3.2-3.2l-8.3-46.4c-.2-1.3.2-2.6 1.1-3.5l44.8-45c3.5-3.5 3-9.3-1-12.1-10.1-7.2-22.1-10.7-31.8-10.7-.7 0-1.4 0-2 .1-12.5.7-39.3 7.7-60 29.7-20.1 21.2-41.1 60.6-22.5 104.5 2.2 5.3 4.7 12.3-2.7 19.7C253.1 209.4 61 390.3 61 390.3c-18 15.5-16.7 44.2-.1 60.9 8.5 8.4 20 12.8 31.3 12.8 11.1 0 21.9-4.2 29.6-13.1 0 0 179.4-191.1 188.2-199.8 4-3.9 7.7-5.1 11.1-5.1 3.3 0 6.3 1.2 8.6 2.4 9.9 5.1 21 7.4 32.4 7.4 26.8 0 55-12.4 72.2-29.6 24.4-24.4 28.9-48 29.6-60.1.6-9.9-2.2-22.6-10.7-34.2-2.9-3.8-8.6-4.2-12.1-.8zM102.5 429.3c-5.5 5.4-14.4 5.4-19.9 0-5.4-5.5-5.4-14.4 0-19.9 5.5-5.4 14.4-5.4 19.9 0 5.4 5.6 5.4 14.5 0 19.9z")  },
            { PackIconKind.CheckmarkiOS, new PathInfo (PackIconFlipOrientation.Normal,"M362.6 192.9L345 174.8c-.7-.8-1.8-1.2-2.8-1.2-1.1 0-2.1.4-2.8 1.2l-122 122.9-44.4-44.4c-.8-.8-1.8-1.2-2.8-1.2-1 0-2 .4-2.8 1.2l-17.8 17.8c-1.6 1.6-1.6 4.1 0 5.7l56 56c3.6 3.6 8 5.7 11.7 5.7 5.3 0 9.9-3.9 11.6-5.5h.1l133.7-134.4c1.4-1.7 1.4-4.2-.1-5.7z")  },
            { PackIconKind.CloseiOS, new PathInfo (PackIconFlipOrientation.Normal,"M278.6 256l68.2-68.2c6.2-6.2 6.2-16.4 0-22.6-6.2-6.2-16.4-6.2-22.6 0L256 233.4l-68.2-68.2c-6.2-6.2-16.4-6.2-22.6 0-3.1 3.1-4.7 7.2-4.7 11.3 0 4.1 1.6 8.2 4.7 11.3l68.2 68.2-68.2 68.2c-3.1 3.1-4.7 7.2-4.7 11.3 0 4.1 1.6 8.2 4.7 11.3 6.2 6.2 16.4 6.2 22.6 0l68.2-68.2 68.2 68.2c6.2 6.2 16.4 6.2 22.6 0 6.2-6.2 6.2-16.4 0-22.6L278.6 256z")  },
            { PackIconKind.FingerPrintiOS, new PathInfo (PackIconFlipOrientation.Normal,"m 265.2,245.9 c -2.1,-5.1 -7.1,-8.5 -12.7,-8.5 -1.8,0 -3.6,0.4 -5.2,1 -7,2.9 -10.3,10.9 -7.4,17.9 6.6,16 11.8,46.2 14.1,82.8 2.4,36.9 1.7,76.9 -2,109.6 -0.4,3.6 0.6,7.2 2.9,10.1 2.3,2.9 5.5,4.6 9.2,5 0.5,0.1 1,0.1 1.5,0.1 7,0 12.8,-5.2 13.6,-12.2 3.9,-35.5 4.6,-76.6 2,-115.8 -2.5,-39.3 -8.2,-71.3 -16,-90 z m 29.4,-40 c -11.2,-10.5 -25.2,-16.1 -40.5,-16.1 -19.8,0 -36.7,7.3 -47.6,20.5 -8.3,10.1 -17,28.8 -10.9,60 12.3,62.5 15,121.6 8.1,175.6 -1,7.5 4.4,14.4 11.8,15.3 0.6,0.1 1.2,0.1 1.7,0.1 6.9,0 12.7,-5.1 13.6,-12 7.3,-57 4.5,-119 -8.3,-184.4 -3.2,-16.4 -1.4,-29.3 5.1,-37.3 5.6,-6.8 15,-10.5 26.4,-10.5 25,0 35.1,27.6 38.3,39.4 6.9,25.7 10.9,63 11.5,107.7 0.1,7.4 6.2,13.5 13.7,13.5 h 0.2 c 3.6,-0.1 7,-1.5 9.6,-4.1 2.5,-2.6 3.9,-6.1 3.9,-9.7 -0.7,-47 -5,-86.5 -12.5,-114.4 -4.9,-18.5 -13.1,-33.2 -24.1,-43.6 z m 78.9,61.6 c -5.9,-37.5 -19.9,-68.8 -40.6,-90.6 -20.8,-22 -47.4,-33.7 -76.9,-33.7 -19,0 -37.7,4.1 -54.1,12 -3.7,1.8 -6.4,5.1 -7.4,9.1 -1,4.1 -0.1,8.3 2.5,11.6 2.6,3.4 6.5,5.3 10.8,5.3 2.1,0 4,-0.5 5.9,-1.3 12.6,-6 27.2,-9.2 42.2,-9.2 22.4,0 42.5,9.2 58.3,26.6 16.1,17.8 27.3,43.6 32.1,74.6 4.7,29.6 7,53.5 7.1,73.1 0.2,39.7 -4.8,72.7 -4.8,73.1 -0.6,3.6 0.3,7.2 2.5,10.2 2.2,3 5.3,4.9 8.9,5.5 0.7,0.1 1.4,0.2 2.1,0.2 6.8,0 12.5,-4.9 13.5,-11.6 0.2,-1.4 5.4,-35.2 5.2,-77.5 0,-21.3 -2.4,-46.5 -7.3,-77.4 z m -188.2,-64 c 4,-5.8 2.9,-13.6 -2.5,-18.2 -2.5,-2.1 -5.6,-3.2 -8.8,-3.2 -4.5,0 -8.7,2.2 -11.3,5.9 -14.7,21.5 -19.7,49.1 -14.4,79.8 8.9,51.3 16.9,111.1 9.4,165 -0.5,3.8 0.5,7.7 2.9,10.7 2.3,3 5.7,4.8 9.4,5.1 0.4,0 0.8,0.1 1.2,0.1 6.8,0 12.6,-5.1 13.5,-11.8 8.2,-57.7 -0.2,-120.2 -9.5,-173.8 -4.1,-23.8 -0.7,-43.9 10.1,-59.6 z M 317.5,388 H 317 c -7.6,0.3 -13.5,6.6 -13.2,14.2 0,0.2 0.6,17.9 -2.6,34.7 -1.3,6.9 2.6,13.7 9.1,15.8 1.4,0.4 2.8,0.7 4.3,0.7 6.6,0 12.2,-4.7 13.4,-11.1 3.8,-20.1 3.1,-40.2 3.1,-41.1 -0.2,-7.5 -6.2,-13.2 -13.6,-13.2 z M 355.7,129.8 C 328.4,106.1 295,93.6 259,93.6 c -48.3,0 -91.4,17.8 -121.5,50.1 -28.7,30.8 -42.8,71.7 -39.7,115.1 2.3,32.7 6,50.7 9.3,66.6 4.3,21.1 7.7,37.8 5.1,84.1 -0.4,6.7 3.7,12.7 10,14.6 1.2,0.4 2.3,0.5 3.6,0.5 7.2,0 13.2,-5.7 13.7,-12.9 2.9,-50.4 -0.8,-68.7 -5.5,-91.9 -3.1,-15.1 -6.6,-32.2 -8.8,-63.1 -2.6,-35.7 9,-69.3 32.4,-94.5 24.8,-26.7 60.9,-41.4 101.4,-41.4 29.3,0 56.5,10.2 78.7,29.5 22.3,19.3 39.2,47.4 49,81.1 11.4,39.3 14.5,89.3 9.1,144.5 -0.7,7.5 4.8,14.2 12.3,15 0.4,0 0.9,0.1 1.3,0.1 7.1,0 12.9,-5.3 13.6,-12.4 5.8,-58.7 2.3,-112.2 -10.1,-154.8 -11.1,-38.6 -30.9,-71.2 -57.2,-94 z m 95.1,52.2 c -8.6,-24.4 -20.3,-44.9 -33,-57.5 -2.6,-2.6 -6,-4 -9.7,-4 -3.7,0 -7.1,1.4 -9.7,4 -5.3,5.3 -5.3,14 0,19.4 9.9,9.9 19.6,27.4 26.8,48.1 7.3,21.2 11.4,43.6 11.4,63.1 0,3 -0.1,7.6 -0.2,10.1 -0.2,3.7 1.1,7.1 3.6,9.8 2.5,2.7 5.8,4.3 9.5,4.4 h 0.6 c 7.3,0 13.3,-5.7 13.7,-13.1 0.1,-3.1 0.2,-8.1 0.2,-11.3 0,-22.8 -4.7,-48.8 -13.2,-73 z M 143,110.6 c 2.9,0 5.6,-0.9 8,-2.6 29.9,-21.4 66.2,-32.7 105,-32.7 40.8,0 80.1,14.8 113.7,42.8 2.5,2 5.6,3.2 8.8,3.2 4.1,0 7.9,-1.8 10.5,-4.9 4.8,-5.8 4,-14.4 -1.7,-19.3 C 348.6,65 303.3,48 256,48 c -44.6,0 -86.4,13.1 -121,37.8 -3,2.1 -4.9,5.3 -5.5,8.9 -0.6,3.6 0.2,7.2 2.4,10.2 2.5,3.6 6.7,5.7 11.1,5.7 z M 75.4,255 c 0,-43.7 15.8,-85.8 44.5,-118.7 2.4,-2.8 3.6,-6.3 3.3,-9.9 -0.2,-3.6 -1.9,-7 -4.7,-9.4 -2.5,-2.2 -5.7,-3.4 -9,-3.4 -4,0 -7.7,1.7 -10.3,4.7 C 66.2,156.2 48,204.7 48,255 c 0,32.8 5.9,58.8 15.4,90.2 1.8,5.8 7,9.7 13.1,9.7 1.3,0 2.7,-0.2 4,-0.6 3.5,-1.1 6.4,-3.4 8.1,-6.6 1.7,-3.2 2.1,-6.9 1,-10.4 -8.8,-29 -14.2,-52.8 -14.2,-82.3 z")  },
            { PackIconKind.PrintiOS, new PathInfo (PackIconFlipOrientation.Normal,"M 432.5,112 H 80.5 C 62.9,112 48,125.8 48,143.3 V 317.7 C 48,335.2 62.9,350 80.5,350 H 96 c 4.4,0 8,-3.6 8,-8 V 236 c 0,-15.5 12.5,-28 28,-28 h 248 c 15.5,0 28,12.5 28,28 v 106 c 0,4.4 3.6,8 8,8 h 16.5 c 17.6,0 31.5,-14.8 31.5,-32.3 V 143.3 C 464,125.8 450.1,112 432.5,112 Z M 128,248 v 200 c 0,8.8 7.2,16 16,16 h 224 c 8.8,0 16,-7.2 16,-16 V 248 c 0,-8.8 -7.2,-16 -16,-16 H 144 c -8.8,0 -16,7.2 -16,16 z M 384,48 H 128 c -8.8,0 -16,7.2 -16,16 v 20 c 0,2.2 1.8,4 4,4 h 280 c 2.2,0 4,-1.8 4,-4 V 64 c 0,-8.8 -7.2,-16 -16,-16 z")  },
            { PackIconKind.SettingsiOS, new PathInfo (PackIconFlipOrientation.Normal,"M416.3 256c0-21 13.1-38.9 31.7-46.1-4.9-20.5-13-39.7-23.7-57.1-6.4 2.8-13.2 4.3-20.1 4.3-12.6 0-25.2-4.8-34.9-14.4-14.9-14.9-18.2-36.8-10.2-55-17.3-10.7-36.6-18.8-57-23.7C295 82.5 277 95.7 256 95.7S217 82.5 209.9 64c-20.5 4.9-39.7 13-57.1 23.7 8.1 18.1 4.7 40.1-10.2 55-9.6 9.6-22.3 14.4-34.9 14.4-6.9 0-13.7-1.4-20.1-4.3C77 170.3 68.9 189.5 64 210c18.5 7.1 31.7 25 31.7 46.1 0 21-13.1 38.9-31.6 46.1 4.9 20.5 13 39.7 23.7 57.1 6.4-2.8 13.2-4.2 20-4.2 12.6 0 25.2 4.8 34.9 14.4 14.8 14.8 18.2 36.8 10.2 54.9 17.4 10.7 36.7 18.8 57.1 23.7 7.1-18.5 25-31.6 46-31.6s38.9 13.1 46 31.6c20.5-4.9 39.7-13 57.1-23.7-8-18.1-4.6-40 10.2-54.9 9.6-9.6 22.2-14.4 34.9-14.4 6.8 0 13.7 1.4 20 4.2 10.7-17.4 18.8-36.7 23.7-57.1-18.4-7.2-31.6-25.1-31.6-46.2zm-159.4 79.9c-44.3 0-80-35.9-80-80s35.7-80 80-80 80 35.9 80 80-35.7 80-80 80z")  },
            { PackIconKind.WifiiOS, new PathInfo (PackIconFlipOrientation.Normal,"m 113.2,277.5 28.6,28.3 c 3.1,3 8,3.2 11.2,0.3 28.3,-25.1 64.6,-38.9 102.9,-38.9 38.3,0 74.6,13.7 102.9,38.9 3.2,2.9 8.1,2.7 11.2,-0.3 l 28.6,-28.3 c 3.3,-3.3 3.2,-8.6 -0.3,-11.7 -37.5,-33.9 -87.6,-54.6 -142.5,-54.6 -54.9,0 -105,20.7 -142.5,54.6 -3.3,3.1 -3.4,8.4 -0.1,11.7 z M 256,324.2 c -23.4,0 -44.6,9.8 -59.4,25.5 -3,3.2 -2.9,8.1 0.2,11.2 l 53.4,52.7 c 3.2,3.2 8.4,3.2 11.6,0 l 53.4,-52.7 c 3.1,-3.1 3.2,-8 0.2,-11.2 -14.8,-15.6 -36,-25.5 -59.4,-25.5 z M 256,96 c -81.5,0 -163,33.6 -221.5,88.3 -3.3,3 -3.4,8.1 -0.3,11.4 l 26.7,27.9 c 3.1,3.3 8.3,3.4 11.6,0.3 23.3,-21.6 49.9,-38.8 79.3,-51 33,-13.8 68.1,-20.7 104.3,-20.7 36.2,0 71.3,7 104.3,20.7 29.4,12.3 56,29.4 79.3,51 3.3,3.1 8.5,3 11.6,-0.3 L 478,195.7 c 3.1,-3.2 3,-8.3 -0.3,-11.4 C 419,129.6 337.5,96 256,96 Z")  },
            { PackIconKind.FeatureSearchOutline, new PathInfo (PackIconFlipOrientation.Normal,"M15.5,2C13,2 11,4 11,6.5C11,9 13,11 15.5,11C16.4,11 17.2,10.7 17.9,10.3L21,13.4L22.4,12L19.3,8.9C19.7,8.2 20,7.4 20,6.5C20,4 18,2 15.5,2M4,4A2,2 0 0,0 2,6V20A2,2 0 0,0 4,22H18A2,2 0 0,0 20,20V15L18,13V20H4V6H9.03C9.09,5.3 9.26,4.65 9.5,4H4M15.5,4C16.9,4 18,5.1 18,6.5C18,7.9 16.9,9 15.5,9C14.1,9 13,7.9 13,6.5C13,5.1 14.1,4 15.5,4Z")  },
            { PackIconKind.ClearAll, new PathInfo (PackIconFlipOrientation.Normal,"M125 42.5L133.75 33.75L153.75 53.75L173.75 33.75L183.75 42.5L162.5 62.5L183.75 82.5L173.75 92.5L153.75 71.25L133.75 92.5L125 82.5L145 62.5L125 42.5zM12.5 150H187.5V162.5H12.5V150zM12.5 112.5H187.5V125H12.5V112.5zM112.5 81.25V87.5H12.5V75H112.5V81.25zM112.5 37.5V50H12.5V37.5H112.5z")  },
            { PackIconKind.InterfacePassword, new PathInfo (PackIconFlipOrientation.Normal,"M 15.8333,25.3333L 60.1667,25.3333L 60.1667,52.25L 15.8333,52.25L 15.8333,25.3333 Z M 19,28.5L 19,49.0833L 57,49.0833L 57,28.5L 19,28.5 Z M 32.015,39.1558L 28.591,39.5833L 31.0017,42.1286L 28.7652,43.6367L 27.0829,40.4938L 25.3967,43.6367L 23.1483,42.1286L 25.5708,39.5833L 22.135,39.1558L 23.0256,36.7967L 26.1883,38.0633L 25.6817,34.5167L 28.4683,34.5167L 27.9617,38.0633L 31.1006,36.7967L 32.015,39.1558 Z M 43.6683,39.1558L 40.2444,39.5833L 42.655,42.1285L 40.4185,43.6367L 38.7362,40.4938L 37.05,43.6367L 34.8017,42.1285L 37.2242,39.5833L 33.7883,39.1558L 34.679,36.7967L 37.8417,38.0633L 37.335,34.5167L 40.1217,34.5167L 39.615,38.0633L 42.754,36.7967L 43.6683,39.1558 Z M 45.5208,47.5L 45.5208,30.0833L 47.5,30.0833L 47.5,47.5L 45.5208,47.5 Z")  },
            { PackIconKind.FormTextboxPassword, new PathInfo (PackIconFlipOrientation.Normal,"M17,7H22V17H17V19A1,1 0 0,0 18,20H20V22H17.5C16.95,22 16,21.55 16,21C16,21.55 15.05,22 14.5,22H12V20H14A1,1 0 0,0 15,19V5A1,1 0 0,0 14,4H12V2H14.5C15.05,2 16,2.45 16,3C16,2.45 16.95,2 17.5,2H20V4H18A1,1 0 0,0 17,5V7M2,7H13V9H4V15H13V17H2V7M20,15V9H17V15H20M8.5,12A1.5,1.5 0 0,0 7,10.5A1.5,1.5 0 0,0 5.5,12A1.5,1.5 0 0,0 7,13.5A1.5,1.5 0 0,0 8.5,12M13,10.89C12.39,10.33 11.44,10.38 10.88,11C10.32,11.6 10.37,12.55 11,13.11C11.55,13.63 12.43,13.63 13,13.11V10.89Z")  },
            { PackIconKind.RegularMessage, new PathInfo (PackIconFlipOrientation.Vertical,"M853.333 874.667h-682.667c-47.061 0-85.333-38.272-85.333-85.333v-512c0-47.061 38.272-85.333 85.333-85.333h128v-160.725l267.819 160.725h286.848c47.061 0 85.333 38.272 85.333 85.333v512c0 47.061-38.272 85.333-85.333 85.333zM853.333 277.333h-310.485l-158.848-95.275v95.275h-213.333v512h682.667v-512z")  },
            { PackIconKind.RegularMessageAlt, new PathInfo (PackIconFlipOrientation.Vertical,"M810.667 874.667h-597.333c-47.061 0-85.333-38.272-85.333-85.333v-512c0-47.061 38.272-85.333 85.333-85.333h153.003l145.664-145.664 145.664 145.664h153.003c47.061 0 85.333 38.272 85.333 85.333v512c0 47.061-38.272 85.333-85.333 85.333zM810.667 277.333h-188.331l-110.336-110.336-110.336 110.336h-188.331v512h597.333v-512z")  },
            { PackIconKind.MessageBulleted, new PathInfo (PackIconFlipOrientation.Normal,"M20,2H4A2,2 0 0,0 2,4V22L6,18H20A2,2 0 0,0 22,16V4A2,2 0 0,0 20,2M8,14H6V12H8V14M8,11H6V9H8V11M8,8H6V6H8V8M15,14H10V12H15V14M18,11H10V9H18V11M18,8H10V6H18V8Z")  },
            { PackIconKind.MessageBulletedOff, new PathInfo (PackIconFlipOrientation.Normal,"M1.27,1.73L0,3L2,5V22L6,18H15L20.73,23.73L22,22.46L1.27,1.73M8,14H6V12H8V14M6,11V9L8,11H6M20,2H4.08L10,7.92V6H18V8H10.08L11.08,9H18V11H13.08L20.07,18C21.14,17.95 22,17.08 22,16V4A2,2 0 0,0 20,2Z")  },
            { PackIconKind.BackspaceiOS, new PathInfo (PackIconFlipOrientation.Normal,"M395 96H230c-46.7 0-73.2 34.7-102 63.5s-72.1 74.7-72.1 74.7C51 239.8 48 247 48 255.1c0 8 3 15.3 7.9 21 0 0 34.3 37.6 72.1 75.5 37.8 37.8 56.7 64.5 102 64.5h165c38.5 0 69-32.5 69-71V165c0-38.5-30.5-69-69-69zm-17.7 212.7c6.2 6.2 6.2 16.4 0 22.6-6.2 6.2-16.4 6.2-22.6 0L302 278.6l-52.7 52.7c-6.2 6.2-16.3 6.4-22.6 0-6.4-6.4-6.7-15.9 0-22.6l52.7-52.7-52.7-52.7c-6-6-7.1-15.6 0-22.6 7.1-7.1 16.4-6.2 22.6 0l52.7 52.7 52.7-52.7c6.2-6.2 16.4-6.2 22.6 0 6.2 6.2 6.2 16.4 0 22.6L324.6 256l52.7 52.7z")  },
            { PackIconKind.TrashiOS, new PathInfo (PackIconFlipOrientation.Normal,"M133.1 128l23.6 290.7c0 16.2 13.1 29.3 29.3 29.3h141c16.2 0 29.3-13.1 29.3-29.3L379.6 128H133.1zm61.6 265L188 160h18.5l6.9 233h-18.7zm70.3 0h-18V160h18v233zm52.3 0h-18.6l6.8-233H324l-6.7 233zM364 92h-36l-26.3-23c-3.7-3.2-8.4-5-13.2-5h-64.8c-4.9 0-9.7 1.8-13.4 5L184 92h-36c-17.6 0-30 8.4-30 26h276c0-17.6-12.4-26-30-26z")  },
            { PackIconKind.ApplicationImport, new PathInfo (PackIconFlipOrientation.Normal,"M1 12H10.8L8.3 9.5L9.7 8.1L14.6 13L9.7 17.9L8.3 16.5L10.8 14H1V12M21 2H3C1.9 2 1 2.9 1 4V10.1H3V6H21V20H3V16H1V20C1 21.1 1.9 22 3 22H21C22.1 22 23 21.1 23 20V4C23 2.9 22.1 2 21 2")  },
            { PackIconKind.ApplicationExport, new PathInfo (PackIconFlipOrientation.Normal,"M9 12H18.8L16.3 9.5L17.7 8.1L22.6 13L17.7 17.9L16.3 16.5L18.8 14H9V12M21 17.4V20H3V6H21V8.6L23 10.6V4C23 2.9 22.1 2 21 2H3C1.9 2 1 2.9 1 4V20C1 21.1 1.9 22 3 22H21C22.1 22 23 21.1 23 20V15.4L21 17.4Z")  },
            { PackIconKind.CurrentAc, new PathInfo (PackIconFlipOrientation.Normal,"M12.43 11C12.28 10.84 10 7 7 7S2.32 10.18 2 11V13H11.57C11.72 13.16 14 17 17 17S21.68 13.82 22 13V11H12.43M7 9C8.17 9 9.18 9.85 10 11H4.31C4.78 10.17 5.54 9 7 9M17 15C15.83 15 14.82 14.15 14 13H19.69C19.22 13.83 18.46 15 17 15Z")  },
            { PackIconKind.CurrentDc, new PathInfo (PackIconFlipOrientation.Normal,"M2,9V11H22V9H2M2,13V15H7V13H2M9,13V15H15V13H9M17,13V15H22V13H17Z")  },
            { PackIconKind.Signal2g, new PathInfo (PackIconFlipOrientation.Normal,"M11,19.5H2V13.5A3,3 0 0,1 5,10.5H8V7.5H2V4.5H8A3,3 0 0,1 11,7.5V10.5A3,3 0 0,1 8,13.5H5V16.5H11M22,10.5H17.5V13.5H19V16.5H16V7.5H22V4.5H16A3,3 0 0,0 13,7.5V16.5A3,3 0 0,0 16,19.5H19A3,3 0 0,0 22,16.5")  },
            { PackIconKind.Signal3g, new PathInfo (PackIconFlipOrientation.Normal,"M11,16.5V14.25C11,13 10,12 8.75,12C10,12 11,11 11,9.75V7.5A3,3 0 0,0 8,4.5H2V7.5H8V10.5H5V13.5H8V16.5H2V19.5H8A3,3 0 0,0 11,16.5M22,16.5V10.5H17.5V13.5H19V16.5H16V7.5H22V4.5H16A3,3 0 0,0 13,7.5V16.5A3,3 0 0,0 16,19.5H19A3,3 0 0,0 22,16.5Z")  },
            { PackIconKind.Signal4g, new PathInfo (PackIconFlipOrientation.Normal,"M22,16.5V10.5H17.5V13.5H19V16.5H16V7.5H22V4.5H16A3,3 0 0,0 13,7.5V16.5A3,3 0 0,0 16,19.5H19A3,3 0 0,0 22,16.5M8,19.5H11V4.5H8V10.5H5V4.5H2V13.5H8V19.5Z")  },
            { PackIconKind.Signal5g, new PathInfo (PackIconFlipOrientation.Normal,"M22,16.5V10.5H17.5V13.5H19V16.5H16V7.5H22V4.5H16A3,3 0 0,0 13,7.5V16.5A3,3 0 0,0 16,19.5H19A3,3 0 0,0 22,16.5M10,4.5H3V12L3,13.5H7V16.5H3V19.5H8.5A1.5,1.5 0 0,0 10,18V12A1.5,1.5 0 0,0 8.5,10.5H6V7.5H10V4.5Z")  },
            { PackIconKind.Ethernet, new PathInfo (PackIconFlipOrientation.Normal,"M7,15H9V18H11V15H13V18H15V15H17V18H19V9H15V6H9V9H5V18H7V15M4.38,3H19.63C20.94,3 22,4.06 22,5.38V19.63A2.37,2.37 0 0,1 19.63,22H4.38C3.06,22 2,20.94 2,19.63V5.38C2,4.06 3.06,3 4.38,3Z")  },
            { PackIconKind.SerialPort, new PathInfo (PackIconFlipOrientation.Normal,"M7,3H17V5H19V8H16V14H8V8H5V5H7V3M17,9H19V14H17V9M11,15H13V22H11V15M5,9H7V14H5V9Z")  },
            { PackIconKind.EvPlugType1, new PathInfo (PackIconFlipOrientation.Normal,"M12.5 15C12.5 15.28 12.28 15.5 12 15.5S11.5 15.28 11.5 15 11.72 14.5 12 14.5 12.5 14.72 12.5 15M15 10.5C15.28 10.5 15.5 10.28 15.5 10S15.28 9.5 15 9.5 14.5 9.72 14.5 10 14.72 10.5 15 10.5M9 10.5C9.28 10.5 9.5 10.28 9.5 10S9.28 9.5 9 9.5 8.5 9.72 8.5 10 8.72 10.5 9 10.5M13 18.92V20H11V18.92C7.61 18.43 5 15.53 5 12S7.61 5.57 11 5.08V4H13V5.08C16.39 5.57 19 8.5 19 12S16.39 18.43 13 18.92M13 10C13 11.11 13.9 12 15 12S17 11.11 17 10 16.11 8 15 8 13 8.9 13 10M7 10C7 11.11 7.9 12 9 12S11 11.11 11 10 10.11 8 9 8 7 8.9 7 10M9.5 14C9.5 13.17 8.83 12.5 8 12.5S6.5 13.17 6.5 14 7.17 15.5 8 15.5 9.5 14.83 9.5 14M14 15C14 13.9 13.11 13 12 13S10 13.9 10 15 10.9 17 12 17 14 16.11 14 15M16 15.5C16.83 15.5 17.5 14.83 17.5 14S16.83 12.5 16 12.5 14.5 13.17 14.5 14 15.17 15.5 16 15.5Z")  },
            { PackIconKind.EvPlugType2, new PathInfo (PackIconFlipOrientation.Normal,"M7.5 11C7.5 11.28 7.28 11.5 7 11.5S6.5 11.28 6.5 11 6.72 10.5 7 10.5 7.5 10.72 7.5 11M9.5 14.5C9.22 14.5 9 14.72 9 15S9.22 15.5 9.5 15.5 10 15.28 10 15 9.78 14.5 9.5 14.5M9.5 8C9.78 8 10 7.78 10 7.5S9.78 7 9.5 7 9 7.22 9 7.5 9.22 8 9.5 8M14.5 8C14.78 8 15 7.78 15 7.5S14.78 7 14.5 7 14 7.22 14 7.5 14.22 8 14.5 8M12 10.5C11.72 10.5 11.5 10.72 11.5 11S11.72 11.5 12 11.5 12.5 11.28 12.5 11 12.28 10.5 12 10.5M20 11C20 15.42 16.42 19 12 19S4 15.42 4 11C4 8.61 5.06 6.47 6.72 5H17.28C18.94 6.47 20 8.61 20 11M13 7.5C13 8.33 13.67 9 14.5 9S16 8.33 16 7.5 15.33 6 14.5 6 13 6.67 13 7.5M8 7.5C8 8.33 8.67 9 9.5 9S11 8.33 11 7.5 10.33 6 9.5 6 8 6.67 8 7.5M7 13C8.11 13 9 12.11 9 11C9 9.9 8.11 9 7 9S5 9.9 5 11C5 12.11 5.9 13 7 13M11.5 15C11.5 13.9 10.61 13 9.5 13S7.5 13.9 7.5 15C7.5 16.11 8.4 17 9.5 17S11.5 16.11 11.5 15M12 13C13.11 13 14 12.11 14 11C14 9.9 13.11 9 12 9S10 9.9 10 11C10 12.11 10.9 13 12 13M16.5 15C16.5 13.9 15.61 13 14.5 13S12.5 13.9 12.5 15C12.5 16.11 13.4 17 14.5 17S16.5 16.11 16.5 15M19 11C19 9.9 18.11 9 17 9S15 9.9 15 11C15 12.11 15.9 13 17 13S19 12.11 19 11M17 10.5C16.72 10.5 16.5 10.72 16.5 11S16.72 11.5 17 11.5 17.5 11.28 17.5 11 17.28 10.5 17 10.5M14.5 14.5C14.22 14.5 14 14.72 14 15S14.22 15.5 14.5 15.5 15 15.28 15 15 14.78 14.5 14.5 14.5Z")  },
            { PackIconKind.PowerPlug, new PathInfo (PackIconFlipOrientation.Normal,"M16,7V3H14V7H10V3H8V7H8C7,7 6,8 6,9V14.5L9.5,18V21H14.5V18L18,14.5V9C18,8 17,7 16,7Z")  },
            { PackIconKind.PowerPlugOff, new PathInfo (PackIconFlipOrientation.Normal,"M20.84 22.73L15.31 17.2L14.5 18V21H9.5V18L6 14.5V9C6 8.7 6.1 8.41 6.25 8.14L1.11 3L2.39 1.73L22.11 21.46L20.84 22.73M18 14.5V9C18 8 17 7 16 7V3H14V7H10.2L17.85 14.65L18 14.5M10 3H8V4.8L10 6.8V3Z")  },
            { PackIconKind.Bluetooth, new PathInfo (PackIconFlipOrientation.Normal,"M14.88,16.29L13,18.17V14.41M13,5.83L14.88,7.71L13,9.58M17.71,7.71L12,2H11V9.58L6.41,5L5,6.41L10.59,12L5,17.58L6.41,19L11,14.41V22H12L17.71,16.29L13.41,12L17.71,7.71Z")  },
            { PackIconKind.BluetoothOff, new PathInfo (PackIconFlipOrientation.Normal,"M13,5.83L14.88,7.71L13.28,9.31L14.69,10.72L17.71,7.7L12,2H11V7.03L13,9.03M5.41,4L4,5.41L10.59,12L5,17.59L6.41,19L11,14.41V22H12L16.29,17.71L18.59,20L20,18.59M13,18.17V14.41L14.88,16.29")  },

        };
    }
}