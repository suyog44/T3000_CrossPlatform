﻿namespace T3000.Controls
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Collections.Generic;
    using MultipleMonthCalendarControls;

    public partial class MonthsControl : UserControl
    {
        #region Properties

        private int _dimensionX = 1;
        [Description("Dimension X value"), Category("MultipleMonthCalendar")]
        public int DimensionX {
            get { return _dimensionX; }
            set {
                _dimensionX = value;
                InitializeMonths();

                Invalidate();
            }
        }

        private int _dimensionY = 1;
        [Description("Dimension Y value"), Category("MultipleMonthCalendar")]
        public int DimensionY {
            get { return _dimensionY; }
            set {
                _dimensionY = value;
                InitializeMonths();

                Invalidate();
            }
        }

        private DateTime _startDate = DateTime.Today;
        [Description("Start date"), Category("MultipleMonthCalendar")]
        public DateTime StartDate {
            get { return _startDate; }
            set {
                _startDate = value;
                UpdateMonths();
                Invalidate();
            }
        }

        #endregion

        #region Months

        private List<MonthControl> Months = new List<MonthControl>();
        private void InitializeMonths()
        {
            var currentMonthLength = DimensionX * DimensionY;
            if (currentMonthLength == Months.Count)
            {
                return;
            }

            Controls.Clear();
            Months.Clear();

            for (var i = 0; i < currentMonthLength; ++i)
            {
                var month = new MonthControl();
                Controls.Add(month);
                Months.Add(month);
            }
            ResizeMonths();
            UpdateMonths();
        }

        private void ResizeMonths()
        {
            var width = (Width - 10) / (1.0 * DimensionX);
            var heigth = (Height - 10) / (1.0 * DimensionY);
            var size = new Size(Convert.ToInt32(width), Convert.ToInt32(heigth));
            for (var i = 0; i < Months.Count; ++i)
            {
                var month = Months[i];
                var x = i % DimensionX;
                var y = i / DimensionX;

                month.Left = 5 + x * size.Width;
                month.Top = 5 + y * size.Height;
                month.Size = size;
            }
        }

        private void UpdateMonths()
        {
            for (var i = 0; i < Months.Count; ++i)
            {
                var month = Months[i];

                month.Date = MonthUtilities.GetMonth(StartDate, i);
            }
        }

        #endregion

        #region Overrided

        protected override void OnResize(EventArgs e)
        {
            ResizeMonths();

            base.OnResize(e);
        }

        #endregion

        public MonthsControl()
        {
            InitializeComponent();
            InitializeMonths();

            ResizeRedraw = true;
        }
    }
}
