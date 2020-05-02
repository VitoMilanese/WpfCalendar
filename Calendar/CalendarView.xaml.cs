using Calendar.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calendar
{
	/// <summary>
	/// Interaction logic for CalendarView.xaml
	/// </summary>
	public partial class CalendarView : UserControl
	{
		private CalendarViewModel _viewModel { get; set; }

		public CalendarView()
		{
			InitializeComponent();
			_viewModel = (CalendarViewModel)DataContext;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var fdw = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
			var daysOfWeek_int = Enum.GetValues(typeof(DayOfWeek)).Cast<int>().ToList();
			var sunday = 0;
			if (fdw == DayOfWeek.Monday)
			{
				daysOfWeek_int.Add(daysOfWeek_int[0]);
				daysOfWeek_int.RemoveAt(0);
				sunday = 6;
			}

			var daysOfTheWeek = new string[daysOfWeek_int.Count];
			for (var i = 0; i < daysOfWeek_int.Count; ++i)
				daysOfTheWeek[i] = ((DayOfWeek)daysOfWeek_int[i]).ToString().Substring(0, 3);

			for (var i = 0; i < 7; ++i)
				DaysOfTheWeek.Children.Add(new TextBlock
				{
					Text = daysOfTheWeek[i],
					Foreground = i == sunday ? Brushes.Red : Brushes.White
				});
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0) return;
			var month = e.AddedItems[0].ToString();
			RefreshCalendar(_viewModel.Months.IndexOf(month)+1);
		}

		private void DayButton_Click(object sender, RoutedEventArgs e)
		{
			var btn = (Button)sender;
			if (btn == null || btn.Content == null) return;
			int.TryParse(btn.Content.ToString(), out var day);
			var oldDay = _viewModel.SelectedDay;
			_viewModel.SelectedDay = day;
			var oldButton = DayButtonsContainer.Children.OfType<Button>().FirstOrDefault(c => (int)c.Content == oldDay);
			if (oldButton != null)
				oldButton.Foreground = btn.Foreground;
			btn.Foreground = Brushes.LimeGreen;
			SelectedDay.Text = day.ToString();
		}

		private void YearButton_Click(object sender, RoutedEventArgs e)
		{
			var btn = (Button)sender;
			if (btn == null || btn.Tag == null) return;
			int.TryParse(btn.Tag.ToString(), out var tag);
			if (tag == 0)
				--_viewModel.SelectedYear;
			else
				++_viewModel.SelectedYear;
			RefreshCalendar(_viewModel.SelectedMonth);
		}

		private void RefreshCalendar(int month)
		{
			_viewModel.SelectedMonth = month;
			DayButtonsContainer.Children.Clear();
			foreach (var day in _viewModel.Days)
			{
				if (day == 0)
					DayButtonsContainer.Children.Add(new Border());
				else
					DayButtonsContainer.Children.Add(new Button { Content = day });
			}
			SelectedDay.Text = _viewModel.SelectedDay.ToString();
			var newButton = DayButtonsContainer.Children.OfType<Button>().FirstOrDefault(c => (int)c.Content == _viewModel.SelectedDay);
			if (newButton != null)
				newButton.Foreground = Brushes.LimeGreen;
		}

		#region Dependencies

		public int Year
		{
			get => _viewModel.SelectedYear;
			set
			{
				SetValue(YearProperty, value);
				_viewModel.SelectedYear = value;
				RefreshCalendar(_viewModel.SelectedMonth);
			}
		}

		public static readonly DependencyProperty YearProperty =
			DependencyProperty.Register(
				"Year",
				typeof(int),
				typeof(CalendarView),
				new PropertyMetadata(default(int), (o, args) => ((CalendarView)o).Year = int.Parse(args.NewValue.ToString())));

		public int Month
		{
			get => _viewModel.SelectedMonth;
			set
			{
				SetValue(MonthProperty, value);
				_viewModel.SelectedMonth = value;
				MonthsListBox.SelectedIndex = value - 1;
			}
		}

		public static readonly DependencyProperty MonthProperty =
			DependencyProperty.Register(
				"Month",
				typeof(int),
				typeof(CalendarView),
				new PropertyMetadata(default(int), (o, args) => ((CalendarView)o).Month = int.Parse(args.NewValue.ToString())));

		public int Day
		{
			get => _viewModel.SelectedDay;
			set
			{
				SetValue(DayProperty, value);
				_viewModel.SelectedDay = value;
				RefreshCalendar(_viewModel.SelectedMonth);
			}
		}

		public static readonly DependencyProperty DayProperty =
			DependencyProperty.Register(
				"Day",
				typeof(int),
				typeof(CalendarView),
				new PropertyMetadata(default(int), (o, args) => ((CalendarView)o).Day = int.Parse(args.NewValue.ToString())));

		#endregion
	}
}
