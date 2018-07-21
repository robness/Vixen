﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Catel.Data;
using Catel.MVVM;
using Common.WPFCommon.Command;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkCollectionViewModel: ViewModelBase
	{
		private System.Timers.Timer _nameclickTimer = null;

		public MarkCollectionViewModel(MarkCollection markCollection)
		{
			MarkCollection = markCollection;
			SetupCheckboxes();
		}

		#region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override Task CloseAsync()
		{
			return Task.Factory.StartNew(() =>
			{
				foreach (var checkBoxState in CheckBoxStates)
				{
					checkBoxState.CloseViewModelAsync(true);
				}
			});
		}

		#endregion

		#region MarkCollection model property

		/// <summary>
		/// Gets or sets the MarkCollection value.
		/// </summary>
		[Model]
		public MarkCollection MarkCollection
		{
			get { return GetValue<MarkCollection>(MarkCollectionProperty); }
			private set { SetValue(MarkCollectionProperty, value); }
		}

		/// <summary>
		/// MarkCollection property data.
		/// </summary>
		public static readonly PropertyData MarkCollectionProperty = RegisterProperty("MarkCollection", typeof(MarkCollection));

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

		#region ShowGridLines property

		/// <summary>
		/// Gets or sets the IsEnabled value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool ShowGridLines
		{
			get { return GetValue<bool>(ShowGridLinesProperty); }
			set { SetValue(ShowGridLinesProperty, value); }
		}

		/// <summary>
		/// IsEnabled property data.
		/// </summary>
		public static readonly PropertyData ShowGridLinesProperty = RegisterProperty("ShowGridLines", typeof(bool), null);

		#endregion

		#region ShowMarkBar property

		/// <summary>
		/// Gets or sets the ShowMarkBar value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool ShowMarkBar
		{
			get { return GetValue<bool>(ShowMarkBarProperty); }
			set { SetValue(ShowMarkBarProperty, value); }
		}

		/// <summary>
		/// ShowMarkBar property data.
		/// </summary>
		public static readonly PropertyData ShowMarkBarProperty = RegisterProperty("ShowMarkBar", typeof(bool), null);

		#endregion

		#region IsDefault property

		/// <summary>
		/// Gets or sets the IsDefault value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool IsDefault
		{
			get { return GetValue<bool>(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}

		/// <summary>
		/// IsDefault property data.
		/// </summary>
		public static readonly PropertyData IsDefaultProperty = RegisterProperty("IsDefault", typeof(bool), null);

		#endregion

		#region CollectionType property

		/// <summary>
		/// Gets or sets the CollectionType value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public MarkCollectionType CollectionType
		{
			get { return GetValue<MarkCollectionType>(CollectionTypeProperty); }
			set { SetValue(CollectionTypeProperty, value); }
		}

		/// <summary>
		/// CollectionType property data.
		/// </summary>
		public static readonly PropertyData CollectionTypeProperty = RegisterProperty("CollectionType", typeof(MarkCollectionType), null);

		#endregion
		
		#region BeginEdit command

		private Command<MouseButtonEventArgs> _beginEditCommand;

		/// <summary>
		/// Gets the LeftMouseUp command.
		/// </summary>
		
		public Command<MouseButtonEventArgs> BeginEditCommand
		{
			get { return _beginEditCommand ?? (_beginEditCommand = new Command<MouseButtonEventArgs>(BeginEdit)); }
		}

		/// <summary>
		/// Method to invoke when the LeftMouseUp command is executed.
		/// </summary>
		private void BeginEdit(MouseButtonEventArgs e)
		{
			if (_nameclickTimer == null)
			{
				_nameclickTimer = new System.Timers.Timer { Interval = 300};
				_nameclickTimer.Elapsed += nameClickTimer_Elapsed;
			}

			if (!_nameclickTimer.Enabled)
			{ // Equal: (e.ClickCount == 1)
				_nameclickTimer.Start();
			}
			else
			{ // Equal: (e.ClickCount == 2)
				_nameclickTimer.Stop();

				IsEditing = true;
			}

		}

		private void nameClickTimer_Elapsed(object sender, EventArgs e)
		{ // single-clicked
			_nameclickTimer.Stop();

		}

		#endregion

		#region IsEditing property

		/// <summary>
		/// Gets or sets the IsEditing value.
		/// </summary>
		public bool IsEditing
		{
			get { return GetValue<bool>(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}

		/// <summary>
		/// IsEditing property data.
		/// </summary>
		public static readonly PropertyData IsEditingProperty = RegisterProperty("IsEditing", typeof(bool));

		#endregion

		#region DoneEditing command

		private Command _doneEditingCommand;

		/// <summary>
		/// Gets the EditFocusLost command.
		/// </summary>
		
		public Command DoneEditingCommand
		{
			get { return _doneEditingCommand ?? (_doneEditingCommand = new Command(DoneEditing)); }
		}

		/// <summary>
		/// Method to invoke when the EditFocusLost command is executed.
		/// </summary>
		private void DoneEditing()
		{
			IsEditing = false;
			IsDirty = true;
		}

		#endregion

		#region Decorator property

		/// <summary>
		/// Gets or sets the Decorator value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public MarkDecorator Decorator
		{
			get { return GetValue<MarkDecorator>(DecoratorProperty); }
			set { SetValue(DecoratorProperty, value); }
		}

		/// <summary>
		/// Decorator property data.
		/// </summary>
		public static readonly PropertyData DecoratorProperty = RegisterProperty("Decorator", typeof(MarkDecorator), null);

		#endregion

		#region PickColor command

		private Command _pickColorCommand;

		/// <summary>
		/// Gets the PickColor command.
		/// </summary>
		public Command PickColorCommand
		{
			get { return _pickColorCommand ?? (_pickColorCommand = new Command(PickColor)); }
		}

		/// <summary>
		/// Method to invoke when the PickColor command is executed.
		/// </summary>
		private void PickColor()
		{
			var picker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();
			var result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				Decorator.Color = picker.Color.ToRGB().ToArgb();
			}
		}

		#endregion

		#region Delete command

		private Command _deleteCommand;

		/// <summary>
		/// Gets the Delete command.
		/// </summary>
		public Command DeleteCommand
		{
			get { return _deleteCommand ?? (_deleteCommand = new Command(Delete)); }
		}

		/// <summary>
		/// Method to invoke when the Delete command is executed.
		/// </summary>
		private void Delete()
		{
			var model = ParentViewModel as MarkDockerViewModel;
			model?.DeleteCollection(MarkCollection);
		}

		#endregion

		#region CheckBoxStates property

		/// <summary>
		/// Gets or sets the CheckBoxStates value.
		/// </summary>
		public ObservableCollection<CheckBoxState> CheckBoxStates
		{
			get { return GetValue<ObservableCollection<CheckBoxState>>(CheckBoxStatesProperty); }
			set { SetValue(CheckBoxStatesProperty, value); }
		}

		/// <summary>
		/// CheckBoxStates property data.
		/// </summary>
		public static readonly PropertyData CheckBoxStatesProperty = RegisterProperty("CheckBoxStates", typeof(ObservableCollection<CheckBoxState>));

		#endregion

		#region ResolveCollectionType command

		private Command<CheckBoxState> _resolveCollectionTypeCommand;

		/// <summary>
		/// Gets the ResolveCollectionType command.
		/// </summary>
		public Command<CheckBoxState> ResolveCollectionTypeCommand
		{
			get { return _resolveCollectionTypeCommand ?? (_resolveCollectionTypeCommand = new Command<CheckBoxState>(ResolveCollectionType)); }
		}

		/// <summary>
		/// Method to invoke when the ResolveCollectionType command is executed.
		/// </summary>
		private void ResolveCollectionType(CheckBoxState state)
		{
			if (state.Value)
			{
				CollectionType = state.Type;
				foreach (var checkBoxState in CheckBoxStates.Where(x => x.Type != state.Type))
				{
					checkBoxState.Value = false;
				}
			}
		}

		#endregion


		private void SetupCheckboxes()
		{
			CheckBoxStates = new ObservableCollection<CheckBoxState>();
			foreach (MarkCollectionType value in Enum.GetValues(typeof(MarkCollectionType)))
			{
				CheckBoxStates.Add(new CheckBoxState() { Text = value.ToString(), Value = (value == CollectionType), Type = value});
			}
		}
	}
}