﻿namespace T3000.Forms
{
    using PRGReaderLibrary;
    using Properties;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class VariablesForm : Form
    {
        public List<VariablePoint> Points { get; set; }
        public CustomUnits CustomUnits { get; private set; }

        public VariablesForm(List<VariablePoint> points, CustomUnits customUnits = null)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points));
            }

            Points = points;
            CustomUnits = customUnits;

            InitializeComponent();

            //User input handles
            view.AddEditHandler(AutoManualColumn, TViewUtilities.EditEnum<AutoManual>);
            view.AddEditAction(ValueColumn, TViewUtilities.EditValue,
                UnitsColumn, RangeColumn, CustomUnits);
            view.AddEditAction(UnitsColumn, TViewUtilities.EditUnitsColumn,
                ValueColumn, UnitsColumn, RangeColumn,
                CustomUnits, new Func<Unit, bool>(unit => unit.IsVariableAnalog()));
            view.AddEditHandler(StatusColumn, TViewUtilities.EditEnum<OffOn>);

            //Value changed handles
            view.AddChangedHandler(StatusColumn, TViewUtilities.ChangeColor, Color.Red, Color.Blue);
            view.AddChangedHandler(UnitsColumn, TViewUtilities.ChangeValue,
                AutoManualColumn, AutoManual.Manual);
            view.AddChangedHandler(ValueColumn, TViewUtilities.ChangeValue,
                AutoManualColumn, AutoManual.Manual);

            //Formating
            view.AddFormating(UnitsColumn, o => ((Unit)o).GetOffOnName(CustomUnits));

            //Show points
            view.Rows.Clear();
            view.Rows.Add(Points.Count);
            for (var i = 0; i < Points.Count; ++i)
            {
                var point = Points[i];
                var row = view.Rows[i];
                    //view.CreateRow(ToValues(point, i + 1));
                row.SetValue(NumberColumn, $"{i + 1}");
                SetRow(row, point);
                row.Cells[ValueColumn.Name] = 
                    TViewUtilities.GetValueCellForUnit(
                        point.Value.ToString(), 
                        point.Value.Unit);
            }

            //Validation
            view.AddValidation(DescriptionColumn, TViewUtilities.ValidateString, 21);
            view.AddValidation(LabelColumn, TViewUtilities.ValidateString, 9);
            view.AddValidation(ValueColumn, TViewUtilities.ValidateValue,
                ValueColumn, UnitsColumn, CustomUnits);
            view.AddValidation(UnitsColumn, TViewUtilities.ValidateValue,
                ValueColumn, UnitsColumn, CustomUnits);
            view.Validate();
        }

        private void SetRow(DataGridViewRow row, VariablePoint point)
        {
            if (row == null || point == null)
            {
                return;
            }

            row.SetValue(DescriptionColumn, point.Description);
            row.SetValue(AutoManualColumn, point.AutoManual);
            row.SetValue(ValueColumn, point.Value.ToString());
            row.SetValue(UnitsColumn, point.Value.Unit);
            row.SetValue(RangeColumn, point.Value.Value);
            row.SetValue(StatusColumn, point.Control);
            row.SetValue(LabelColumn, point.Label);
        }
        /*
        private Dictionary<string, object> ToValues(VariablePoint point, int number)
        {
            var values = new Dictionary<string, object>();
            values[NumberColumn.Name] = $"{number}";
            values[DescriptionColumn.Name] = point.Description;
            values[AutoManualColumn.Name] = point.AutoManual;
            values[ValueColumn.Name] = point.Value.ToString();
            values[UnitsColumn.Name] = point.Value.Unit;
            values[RangeColumn.Name] = point.Value.Value;
            values[StatusColumn.Name] = point.Control;
            values[LabelColumn.Name] = point.Label;

            return values;
        }
        */
        #region Buttons

        private void ClearSelectedRow(object sender, EventArgs e) =>
            SetRow(view.CurrentRow, new VariablePoint());

        private void Save(object sender, EventArgs e)
        {
            if (!view.Validate())
            {
                MessageBoxUtilities.ShowWarning(Resources.ViewNotValidated);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                for (var i = 0; i < view.RowCount && i < Points.Count; ++i)
                {
                    var point = Points[i];
                    var row = view.Rows[i];
                    point.Description = row.GetValue<string>(DescriptionColumn);
                    point.AutoManual = row.GetValue<AutoManual>(AutoManualColumn);
                    point.Value = TViewUtilities.GetVariableValue(row, ValueColumn, UnitsColumn, RangeColumn, CustomUnits);
                    point.Label = row.GetValue<string>(LabelColumn);
                }
            }
            catch (Exception exception)
            {
                MessageBoxUtilities.ShowException(exception);
                DialogResult = DialogResult.None;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
