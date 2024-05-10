using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OhmStudio.UI.Controls
{
    [TemplateVisualState(GroupName = VisualStates.GroupActive, Name = VisualStates.StateActive)]
    [TemplateVisualState(GroupName = VisualStates.GroupActive, Name = VisualStates.StateInactive)]
    public class ProgressRing : Control
    {
        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing), new PropertyMetadata(false, new PropertyChangedCallback(IsActiveChanged)));

        // Using a DependencyProperty as the backing store for TemplateSettings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemplateSettingsProperty =
            DependencyProperty.Register("TemplateSettings", typeof(TemplateSettingValues), typeof(ProgressRing), new PropertyMetadata(null));

        private bool _hasAppliedTemplate = false;

        public ProgressRing()
        {
            DefaultStyleKey = typeof(ProgressRing);
            TemplateSettings = new TemplateSettingValues(60);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public TemplateSettingValues TemplateSettings
        {
            get => (TemplateSettingValues)GetValue(TemplateSettingsProperty);
            set => SetValue(TemplateSettingsProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _hasAppliedTemplate = true;
            UpdateState(IsActive);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var width = 20d;
            var height = 20d;
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                width = double.IsNaN(Width) == false ? Width : availableSize.Width;
                height = double.IsNaN(Height) == false ? Height : availableSize.Height;
            }

            TemplateSettings = new TemplateSettingValues(Math.Min(width, height));
            return base.MeasureOverride(availableSize);
        }

        private static void IsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var progressRing = (ProgressRing)sender;
            var isActive = (bool)e.NewValue;
            progressRing.UpdateState(isActive);
        }

        private void UpdateState(bool isActive)
        {
            if (_hasAppliedTemplate)
            {
                string state = isActive ? VisualStates.StateActive : VisualStates.StateInactive;
                VisualStateManager.GoToState(this, state, true);
            }
        }
    }

    public class TemplateSettingValues : DependencyObject
    {
        // Using a DependencyProperty as the backing store for MaxSideLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxSideLengthProperty =
            DependencyProperty.Register("MaxSideLength", typeof(double), typeof(TemplateSettingValues), new PropertyMetadata(0D));

        // Using a DependencyProperty as the backing store for EllipseDiameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EllipseDiameterProperty =
            DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(TemplateSettingValues), new PropertyMetadata(0D));

        // Using a DependencyProperty as the backing store for EllipseOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EllipseOffsetProperty =
            DependencyProperty.Register("EllipseOffset", typeof(Thickness), typeof(TemplateSettingValues), new PropertyMetadata(default(Thickness)));

        public TemplateSettingValues(double width)
        {
            if (width <= 40)
            {
                EllipseDiameter = (width / 10) + 1;
            }
            else
            {
                EllipseDiameter = width / 10;
            }
            MaxSideLength = width - EllipseDiameter;
            EllipseOffset = new Thickness(0, EllipseDiameter * 2.5, 0, 0);
        }

        public double MaxSideLength
        {
            get => (double)GetValue(MaxSideLengthProperty);
            set => SetValue(MaxSideLengthProperty, value);
        }

        public double EllipseDiameter
        {
            get => (double)GetValue(EllipseDiameterProperty);
            set => SetValue(EllipseDiameterProperty, value);
        }

        public Thickness EllipseOffset
        {
            get => (Thickness)GetValue(EllipseOffsetProperty);
            set => SetValue(EllipseOffsetProperty, value);
        }
    }

    internal static class VisualStates
    {
        #region GroupCommon

        /// <summary>
        /// Common state group.
        /// </summary>
        public const string GroupCommon = "CommonStates";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// Normal state of the Common state group.
        /// </summary>
        public const string StateReadOnly = "ReadOnly";

        /// <summary>
        /// MouseOver state of the Common state group.
        /// </summary>
        public const string StateMouseOver = "MouseOver";

        /// <summary>
        /// Pressed state of the Common state group.
        /// </summary>
        public const string StatePressed = "Pressed";

        /// <summary>
        /// Disabled state of the Common state group.
        /// </summary>
        public const string StateDisabled = "Disabled";
        #endregion GroupCommon

        #region GroupFocus

        /// <summary>
        /// Focus state group.
        /// </summary>
        public const string GroupFocus = "FocusStates";

        /// <summary>
        /// Unfocused state of the Focus state group.
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Focused state of the Focus state group.
        /// </summary>
        public const string StateFocused = "Focused";
        #endregion GroupFocus

        #region GroupSelection

        /// <summary>
        /// Selection state group.
        /// </summary>
        public const string GroupSelection = "SelectionStates";

        /// <summary>
        /// Selected state of the Selection state group.
        /// </summary>
        public const string StateSelected = "Selected";

        /// <summary>
        /// Unselected state of the Selection state group.
        /// </summary>
        public const string StateUnselected = "Unselected";

        /// <summary>
        /// Selected inactive state of the Selection state group.
        /// </summary>
        public const string StateSelectedInactive = "SelectedInactive";
        #endregion GroupSelection

        #region GroupExpansion

        /// <summary>
        /// Expansion state group.
        /// </summary>
        public const string GroupExpansion = "ExpansionStates";

        /// <summary>
        /// Expanded state of the Expansion state group.
        /// </summary>
        public const string StateExpanded = "Expanded";

        /// <summary>
        /// Collapsed state of the Expansion state group.
        /// </summary>
        public const string StateCollapsed = "Collapsed";
        #endregion GroupExpansion

        #region GroupPopup

        /// <summary>
        /// Popup state group.
        /// </summary>
        public const string GroupPopup = "PopupStates";

        /// <summary>
        /// Opened state of the Popup state group.
        /// </summary>
        public const string StatePopupOpened = "PopupOpened";

        /// <summary>
        /// Closed state of the Popup state group.
        /// </summary>
        public const string StatePopupClosed = "PopupClosed";
        #endregion

        #region GroupValidation

        /// <summary>
        /// ValidationStates state group.
        /// </summary>
        public const string GroupValidation = "ValidationStates";

        /// <summary>
        /// The valid state for the ValidationStates group.
        /// </summary>
        public const string StateValid = "Valid";

        /// <summary>
        /// Invalid, focused state for the ValidationStates group.
        /// </summary>
        public const string StateInvalidFocused = "InvalidFocused";

        /// <summary>
        /// Invalid, unfocused state for the ValidationStates group.
        /// </summary>
        public const string StateInvalidUnfocused = "InvalidUnfocused";
        #endregion

        #region GroupExpandDirection

        /// <summary>
        /// ExpandDirection state group.
        /// </summary>
        public const string GroupExpandDirection = "ExpandDirectionStates";

        /// <summary>
        /// Down expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandDown = "ExpandDown";

        /// <summary>
        /// Up expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandUp = "ExpandUp";

        /// <summary>
        /// Left expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandLeft = "ExpandLeft";

        /// <summary>
        /// Right expand direction state of ExpandDirection state group.
        /// </summary>
        public const string StateExpandRight = "ExpandRight";
        #endregion

        #region GroupHasItems

        /// <summary>
        /// HasItems state group.
        /// </summary>
        public const string GroupHasItems = "HasItemsStates";

        /// <summary>
        /// HasItems state of the HasItems state group.
        /// </summary>
        public const string StateHasItems = "HasItems";

        /// <summary>
        /// NoItems state of the HasItems state group.
        /// </summary>
        public const string StateNoItems = "NoItems";
        #endregion GroupHasItems

        #region GroupIncrease

        /// <summary>
        /// Increment state group.
        /// </summary>
        public const string GroupIncrease = "IncreaseStates";

        /// <summary>
        /// State enabled for increment group.
        /// </summary>
        public const string StateIncreaseEnabled = "IncreaseEnabled";

        /// <summary>
        /// State disabled for increment group.
        /// </summary>
        public const string StateIncreaseDisabled = "IncreaseDisabled";
        #endregion GroupIncrease

        #region GroupDecrease

        /// <summary>
        /// Decrement state group.
        /// </summary>
        public const string GroupDecrease = "DecreaseStates";

        /// <summary>
        /// State enabled for decrement group.
        /// </summary>
        public const string StateDecreaseEnabled = "DecreaseEnabled";

        /// <summary>
        /// State disabled for decrement group.
        /// </summary>
        public const string StateDecreaseDisabled = "DecreaseDisabled";
        #endregion GroupDecrease

        #region GroupIteractionMode

        /// <summary>
        /// InteractionMode state group.
        /// </summary>
        public const string GroupInteractionMode = "InteractionModeStates";

        /// <summary>
        /// Edit of the DisplayMode state group.
        /// </summary>
        public const string StateEdit = "Edit";

        /// <summary>
        /// Display of the DisplayMode state group.
        /// </summary>
        public const string StateDisplay = "Display";
        #endregion GroupIteractionMode

        #region GroupLocked

        /// <summary>
        /// DisplayMode state group.
        /// </summary>
        public const string GroupLocked = "LockedStates";

        /// <summary>
        /// Edit of the DisplayMode state group.
        /// </summary>
        public const string StateLocked = "Locked";

        /// <summary>
        /// Display of the DisplayMode state group.
        /// </summary>
        public const string StateUnlocked = "Unlocked";
        #endregion GroupLocked

        #region GroupActive

        /// <summary>
        /// Active state.
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Inactive state.
        /// </summary>
        public const string StateInactive = "Inactive";

        /// <summary>
        /// Active state group.
        /// </summary>
        public const string GroupActive = "ActiveStates";
        #endregion GroupActive

        #region GroupWatermark

        /// <summary>
        /// Non-watermarked state.
        /// </summary>
        public const string StateUnwatermarked = "Unwatermarked";

        /// <summary>
        /// Watermarked state.
        /// </summary>
        public const string StateWatermarked = "Watermarked";

        /// <summary>
        /// Watermark state group.
        /// </summary>
        public const string GroupWatermark = "WatermarkStates";
        #endregion GroupWatermark

        #region GroupCalendarButtonFocus

        /// <summary>
        /// Unfocused state for Calendar Buttons.
        /// </summary>
        public const string StateCalendarButtonUnfocused = "CalendarButtonUnfocused";

        /// <summary>
        /// Focused state for Calendar Buttons.
        /// </summary>
        public const string StateCalendarButtonFocused = "CalendarButtonFocused";

        /// <summary>
        /// CalendarButtons Focus state group.
        /// </summary>
        public const string GroupCalendarButtonFocus = "CalendarButtonFocusStates";
        #endregion GroupCalendarButtonFocus

        #region GroupBusyStatus

        /// <summary>
        /// Busy state for BusyIndicator.
        /// </summary>
        public const string StateBusy = "Busy";

        /// <summary>
        /// Idle state for BusyIndicator.
        /// </summary>
        public const string StateIdle = "Idle";

        /// <summary>
        /// Busyness group name.
        /// </summary>
        public const string GroupBusyStatus = "BusyStatusStates";
        #endregion

        #region GroupVisibility

        /// <summary>
        /// Visible state name for BusyIndicator.
        /// </summary>
        public const string StateVisible = "Visible";

        /// <summary>
        /// Hidden state name for BusyIndicator.
        /// </summary>
        public const string StateHidden = "Hidden";

        /// <summary>
        /// BusyDisplay group.
        /// </summary>
        public const string GroupVisibility = "VisibilityStates";
        #endregion

        /// <summary>
        /// Use VisualStateManager to change the visual state of the control.
        /// </summary>
        /// <param name="control">
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions">
        /// A value indicating whether to use transitions when updating the
        /// visual state, or to snap directly to the new visual state.
        /// </param>
        /// <param name="stateNames">
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used.
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(stateNames != null, "stateNames should not be null!");
            Debug.Assert(stateNames.Length > 0, "stateNames should not be empty!");

            foreach (string name in stateNames)
            {
                if (VisualStateManager.GoToState(control, name, useTransitions))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the implementation root of the Control.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject.</param>
        /// <remarks>
        /// Implements Silverlight's corresponding internal property on Control.
        /// </remarks>
        /// <returns>Returns the implementation root or null.</returns>
        public static FrameworkElement GetImplementationRoot(DependencyObject dependencyObject)
        {
            Debug.Assert(dependencyObject != null, "DependencyObject should not be null.");
            return (1 == VisualTreeHelper.GetChildrenCount(dependencyObject)) ?
                VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement :
                null;
        }

        /// <summary>
        /// This method tries to get the named VisualStateGroup for the
        /// dependency object. The provided object's ImplementationRoot will be
        /// looked up in this call.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="groupName">The visual state group's name.</param>
        /// <returns>Returns null or the VisualStateGroup object.</returns>
        public static VisualStateGroup TryGetVisualStateGroup(DependencyObject dependencyObject, string groupName)
        {
            FrameworkElement root = GetImplementationRoot(dependencyObject);
            if (root == null)
            {
                return null;
            }

            return VisualStateManager.GetVisualStateGroups(root)
                .OfType<VisualStateGroup>()
                .Where(group => string.CompareOrdinal(groupName, group.Name) == 0)
                .FirstOrDefault();
        }
    }
}